using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vepotrack.API.Models;

namespace Vepotrack.API.Services.Interfaces
{
    public interface IVehicleService
    {
        /// <summary>
        /// Obtiene la lista de vehiculos lista para devolverla por la API
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<VehicleAPI>> GetList();
        /// <summary>
        /// Obtiene el vehículo indicado en la referencia
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        Task<VehicleAPI> GetVehicle(string reference);
        /// <summary>
        /// Añade un vehículo al sistema
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> AddVehicle(VehicleDataAPI value);
        /// <summary>
        /// Actualiza un vehículo del sistema basado en la referencia
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> UpdateVehicle(string reference, VehicleDataAPI value);
        /// <summary>
        /// Añade la posición de un vehículo dada la referencia de este
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> AddVehiclePosition(string reference, PositionAPI value);
        /// <summary>
        /// Obtiene las posiciones de un vehiculo dada la referencia y fechas desde y hasta
        /// </summary>
        /// <param name="reference">Referencia del vehículo</param>
        /// <param name="fromDate">Fecha Desde, puede ser NULL para que obtenga las posiciones desde el principio</param>
        /// <param name="toDate">Fecha Hasta, puede ser NULL para obtener todas las posiciones hasta la última grabada</param>
        /// <param name="pageLength">Longitud de página por si queremos los resultados paginados</param>
        /// <param name="pageNum">Número de página, solo valido si hemos indicado la longitud de página</param>
        /// <returns>Devuelve el listado de posiciones del vehículo indicado</returns>
        Task<IEnumerable<PositionAPI>> GetVehiclePositions(string reference, DateTime? fromDate = null, DateTime? toDate = null, int? pageLength = null, int? pageNum = null);
    }
}
