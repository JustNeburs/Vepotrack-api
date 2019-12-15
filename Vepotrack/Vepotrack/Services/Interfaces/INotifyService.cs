using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.Models;

namespace Vepotrack.API.Services.Interfaces
{
    /// <summary>
    /// Enumeración para definir el tipo de servicio a usar para notificar
    /// </summary>
    public enum NotifyType
    {
        /// <summary>
        /// Notificación a Webhook
        /// </summary>
        Webhook = 1,
        /// <summary>
        /// Notificación mediante servicio MQTT
        /// </summary>
        MQTT = 2
    }

    public interface INotifyService
    {
        /// <summary>
        /// Notificamos la posición de un vehículo
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="position"></param>
        void NotifyPosition(Guid vehicleId, PositionAPI position);
        /// <summary>
        /// Añadimos un destino de notificación
        /// </summary>
        /// <param name="referenceOrder"></param>
        /// <param name="notifyType"></param>
        /// <param name="destiny"></param>
        /// <returns></returns>
        Task<bool> AddNotifyDestiny(String referenceOrder, NotifyType notifyType, String destiny = null);
    }
}
