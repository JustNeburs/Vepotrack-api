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
    /// Servicio para las funciones a realizar con vehiculos
    /// </summary>
    public class VehicleService : BaseService, IVehicleService
    {
        /// <summary>
        /// Variable donde se establecera el repositorio de usuarios
        /// </summary>
        private readonly IUserRepository _userRepository;
        /// <summary>
        /// Variable donde se establecera el repositorio de pedidos
        /// </summary>
        private readonly IOrderRepository _orderRepository;
        /// <summary>
        /// Variable donde se establecera el repositorio de vehículos
        /// </summary>
        private readonly IVehicleRepository _vehicleRepository;
        /// <summary>
        /// Servicio de notificaciones
        /// </summary>
        private readonly INotifyService _notifyService;
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository,
            IOrderRepository orderRepository,
            IVehicleRepository vehicleRepository,
            INotifyService notifyService,
            ILogger<VehicleService> logger) : base(httpContextAccessor)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _vehicleRepository = vehicleRepository;
            _notifyService = notifyService;
            _logger = logger;
        }
        /// <summary>
        /// Añade un vehículo al sistema
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> AddVehicle(VehicleDataAPI value)
        {
            // Comprobamos que sea administraodr
            if (!IsAdmin())
                return false;

            Guid? userId = null;
            if (!String.IsNullOrEmpty(value.UserName))
            {
                var usr = await _userRepository.GetUser(value.UserName);
                userId = usr?.Id;
            }

            // Añadimos el vehiculo
            return await _vehicleRepository.AddVehicle(value, userId);
        }
        /// <summary>
        /// Añade la posición de un vehículo dada la referencia de este
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> UpdateVehicle(string reference, VehicleDataAPI value)
        {
            // Comprobamos que tenga referencia y que sea administrador
            if (String.IsNullOrEmpty(reference) || !IsAdmin())
                return false;

            Guid? userId = null;
            if (!String.IsNullOrEmpty(value.UserName))
            {
                var usr = await _userRepository.GetUser(value.UserName);
                userId = usr?.Id;
            }
            // Actualizamos el vehiculo
            return await _vehicleRepository.UpdateVehicle(reference, value, userId);
        }
        /// <summary>
        /// Añade la posición de un vehículo dada la referencia de este
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> AddVehiclePosition(string reference, PositionAPI value)
        {
            // Si ni es un vehiculo ni administrador no podemos añadir la posición
            if (!IsVehicle() && !IsAdmin())
                return false;

            // Buscamos el vehículo
            var vehicle = await _vehicleRepository.Find(reference);
            if (vehicle == null)
                return false;
            // Si el usuario no es administrador vemos si es el usuario asociado al vehículo
            if (!IsAdmin() && vehicle.UserId != GetUserId())
                return false;
            if (value.SetDate == DateTime.MinValue)
                value.SetDate = DateTime.Now;
            // Añadimos la posición
            bool retValue = await _vehicleRepository.AddVehiclePosition(vehicle.Id, value);
            // Notificamos la posicion
            _notifyService.NotifyPosition(vehicle.Id, value);
            return retValue;
        }
        /// <summary>
        /// Obtiene la lista de vehiculos lista para devolverla por la API
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<VehicleAPI>> GetList()
        {
            if (!IsVehicle() && !IsAdmin())
                return new List<VehicleAPI>();

            IEnumerable<Vehicle> lstRet = new List<Vehicle>();
            if (!IsAdmin())
            {
                lstRet = await _vehicleRepository.GetVehicles(GetUserId());
            }
            else
            {
                lstRet = await _vehicleRepository.GetVehicles();
            }

            return lstRet?.Select(x => x.ToVehicleAPI()).ToList() ?? new List<VehicleAPI>();
        }
        /// <summary>
        /// Obtiene el vehículo indicado en la referencia
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public async Task<VehicleAPI> GetVehicle(string reference)
        {
            if (!IsVehicle() && !IsAdmin())
                return null;

            var vehicle = await _vehicleRepository.Find(reference);
            // Obtenemos los pedidos que actualmente tiene asignado el vehiculo
            IEnumerable<String> actualOrders = null;
            if (vehicle != null)
                actualOrders = await _orderRepository.GetActiveVehicleOrder(vehicle.Id);

            return vehicle?.ToVehicleAPI(actualOrders);
        }
        /// <summary>
        /// Obtiene las posiciones de un vehiculo dada la referencia y fechas desde y hasta
        /// </summary>
        /// <param name="reference">Referencia del vehículo</param>
        /// <param name="fromDate">Fecha Desde, puede ser NULL para que obtenga las posiciones desde el principio</param>
        /// <param name="toDate">Fecha Hasta, puede ser NULL para obtener todas las posiciones hasta la última grabada</param>
        /// <param name="pageLength">Longitud de página por si queremos los resultados paginados</param>
        /// <param name="pageNum">Número de página, solo valido si hemos indicado la longitud de página</param>
        /// <returns>Devuelve el listado de posiciones del vehículo indicado</returns>
        public async Task<IEnumerable<PositionAPI>> GetVehiclePositions(string reference, DateTime? fromDate = null, DateTime? toDate = null, int? pageLength = null, int? pageNum = null)
        {
            // Buscamos el vehiculo asocidado
            var vehicle = await _vehicleRepository.Find(reference);
            if (vehicle == null)
                return new List<PositionAPI>();

            // Si es un usuario regular solo se le permite ver el día de hoy
            if (!IsVehicle() && !IsAdmin())
            {
                fromDate = DateTime.Now.Date;
                toDate = null;
            }
            // Si los dos tiempos tienen valor y el Desde es mayor que el hasta le damos la vuelta
            if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
            {
                DateTime tmAux = toDate.Value;
                toDate = fromDate;
                fromDate = tmAux;
            }
            // Obtenemos las posiciones
            var positions = await _vehicleRepository.GetPositions(vehicle.Id, fromDate, toDate, pageLength, pageNum);
            // Convertimos las posiciones a un objeto que devuelva la API
            return positions?.Select(x => x.ToPositionAPI()).ToList() ?? new List<PositionAPI>();
        }

        
    }
}
