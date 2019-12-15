using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;
using Vepotrack.API.Models;

namespace Vepotrack.API.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Convierte un objeto de usuario de base de datos en un Usuario de API para 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserAPI ToUserAPI(this UserApp user)
        {
            if (user == null)
                return null;
            return new UserAPI()
            {
                Id = user.Id.ToString(),
                Username = user.UserName,
                Name = user.Name,
                LastLogin = user.LastLogin
            };
        }

        /// <summary>
        /// Convierte un objeto de pedido de base de datos en un Pedido para ser devuelto por la API 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static OrderAPI ToOrderAPI(this Order order, VehiclePosition lastPosition = null)
        {
            if (order == null)
                return null;
            return new OrderAPI
            {
                Reference = order.Reference,
                Address = order.Address,
                Created = order.Created,
                Status = order.Status.ToVisibleText(),
                ReferenceVehicle = order.Vehicle?.Reference ?? String.Empty,
                LastPosition = lastPosition?.ToPositionAPI()            
            };
        }
        /// <summary>
        /// Obtiene el objeto de vehiculo para ser devuelto por la API
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        public static VehicleAPI ToVehicleAPI(this Vehicle vehicle, IEnumerable<String> orders = null)
        {
            if (vehicle == null)
                return null;

            return new VehicleAPI
            {
                Reference = vehicle.Reference,
                Name = vehicle.Name,
                Plate = vehicle.Plate,
                DriverName = vehicle.User?.Name ?? String.Empty,
                ActualOrders = orders ?? new List<string>()               
            };
        }

        /// <summary>
        /// Obtiene la respresentación textual del estado de un pedido
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static String ToVisibleText(this OrderStatus status)
        {
            switch(status)
            {
                case OrderStatus.None: return "Desconocido";
                case OrderStatus.Added: return "Añadido en el sistema";
                case OrderStatus.Assigned: return "Asignado a vehículo";
                case OrderStatus.Cancel: return "Cancelado";
                case OrderStatus.InCourse: return "En ruta";
                case OrderStatus.Warning: return "Problema";
                case OrderStatus.Delivery: return "Entregado";
                default: return String.Empty;
            }
        }
        /// <summary>
        /// Obtenemos el objeto PositionAPI en base a la posición del vehiculo
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static PositionAPI ToPositionAPI(this VehiclePosition position)
        {
            if (position == null)
                return null;
            return new PositionAPI(position);
        }
    }
}
