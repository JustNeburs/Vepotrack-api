using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;

namespace Vepotrack.API.Models
{
    public class PositionAPI
    {
        public PositionAPI()
        {

        }

        public PositionAPI(VehiclePosition position):this()
        {
            if (position == null)
                return;
            SetDate = position.SetDate;
            PositionFormat = position.PositionFormat;
            Latitude = position.Latitude;
            Longitude = position.Longitude;
            Precision = position.Precision;
            ExtendedInfo = position.ExtendedInfo;
        }

        /// <summary>
        /// Fecha/hora de captura
        /// </summary>
        public DateTime SetDate { get; set; }
        /// <summary>
        /// Tipo de posición, se pone cadena para dejar abierta la ampliación (Latitud|Longitud, Posición en GMaps, ...)
        /// </summary>
        public String PositionFormat { get; set; }
        /// <summary>
        /// Latitud de la posición
        /// </summary>
        public Decimal Latitude { get; set; }
        /// <summary>
        /// Longitud de la posición
        /// </summary>
        public Decimal Longitude { get; set; }
        /// <summary>
        /// Precision en metros
        /// </summary>
        public Decimal Precision { get; set; }
        /// <summary>
        /// Información extendida/auxiliar de esta posición.
        /// </summary>
        public String ExtendedInfo { get; set; }
    }
}
