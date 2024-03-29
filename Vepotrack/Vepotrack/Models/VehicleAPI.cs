﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vepotrack.API.Models
{
    public class VehicleAPI
    {
        /// <summary>
        /// Referencia unica del vehiculo
        /// </summary>
        public String Reference { get; set; }
        /// <summary>
        /// Nombre representativo del vehiculo
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Matricula del vehiculo
        /// </summary>
        public String Plate { get; set; }
        /// <summary>
        /// Nombre del usuario asociado al vehiculo
        /// </summary>
        public String DriverName { get; set; }

        /// <summary>
        /// Pedidos que actualmente tienen una asociación al vehiculo y cuyo estado es 
        /// </summary>
        public IEnumerable<String> ActualOrders { get; set; }
    }
}
