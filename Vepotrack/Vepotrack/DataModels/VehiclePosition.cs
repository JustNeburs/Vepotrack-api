using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vepotrack.API.DataModels
{
    public class VehiclePosition
    {
        /// <summary>
        /// Id interno generado
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// Vehiculo de esta posición
        /// </summary>
        [Required]
        public Guid VehicleId { get; set; }
        /// <summary>
        /// Fecha/hora de captura
        /// </summary>
        [Required]
        public DateTime SetDate { get; set; }
        /// <summary>
        /// Tipo de posición, se pone cadena para dejar abierta la ampliación (Latitud|Longitud, Posición en GMaps, ...)
        /// </summary>
        public String PositionFormat { get; set; }
        /// <summary>
        /// Latitud de la posición
        /// </summary>
        [Required]
        public Decimal Latitude { get; set; }
        /// <summary>
        /// Longitud de la posición
        /// </summary>
        [Required]
        public Decimal Longitude { get; set; }
        /// <summary>
        /// Precision en metros
        /// </summary>
        [Required]
        public Decimal Precision { get; set; }
        /// <summary>
        /// Información extendida/auxiliar de esta posición.
        /// </summary>
        public String ExtendedInfo { get; set; }

    }
}
