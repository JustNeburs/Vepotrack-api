using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vepotrack.API.Models
{
    public class VehicleDataAPI
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
        /// Usuario asociado al vehiculo, es nullable porque puede que un vehiculo no tenga un usuario asociado actualmente
        /// </summary>
        public String UserName { get; set; }

    }
}
