using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vepotrack.API.DataModels
{
    /// <summary>
    /// Enum que identifica los posibles valores de permisos, utilizamos Flags para combinar los permisos.
    /// </summary>
    [Flags]
    public enum Permission
    {
        None = 0,
        Read = 1,
        Write = 2,
        ReadWrite = Read | Write,
        Execute = 4,
        ReadWriteExecute = ReadWrite | Execute,
        Special = 1024,
        All = ReadWriteExecute | Special
    }
    /// <summary>
    /// Clase para definir si un usuario tiene permisos para una categoria
    /// </summary>
    public class UserPermission
    {
        /// <summary>
        /// Identificador del usuario
        /// </summary>
        [Key()]
        public Guid UserId { get; set; }
        /// <summary>
        /// Nombre único de la categoria
        /// </summary>
        [Key]
        public String Category { get; set; }
        /// <summary>
        /// Identifica los permisos asociados a la categoria
        /// </summary>
        [Required]
        public Permission Permission { get; set; }

        /// <summary>
        /// Carga del usuario asociado
        /// </summary>
        public virtual User User { get; set; }
        /// <summary>
        /// Metodo que nos indica rapidamente si tiene un permiso indicado
        /// </summary>
        /// <param name="permission">Permiso a chequear</param>
        /// <returns>True si contiene el permiso o False si no lo contiene</returns>
        public virtual bool Has(Permission permission) => Permission.HasFlag(permission);

    }
}
