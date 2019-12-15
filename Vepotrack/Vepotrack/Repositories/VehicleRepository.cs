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
    /// Repositorio para el acceso a datos de vehiculos
    /// </summary>
    public class VehicleRepository : BaseRepository, IVehicleRepository
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<VehicleRepository> _logger;

        public VehicleRepository(ApiDbContext context,
            ILogger<VehicleRepository> logger) : base(context)
        {
            _logger = logger;
        }
        /// <summary>
        /// Añade un vehiculo al sistema
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> AddVehicle(VehicleDataAPI value, Guid? userId = null)
        {
            if (value == null)
                return false;

            try
            {
                // Añadimos el vehículo
                var vehicle = await _context.Vehicles.AddAsync(new Vehicle
                {
                    Reference = value.Reference,
                    Name = value.Name,
                    Plate = value.Plate,
                    UserId = userId
                });

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error añadiendo vehiculo. {@value}", value);
            }

            return false;
        }
        /// <summary>
        /// Actualiza un vehículo al sistema
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> UpdateVehicle(string reference, VehicleDataAPI value, Guid? userId = null)
        {
            if (value == null)
                return false;

            try
            {
                var vehicle = await _context.Vehicles.FirstOrDefaultAsync(x => x.Reference == reference);
                if (vehicle == null)
                    return false;

                // Actualizamos los campos del vehiculos
                vehicle.Reference = value.Reference;
                vehicle.Name = value.Name;
                vehicle.Plate = value.Plate;
                vehicle.UserId = userId;

                // actualizamos el vehículo
                var veh = _context.Vehicles.Update(vehicle);

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando vehiculo. {@value}", value);
            }

            return false;
        }
        /// <summary>
        /// Añade una posición a un vehiculo dado el id de este y los datos de posición
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> AddVehiclePosition(Guid vehicleId, PositionAPI value)
        {
            if (value == null || vehicleId == Guid.Empty)
                return false;

            try
            {
                var position = await _context.VehiclePositions.AddAsync(new VehiclePosition
                {
                    VehicleId = vehicleId,
                    SetDate = value.SetDate,
                    PositionFormat = value.PositionFormat,
                    Latitude = value.Latitude,
                    Longitude = value.Longitude,
                    Precision = value.Precision,
                    ExtendedInfo = value.ExtendedInfo
                });

                return await _context.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error añadiendo posición del vehiculo. {@value}", value);
            }

            return false;
        }
        /// <summary>
        /// Busca un vehículo por referencia
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Vehicle> Find(string referenceVehicle)
        {
            if (String.IsNullOrEmpty(referenceVehicle))
                return null;

            try
            {
                return await _context.Vehicles.Include(x => x.User).FirstOrDefaultAsync(x => x.Reference == referenceVehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buscando un vehiculo. {referenceVehicle}", referenceVehicle);
            }

            return null;
        }
        /// <summary>
        /// Busca un vehículo por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Vehicle> Find(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            try
            {
                return await _context.Vehicles.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buscando un vehiculo por id. {id}", id);
            }

            return null;
        }
        /// <summary>
        /// Obtiene la última posición de un vehículo
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public async Task<VehiclePosition> GetLastPosition(Guid vehicleId)
        {
            if (vehicleId == Guid.Empty)
                return null;

            try
            {
                return await _context.VehiclePositions.OrderByDescending(x => x.SetDate).FirstOrDefaultAsync(x => x.VehicleId == vehicleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buscando la última posición del  vehiculo {vehicleId}", vehicleId);
            }

            return null;
        }
        /// <summary>
        /// Obtiene las posiciones de un vehículo dado el Id de este, las fechas desde y hasta; la longitud de página y el número de página
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="pageLength"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public async Task<IEnumerable<VehiclePosition>> GetPositions(Guid vehicleId, DateTime? fromDate, DateTime? toDate, int? pageLength = null, int? pageNum = null)
        {
            if (vehicleId == Guid.Empty)
                return new List<VehiclePosition>(); 
            try
            {
                var query = _context.VehiclePositions.Where(x => x.VehicleId == vehicleId);
                // Si tenemos el 'Desde' filtramos con el desde
                if (fromDate.HasValue)
                    query = query.Where(x => x.SetDate >= fromDate.Value);
                // Si tenemos el 'Hasta' filtramos por el hasta
                if (toDate.HasValue)
                    query = query.Where(x => x.SetDate <= toDate.Value);
                // Si tenemos paginación la aplicamos
                if (pageLength.HasValue && pageNum.HasValue && pageLength > 0 && pageNum > 0)
                    query = query.Skip((pageNum.Value - 1) * pageLength.Value).Take(pageLength.Value);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buscando las posiciones del  vehiculo {vehicleId} para la fechas ({fromDate},{toDate})", vehicleId, fromDate, toDate);
            }

            return new List<VehiclePosition>();
        }
        /// <summary>
        /// Obtiene los vehículos pudiendo indicar el usuario asigando para que filtre por este
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Vehicle>> GetVehicles(Guid? userId = null)
        {
            try
            {
                if (userId.HasValue)
                    return await _context.Vehicles.Include(x => x.User).Where(x => x.UserId == userId).ToListAsync();
                return await _context.Vehicles.Include(x => x.User).ToListAsync();
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error obteneindo vehiculos ({userId})", userId);
            }

            return new List<Vehicle>();
        }
        
    }
}
