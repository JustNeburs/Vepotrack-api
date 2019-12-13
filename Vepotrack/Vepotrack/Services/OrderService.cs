using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private IOrderRepository _orderRepository;
        private IVehicleRepository _vehicleRepository;

        public OrderService(IHttpContextAccessor httpContextAccessor,
            IOrderRepository orderRepository,
            IVehicleRepository vehicleRepository) : base(httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _vehicleRepository = vehicleRepository;
        }

        public async Task<bool> AddOrder(OrderDataAPI value)
        {
            // Si no tenemos asociada la referencia no es correcto
            if (String.IsNullOrEmpty(value?.Reference))
                return false;
            // Si existe un pedido con esa referencia devolvemos un fallo
            if (await _orderRepository.Find(value.Reference, value.ReferenceUniqueAccess) != null)
                return false;
            // Chequeamos que el vehiculo exista en caso de estar indicado
            if (!String.IsNullOrEmpty(value.ReferenceVehicle) && await _vehicleRepository.Find(value.ReferenceVehicle) == null)
                return false;
            // Creamos el pedido
            return await _orderRepository.AddOrder(value);
        }

        public async Task<bool> UpdateOrder(String reference, OrderDataAPI value)
        {
            // Si no tenemos asociada la referencia no es correcto
            if (String.IsNullOrEmpty(reference) || String.IsNullOrEmpty(value?.Reference))
                return false;
            // Creamos el pedido
            return await _orderRepository.UpdateOrder(reference, value);
        }

        public async Task<bool> AssignOrderVehicle(string referenceOrder, string referenceVehicle)
        {
            // Si no tenemos asociada la referencia no es correcto
            if (String.IsNullOrEmpty(referenceOrder) || String.IsNullOrEmpty(referenceVehicle))
                return false;

            var order = await _orderRepository.Find(referenceOrder);
            var vehicle = await _vehicleRepository.Find(referenceVehicle);
            // Chequeamos que el vehiculo y el pedido existan
            if (order == null || vehicle == null)
                return false;

            // Si no es administrador y no es el usuario del vehiculo indicado no puede asignar el pedido al vehiculo
            if (!IsAdmin() && vehicle.UserId != GetUserId())
                throw new ExtendedResultException(new StatusCodeResult(StatusCodes.Status401Unauthorized));

            return await _orderRepository.Assign(referenceOrder, referenceVehicle);
        }

        public async Task<OrderAPI> GetOrder(string reference)
        {
            var order = await _orderRepository.Find(reference);
            VehiclePosition position = null;
            if (order?.VehicleId != null)
                position = await _vehicleRepository.GetLastPosition(order.VehicleId);

            return order?.ToOrderAPI(position);
        }

        public async Task<OrderAPI> GetOrderUniqueReference(string uniqueReference)
        {
            var order = await _orderRepository.Find(null, uniqueReference);
            VehiclePosition position = null;
            if (order?.VehicleId != null)
                position = await _vehicleRepository.GetLastPosition(order.VehicleId);

            return order?.ToOrderAPI(position);
        }

        
    }
}
