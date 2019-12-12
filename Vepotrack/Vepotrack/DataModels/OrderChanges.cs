using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vepotrack.API.DataModels
{
    public class OrderChanges
    {
        /// <summary>
        /// Id interno generado
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// Id del pedido asociado
        /// </summary>
        [Required]
        public Guid OrderId { get; set; }
        /// <summary>
        /// Id del grupo de cambios asociado
        /// </summary>
        [Required]
        public Guid ChangeId { get; set; }
        /// <summary>
        /// Fecha del cambio
        /// </summary>
        [Required]
        public DateTime ChangeDate { get; set; }
        /// <summary>
        /// Nombre del campo modificado
        /// </summary>
        [Required]
        public String FieldChange { get; set; }
        /// <summary>
        /// Valor anterior del campo
        /// </summary>
        [Required]
        public String OldValue { get; set; }
        /// <summary>
        /// Usuario que genero el cambio
        /// </summary>
        [Required]
        public Guid UserChange { get; set; }

        /// <summary>
        /// Objeto que identifica el pedido
        /// </summary>
        public virtual Order Order { get; set; }

    }
}
