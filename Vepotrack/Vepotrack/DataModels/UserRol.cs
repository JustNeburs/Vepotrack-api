using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vepotrack.API.DataModels
{
    /// <summary>
    /// Clase para definir si un usuario tiene permisos para una categoria
    /// </summary>
    public class UserRol : IdentityRole<Guid>
    {
        /// <summary>
        /// Texto para el rol de administraodr
        /// </summary>
        public const string AdminRol = "Admin";
        /// <summary>
        /// Texto para el rol de Vehiculo
        /// </summary>
        public const string VehicleRol = "Vehicle";
        /// <summary>
        /// Texto para el rol Regular
        /// </summary>
        public const string RegularRol = "Regular";
    }
}
