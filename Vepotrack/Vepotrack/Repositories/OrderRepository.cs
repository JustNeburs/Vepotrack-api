using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;
using Vepotrack.API.Models;
using Vepotrack.API.Persistence.Contexts;
using Vepotrack.API.Repositories.Interfaces;

namespace Vepotrack.API.Repositories
{
    /// <summary>
    /// Repositorio para el acceso a datos de los pedidos
    /// </summary>
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(ApiDbContext context,
            ILogger<OrderRepository> logger) : base(context)
        {
            _logger = logger;
        }
        /// <summary>
        /// Añade un pedido al sistema
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> AddOrder(OrderDataAPI value, Guid? userId = null, Guid? vehicleId = null)
        {
            if (value == null)
                return false;

            try
            {
                // Añadimos el pedido
                var order = await _context.Orders.AddAsync(new Order
                {
                    Reference = value.Reference,
                    ReferenceUniqueAccess = value.ReferenceUniqueAccess,
                    Created = DateTime.Now,
                    Address = value.Address,
                    Status = value.Status,
                    UserId = userId,
                    VehicleId = vehicleId
                });

                return await _context.SaveChangesAsync() > 0;

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error añadiendo pedido. {@value}", value);
            }
            return false;
        }
        /// <summary>
        /// Actualiza un pedido en el sistema
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> UpdateOrder(Guid idUserChange, string reference, OrderDataAPI value, Guid? userId = null, Guid? vehicleId = null)
        {
            if (value == null)
                return false;

            var orderPrevious = _context.Orders.FirstOrDefault(x => x.Reference == reference);

            if (orderPrevious == null)
                return false;

            Dictionary<String, String> changes = new Dictionary<string, string>();
            //Cargamos los cambios
            if (orderPrevious.Reference != value.Reference)
            {
                changes["Reference"] = orderPrevious.Reference;
                orderPrevious.Reference = value.Reference;
            }
            if (orderPrevious.ReferenceUniqueAccess != value.ReferenceUniqueAccess)
            {
                changes["ReferenceUniqueAccess"] = orderPrevious.ReferenceUniqueAccess;
                orderPrevious.ReferenceUniqueAccess = value.ReferenceUniqueAccess;
            }
            if (orderPrevious.Address != value.Address)
            {
                changes["Address"] = orderPrevious.Address;
                orderPrevious.Address = value.Address;
            }
            if (orderPrevious.Status != value.Status)
            {
                changes["Status"] = orderPrevious.Status.ToString();
                orderPrevious.Status = value.Status;
            }
            if (orderPrevious.UserId != userId)
            {
                changes["UserId"] = userId?.ToString() ?? String.Empty;
                orderPrevious.UserId = userId;
            }

            if (orderPrevious.VehicleId != vehicleId)
            {
                changes["VehicleId"] = vehicleId?.ToString() ?? String.Empty;
                orderPrevious.VehicleId = vehicleId;
            }
            // Si no hay cambios no actualizamos nada
            if (!changes.Any())
                return true;

            try
            {
                // Añadimos el pedido
                var order = _context.Orders.Update(orderPrevious);
                if (order != null)
                {
                    // Generamos un id para todo el grupo de cambios
                    Guid groupChangesId = Guid.NewGuid();
                    DateTime dtNow = DateTime.Now;
                    // Ahora añadimos los cambios
                    foreach(var kvp in changes)
                    {
                        await _context.OrderChanges.AddAsync(new OrderChanges
                        {
                            OrderId = orderPrevious.Id,
                            ChangeId = groupChangesId,
                            ChangeDate = dtNow,
                            FieldChange = kvp.Key,
                            OldValue = kvp.Value,    
                            UserChange = idUserChange
                        });
                    }
                }

                return await _context.SaveChangesAsync() > 0;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando pedido({reference}). {@value}",reference, value);
            }
            return false;
        }
        /// <summary>
        /// Asigna un pedido y un vehículo
        /// </summary>
        /// <param name="referenceOrder"></param>
        /// <param name="referenceVehicle"></param>
        /// <returns></returns>
        public async Task<bool> Assign(Guid idUserChange, string referenceOrder, string referenceVehicle)
        {
            try
            {
                var order = _context.Orders.FirstOrDefault(x => x.Reference == referenceOrder);
                var vehicle = _context.Vehicles.FirstOrDefault(x => x.Reference == referenceVehicle);
                if (order == null || vehicle == null)
                    return false;

                if (order.VehicleId == vehicle.Id && order.Status > OrderStatus.Assigned)
                    return true;

                // Asignamos el vehículo
                order.VehicleId = vehicle.Id;
                String statusAssign = null;
                if (order.Status < OrderStatus.Assigned)
                {
                    statusAssign = order.Status.ToString();
                    order.Status = OrderStatus.Assigned;                    
                }
                // Actualizamos el pedido
                var ord = _context.Orders.Update(order);

                if (ord != null)
                {
                    Guid groupGuid = Guid.NewGuid();
                    // Guardamos los cambios
                    await _context.OrderChanges.AddAsync(new OrderChanges
                    {
                        OrderId = order.Id,
                        ChangeId = groupGuid,
                        ChangeDate = DateTime.Now,
                        FieldChange = "VehicleId",
                        OldValue = vehicle.Id.ToString(),
                        UserChange = idUserChange
                    });
                    if (!String.IsNullOrEmpty(statusAssign))
                    {
                        await _context.OrderChanges.AddAsync(new OrderChanges
                        {
                            OrderId = order.Id,
                            ChangeId = groupGuid,
                            ChangeDate = DateTime.Now,
                            FieldChange = "Status",
                            OldValue = statusAssign,
                            UserChange = idUserChange
                        });
                    }
                }

                return await _context.SaveChangesAsync() > 0;

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error asignando vehículo ({referenceVehicle}} al pedido ({referenceOrder}).", referenceVehicle, referenceOrder);
            }

            return false;
        }
        /// <summary>
        /// Obtiene el pedido dada la referencia o referencia única
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="referenceUniqueAccess"></param>
        /// <returns></returns>
        public async Task<Order> Find(string reference, string referenceUniqueAccess = null)
        {
            try
            {
                // Buscamos el pedido por referencia o referencia unica
                if (!String.IsNullOrEmpty(reference))
                    return _context.Orders.Include(x => x.User).Include(x => x.Vehicle).FirstOrDefault(x => x.Reference == reference);
                else if (!String.IsNullOrEmpty(referenceUniqueAccess))
                    return _context.Orders.Include(x => x.User).Include(x => x.Vehicle).FirstOrDefault(x => x.ReferenceUniqueAccess == referenceUniqueAccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buscando pedido ({reference},{referenceUniqueAccess}).", reference, referenceUniqueAccess);
            }

            return null;
        }
        /// <summary>
        /// Obtiene el pedido dado su Id Interno
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Order> Find(Guid id)
        {
            try
            {
                return await _context.Orders.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buscando pedido por id ({id}).", id);
            }

            return null;
        }
        /// <summary>
        /// Obtenemos la referencias a los pedidos abiertos incluidos en el vehículo
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<String>> GetActiveVehicleOrder(Guid vehicleId)
        {
            try
            {
                return await _context.Orders
                    .Where(x => x.VehicleId == vehicleId && (x.Status == OrderStatus.Assigned || x.Status == OrderStatus.InCourse))
                    .Select(x => x.Reference)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo pedidos abiertos para el vehiculo {vehicleId}", vehicleId);
            }

            return new List<String>();
        }

        public async Task<IEnumerable<Order>> GetOrders(Guid? vehicleId = null)
        {
            try
            {
                if (vehicleId.HasValue)
                   return await _context.Orders.Include(x => x.User).Include(x => x.Vehicle).Where(x => x.VehicleId == vehicleId).ToListAsync();
                return await _context.Orders.Include(x => x.User).Include(x => x.Vehicle).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo pedidos para el vehiculo {vehicleId}", vehicleId);
            }

            return new List<Order>();
        }
    }
}
