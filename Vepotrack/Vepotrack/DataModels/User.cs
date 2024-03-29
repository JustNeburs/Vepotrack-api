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
        /// Establecemos el tipo del Claim para el Id Interno
        /// </summary>
        public const String IdentityIdClaim = "InternalId";

        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Ultimo login del usuario
        /// </summary>
        public DateTime? LastLogin { get; set; }               


        public virtual List<Order> Orders { get; set; }
        public virtual List<OrderChanges> OrderChanges { get; set; }
        public virtual Vehicle Vehicle { get; set; }
    }
}
