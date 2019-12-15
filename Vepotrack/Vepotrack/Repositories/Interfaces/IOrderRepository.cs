using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;
using Vepotrack.API.Models;

namespace Vepotrack.API.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        /// <summary>
        /// Obtiene el pedido dada la referencia o referencia única
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="referenceUniqueAccess"></param>
        /// <returns></returns>
        Task<Order> Find(string reference, string referenceUniqueAccess = null);
        /// <summary>
        /// Obtiene el pedido dado su Id Interno
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Order> Find(Guid id);
        /// <summary>
        /// Añade un pedido al sistema
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> AddOrder(OrderDataAPI value, Guid? userId = null, Guid? vehicleId = null);
        /// <summary>
        /// Actualiza un pedido en el sistema
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> UpdateOrder(Guid idUserChange, string reference, OrderDataAPI value, Guid? userId = null, Guid? vehicleId = null);
        /// <summary>
        /// Asigna un pedido y un vehículo
        /// </summary>
        /// <param name="referenceOrder"></param>
        /// <param name="referenceVehicle"></param>
        /// <returns></returns>
        Task<bool> Assign(Guid idUserChange, string referenceOrder, string referenceVehicle);

        /// <summary>
        /// Obtenemos la referencias a los pedidos abiertos incluidos en el vehículo
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        Task<IEnumerable<String>> GetActiveVehicleOrder(Guid vehicleId);
        /// <summary>
        /// Obtenemos los pedidos
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        Task<IEnumerable<Order>> GetOrders(Guid? vehicleId = null);


    }
}
