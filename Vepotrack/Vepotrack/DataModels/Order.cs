using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vepotrack.API.DataModels
{
    /// <summary>
    /// Generamos un Enum para los estados de una orden dejando espacio entre los valores para posibles ampliaciones
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Sin estado
        /// </summary>
        None = 0,
        /// <summary>
        /// Añadida al sistema, estado por defecto 
        /// </summary>
        Added = 10,
        /// <summary>
        /// Asignada a un vehiculo
        /// </summary>
        Assigned = 20,
        /// <summary>
        /// Una vez el vehiculo esta en movimiento el pedido esta en reparto
        /// </summary>
        InCourse = 30,
        /// <summary>
        /// Pedido entregado
        /// </summary>
        Delivery = 40,
        /// <summary>
        /// Problemas con el pedido
        /// </summary>
        Warning = 50,
        /// <summary>
        /// Pedido cancelado
        /// </summary>
        Cancel = 60
    }
    /// <summary>
    /// Clase que simboliza un pedido 
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Id interno generado
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// Referencia unica del pedido para acceso exterior
        /// </summary>
        [Required]
        public String Reference { get; set; }
        /// <summary>
        /// Referencia Unica para Acceso al pedido sin necesidad de login
        /// </summary>
        [Required]
        public String ReferenceUniqueAccess { get; set; }
        /// <summary>
        /// Fecha de creación en el sistema
        /// </summary>
        [Required]
        public DateTime Created { get; set; }
        /// <summary>
        /// Dirección del pedido
        /// </summary>
        public String Address { get; set; }
        /// <summary>
        /// Estado del pedido
        /// </summary>
        [Required]
        public OrderStatus Status { get; set; }
        /// <summary>
        /// Id del usuario asociado al pedido, Nullable para permitir meter pedidos sin necesidad de que el usuario este dado de alta en el sistema
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// Id del vehiculo que tiene actualmente asignado el pedido
        /// </summary>
        public Guid? VehicleId { get; set; }

        /// <summary>
        /// Usuario asociado al pedido
        /// </summary>
        public virtual UserApp User { get; set; }
        /// <summary>
        /// Vehiculo al que esta asociado el pedido actualmente
        /// </summary>
        public virtual Vehicle Vehicle { get; set; }

        /// <summary>
        /// Listado de cambios asociados al pedido
        /// </summary>
        public virtual List<OrderChanges> OrderChanges { get; set; }
    }
}
