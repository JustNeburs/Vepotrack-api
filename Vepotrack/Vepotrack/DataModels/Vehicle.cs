using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vepotrack.API.DataModels
{
    public class Vehicle
    {
        /// <summary>
        /// Id interno generado
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// Referencia unica del vehiculo
        /// </summary>
        [Required]
        public String Reference { get; set; }
        /// <summary>
        /// Nombre representativo del vehiculo
        /// </summary>
        [Required]
        public String Name { get; set; }
        /// <summary>
        /// Matricula del vehiculo
        /// </summary>
        public String Plate { get; set; }
        /// <summary>
        /// Usuario asociado al vehiculo, es nullable porque puede que un vehiculo no tenga un usuario asociado actualmente
        /// </summary>
        public Guid? UserId { get; set; }
                
        /// <summary>
        /// Usuario asociado al vehiculo
        /// </summary>
        public virtual UserApp User { get; set; }
        /// <summary>
        /// Pedidos que actualmente tienen una asociación al vehiculo
        /// </summary>
        public virtual List<Order> Orders { get; set; }

        /// No se agrega la propiedad de navegación hacia posiciones del vehiculo
        /// de forma intencionada, así evitamos que de forma inconsciente intente 
        /// cargar todas las posicones de un vehiculo que pueden ser muchas y
        /// ralentizar la carga de datos     
    }
}
