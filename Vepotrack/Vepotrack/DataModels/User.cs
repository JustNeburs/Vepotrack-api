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
    public class User
    {
        /// <summary>
        /// Id único del usuario
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        /// <summary>
        /// Nombre de usuario, normalmente será único también
        /// </summary>
        [Required]
        public String Username { get; set; }
        /// <summary>
        /// Hash de la contraseña
        /// </summary>
        public String Hash { get; set; }
        /// <summary>
        /// Componente a añadir a la contraseña para generar un Hash mayor
        /// </summary>
        public String BackHash { get; set; }
        /// <summary>
        /// Fecha del último login
        /// </summary>
        public DateTime? LastLogin { get; set; }
        /// <summary>
        /// Fecha del último cambio de contraseña
        /// </summary>
        public DateTime? LastPassChange { get; set; }
        /// <summary>
        /// Indica si esta habilitado el usuario
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Carga de permisos del usuario
        /// </summary>
        public virtual IList<UserPermission> UserPermissions { get; set; }

    }
}
