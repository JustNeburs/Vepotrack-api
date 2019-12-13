using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;

namespace Vepotrack.API.Models
{
    public class OrderAPI
    {
        /// <summary>
        /// Referencia unica del pedido 
        /// </summary>
        public String Reference { get; set; }
        /// <summary>
        /// Fecha de creación en el sistema
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// Dirección del pedido
        /// </summary>
        public String Address { get; set; }
        /// <summary>
        /// Estado del pedido
        /// </summary>
        public String Status { get; set; }
        /// <summary>
        /// Vehiculo del pedido
        /// </summary>
        public String ReferenceVehicle { get; set; }
        /// <summary>
        /// Ultima posición del pedido
        /// </summary>
        public PositionAPI LastPosition { get; set; }

    }
}
