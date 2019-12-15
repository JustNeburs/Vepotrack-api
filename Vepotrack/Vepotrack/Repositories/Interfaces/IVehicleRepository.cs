using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;
using Vepotrack.API.Models;

namespace Vepotrack.API.Repositories.Interfaces
{
    public interface IVehicleRepository
    {
        /// <summary>
        /// Busca un vehículo por referencia
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Vehicle> Find(string referenceVehicle);
        /// <summary>
        /// Busca un vehículo por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Vehicle> Find(Guid id);
        /// <summary>
        /// Obtiene la última posición de un vehículo
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        Task<VehiclePosition> GetLastPosition(Guid vehicleId);
        /// <summary>
        /// Obtiene las posiciones de un vehículo dado el Id de este, las fechas desde y hasta; la longitud de página y el número de página
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="pageLength"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        Task<IEnumerable<VehiclePosition>> GetPositions(Guid vehicleId, DateTime? fromDate, DateTime? toDate, int? pageLength = null, int? pageNum = null);
        /// <summary>
        /// Obtiene los vehículos pudiendo indicar el usuario asigando para que filtre por este
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<Vehicle>> GetVehicles(Guid? userId = null);
        /// <summary>
        /// Añade una posición a un vehiculo dado el id de este y los datos de posición
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> AddVehiclePosition(Guid vehicleId, PositionAPI value);
        /// <summary>
        /// Añade un vehiculo al sistema
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> AddVehicle(VehicleDataAPI value, Guid? userId = null);
        /// <summary>
        /// Actualiza un vehículo al sistema
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> UpdateVehicle(string reference, VehicleDataAPI value, Guid? userId = null);
    }
}
