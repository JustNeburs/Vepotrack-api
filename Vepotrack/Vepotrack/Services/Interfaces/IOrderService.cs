using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.Models;

namespace Vepotrack.API.Services.Interfaces
{
    public interface IOrderService
    {

        /// <summary>
        /// Obtiene el listado de pedidos
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<OrderAPI>> GetOrders();
        /// <summary>
        /// Obtiene un pedido dada la referencia
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        Task<OrderAPI> GetOrder(string reference);
        /// <summary>
        /// Obitne un pedido dada la referencia única de búsqueda
        /// </summary>
        /// <param name="uniqueReference"></param>
        /// <returns></returns>
        Task<OrderAPI> GetOrderUniqueReference(string uniqueReference);
        /// <summary>
        /// Añade un pedido
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> AddOrder(OrderDataAPI value);
        /// <summary>
        /// Actualiza un pedido dada la referencia y los nuevos datos
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> UpdateOrder(String reference, OrderDataAPI value);
        /// <summary>
        /// Asgian un pedido a un vehiculo
        /// </summary>
        /// <param name="referenceOrder"></param>
        /// <param name="referenceVehicle"></param>
        /// <returns></returns>
        Task<bool> AssignOrderVehicle(string referenceOrder, string referenceVehicle);
    }
}
