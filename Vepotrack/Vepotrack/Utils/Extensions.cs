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
                LastLogin = user.LastLogin
            };
        }

        /// <summary>
        /// Convierte un objeto de pedido de base de datos en un Pedido de API para 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static OrderAPI ToOrderAPI(this Order order, VehiclePosition lastPosition)
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

        public static PositionAPI ToPositionAPI(this VehiclePosition position)
        {
            if (position == null)
                return null;
            return new PositionAPI(position);
        }
    }
}
