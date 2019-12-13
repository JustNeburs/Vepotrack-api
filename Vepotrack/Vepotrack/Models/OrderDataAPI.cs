using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vepotrack.API.Models
{
    public class OrderDataAPI
    {
        /// <summary>
        /// Referencia unica del pedido para acceso exterior
        /// </summary>
        public String Reference { get; set; }
        /// <summary>
        /// Referencia del vehículo asignado
        /// </summary>
        public String ReferenceVehicle { get; set; }
        /// <summary>
        /// Referencia Unica para Acceso al pedido sin necesidad de login
        /// </summary>
        public String ReferenceUniqueAccess { get; set; }
        /// <summary>
        /// Dirección del pedido
        /// </summary>
        public String Address { get; set; }
    }
}
