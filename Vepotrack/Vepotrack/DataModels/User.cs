﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Vepotrack.API.DataModels
{
    /// <summary>
    /// Clase de usuario para identificar la información de este
    /// </summary>
    public class UserApp : IdentityUser<Guid>
    {
        /// <summary>
        /// Elemento a añadir al hash de la contraseña antes de generarlo
        /// </summary>
        public String BackHash { get; set; }
        /// <summary>
        /// Ultimo login del usuario
        /// </summary>
        public DateTime? LastLogin { get; set; }
        /// <summary>
        /// Identifica si el usuario es administrador
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
