using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vepotrack.API.DataModels;
using Vepotrack.API.Models;
using Vepotrack.API.Repositories.Interfaces;
using Vepotrack.API.Services.Interfaces;
using Vepotrack.API.Utils;

namespace Vepotrack.API.Services
{
    /// <summary>
    /// Servicio para las funciones a realizar con pedidos
    /// </summary>
    public class OrderService : BaseService, IOrderService
    {
        /// <summary>
        /// Repositorio de pedidos
        /// </summary>
        private IOrderRepository _orderRepository;
        /// <summary>
        /// Repositorio de vehiculos
        /// </summary>
        private IVehicleRepository _vehicleRepository;
        /// <summary>
        /// Repositorio de usuarios
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<OrderService> _logger;

        public OrderService(IHttpContextAccessor httpContextAccessor,
            IOrderRepository orderRepository,
            IVehicleRepository vehicleRepository,
            IUserRepository userRepository,
            ILogger<OrderService> logger) : base(httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _vehicleRepository = vehicleRepository;
            _userRepository = userRepository;
            _logger = logger;
        }
        /// <summary>
        /// Obtiene un listado de pedidos
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<OrderAPI>> GetOrders()
        {
            if (!IsVehicle() && !IsAdmin())
                return new List<OrderAPI>();

            IEnumerable<Order> lstRet = new List<Order>();
            if (!IsAdmin())
            {
                lstRet = await _orderRepository.GetOrders(GetUserId());
            }
            else
            {
                lstRet = await _orderRepository.GetOrders();
            }

            return lstRet?.Select(x => x.ToOrderAPI()).ToList() ?? new List<OrderAPI>();
        }
        /// <summary>
        /// Añade un pedido
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> AddOrder(OrderDataAPI value)
        {
            // Si no tenemos asociada la referencia no es correcto
            if (String.IsNullOrEmpty(value?.Reference) || (!IsAdmin() && !IsVehicle()))
                return false;
            // Si existe un pedido con esa referencia devolvemos un fallo
            if (await _orderRepository.Find(value.Reference, value.ReferenceUniqueAccess) != null)
                return false;
            Guid? vehicleId = null;
            Guid? userId = null;
            if (!String.IsNullOrEmpty(value.UserName))
            {
                var usr = await _userRepository.GetUser(value.UserName);
                userId = usr?.Id;
            }

            // Chequeamos que el vehiculo exista en caso de estar indicado
            if (!String.IsNullOrEmpty(value.ReferenceVehicle))
            {
                var vehicle = await _vehicleRepository.Find(value.ReferenceVehicle);
                if (vehicle == null)
                    return false;
                // Solo asociamos el vehiculo si es administrador o el usuario es el que lo tiene asignado
                if (IsAdmin() || vehicle.UserId == GetUserId())
                    vehicleId = vehicle?.Id;
                else
                    throw new ExtendedResultException(new StatusCodeResult(StatusCodes.Status401Unauthorized));
            }
            // Si no es administrador asociamos el vehiculo
            if (!IsAdmin())
            {
                // Asignamos el primer vehiculo del usuario asociado
                var vehicles = await _vehicleRepository.GetVehicles(GetUserId());
                vehicleId = vehicles?.FirstOrDefault()?.Id;
            }

            if (!Enum.IsDefined(typeof(OrderStatus), value.Status))
                value.Status = vehicleId.HasValue ? OrderStatus.Assigned : OrderStatus.Added;

            // Creamos el pedido
            return await _orderRepository.AddOrder(value, userId, vehicleId);
        }
        /// <summary>
        /// Actualiza el pedido
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> UpdateOrder(String reference, OrderDataAPI value)
        {
            // Si no tenemos asociada la referencia no es correcto
            if (String.IsNullOrEmpty(reference) || String.IsNullOrEmpty(value?.Reference) || (!IsAdmin() && !IsVehicle()))
                return false;
            
            Guid? vehicleId = null;
            Guid? userId = null;
            if (!String.IsNullOrEmpty(value.UserName))
            {
                var usr = await _userRepository.GetUser(value.UserName);
                userId = usr?.Id;
            }
            // Si es un vehículo forzamos a asignar el Id como el del vehículo
            if (!IsAdmin())
            {
                // Asignamso el primer vehiculo del usuario asociado
                var vehicles = await _vehicleRepository.GetVehicles(GetUserId());
                vehicleId = vehicles?.FirstOrDefault()?.Id;
            }
            else if (!String.IsNullOrEmpty(value.ReferenceVehicle))
            {
                var vehicle = await _vehicleRepository.Find(value.ReferenceVehicle);
                vehicleId = vehicle?.Id;
            }

            // Creamos el pedido
            return await _orderRepository.UpdateOrder(GetUserId(), reference, value, userId, vehicleId);
        }
        /// <summary>
        /// Asigna el pedido al vehiculo indicados
        /// </summary>
        /// <param name="referenceOrder"></param>
        /// <param name="referenceVehicle"></param>
        /// <returns></returns>
        public async Task<bool> AssignOrderVehicle(string referenceOrder, string referenceVehicle)
        {
            // Si no tenemos asociada la referencia no es correcto
            if (String.IsNullOrEmpty(referenceOrder) || String.IsNullOrEmpty(referenceVehicle) || (!IsAdmin() && !IsVehicle()))
                return false;

            var order = await _orderRepository.Find(referenceOrder);
            var vehicle = await _vehicleRepository.Find(referenceVehicle);
            // Chequeamos que el vehiculo y el pedido existan
            if (order == null || vehicle == null)
                return false;

            // Si no es administrador y no es el usuario del vehiculo indicado no puede asignar el pedido al vehiculo
            if (!IsAdmin() && vehicle.UserId != GetUserId())
                throw new ExtendedResultException(new StatusCodeResult(StatusCodes.Status401Unauthorized));

            return await _orderRepository.Assign(GetUserId(), referenceOrder, referenceVehicle);
        }

        /// <summary>
        /// Obtiene el pedido dada la referencia
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public async Task<OrderAPI> GetOrder(string reference)
        {
            // Obtenemos el pedido
            var order = await _orderRepository.Find(reference);
            if (order == null)
                return null;
            // Si no es administrador o vehiculo no puede ver los pedidos, solo puede ver los suyos
            if (!IsAdmin() && !IsVehicle() && order.UserId != GetUserId())
                return null;
            // Asiganmos la ultima posición del vehiculo del pedido
            VehiclePosition position = null;
            if (order?.VehicleId != null)
                position = await _vehicleRepository.GetLastPosition(order.VehicleId.Value);

            return order?.ToOrderAPI(position);
        }
        /// <summary>
        /// Obtiene el pedido dada la referencia unica
        /// </summary>
        /// <param name="uniqueReference"></param>
        /// <returns></returns>
        public async Task<OrderAPI> GetOrderUniqueReference(string uniqueReference)
        {
            // Obtenemos el pedido
            var order = await _orderRepository.Find(null, uniqueReference);
            VehiclePosition position = null;
            // Asiganmos la ultima posición del vehiculo del pedido
            if (order?.VehicleId != null)
                position = await _vehicleRepository.GetLastPosition(order.VehicleId.Value);

            return order?.ToOrderAPI(position);
        }

        
    }
}
