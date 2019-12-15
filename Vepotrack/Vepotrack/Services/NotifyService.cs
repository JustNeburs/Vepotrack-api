using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vepotrack.API.Models;
using Vepotrack.API.Repositories.Interfaces;
using Vepotrack.API.Services.Interfaces;

namespace Vepotrack.API.Services
{
    public class NotifyService : BaseService, INotifyService
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<NotifyService> _logger;

        /// <summary>
        /// Objeto para proteguer el acceso al diccionario 
        /// </summary>
        private object LockDictionary = new object();

        /// <summary>
        /// Dicciónario de notificaciones
        /// </summary>
        protected Dictionary<Guid, List<NotifyElement>> notifications = new Dictionary<Guid, List<NotifyElement>>();

        /// <summary>
        /// Timer de chequeo de notificaciones
        /// </summary>
        protected System.Timers.Timer timerCheckNotification;
        /// <summary>
        /// Agregamos la factoria de Scope para llamarla en la asignación de notificación
        /// </summary>
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public NotifyService(IHttpContextAccessor httpContextAccessor,
             IServiceScopeFactory serviceScopeFactory,
            ILogger<NotifyService> logger) : base(httpContextAccessor)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            // El timer es de un minuto
            timerCheckNotification = new System.Timers.Timer(60 * 1000);
            timerCheckNotification.Elapsed += TimerCheckNotification_Elapsed;
            timerCheckNotification.Enabled = true;
        }        

        /// <summary>
        /// Añadimos un destino de notificación
        /// </summary>
        /// <param name="referenceOrder"></param>
        /// <param name="notifyType"></param>
        /// <param name="destiny"></param>
        /// <returns></returns>
        public async Task<bool> AddNotifyDestiny(string referenceOrder, NotifyType notifyType, string destiny = null)
        {
            // Si es de tipo Webhook y si no viene la url de envio o no esta bien formada no añadimos nada
            if (notifyType == NotifyType.Webhook && (!String.IsNullOrEmpty(destiny) || !Uri.IsWellFormedUriString(destiny, UriKind.RelativeOrAbsolute)))
                return false;
            // Si no tenemos la factoria no podemos añadir notificaciones
            if (_serviceScopeFactory == null)
                return false;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var orderRepo = scope.ServiceProvider.GetService<IOrderRepository>();
                var userRepo = scope.ServiceProvider.GetService<IUserRepository>();
                var vehiRepo = scope.ServiceProvider.GetService<IVehicleRepository>();

                // Buscamos el pedido
                var order = await orderRepo.Find(referenceOrder);
                if (order?.VehicleId == null || (order.Status != DataModels.OrderStatus.Assigned && order.Status != DataModels.OrderStatus.InCourse))
                    return false;
                // Obtenemos el usuario
                var user = await userRepo.GetUser(GetUserId());
                // Obtenemos el vehículo
                var vehicle = await vehiRepo.Find(order.VehicleId.Value);
                // Para un usuario estandar asignamos el tiempo de caducidad de la notificación en 5 minutos
                int minutes = 5;
                // Para un administrador un día
                if (IsAdmin())
                    minutes = 1440;
                // Para un vehiculo medio día
                else if (IsVehicle())
                    minutes = 720;

                lock (LockDictionary)
                {
                    // Revisamos que exista datos de notificación para el vehículo
                    if (!notifications.ContainsKey(order.VehicleId.Value))
                    {
                        notifications[order.VehicleId.Value] = new List<NotifyElement>();
                    }
                    // Obtenemos la lista de notificaciones
                    var listNotify = notifications[order.VehicleId.Value];
                    // Buscamos una para el usuario actual
                    var notification = listNotify.FirstOrDefault(x => x.UserId == GetUserId());
                    // Si no estaba previamente la notificación la añadimos
                    if (notification == null)
                    {
                        // Si no existia previamente la añadimos
                        listNotify.Add(new NotifyElement
                        {
                            Data = destiny,
                            DtExpire = DateTime.Now.AddMinutes(minutes),
                            UserId = GetUserId(),
                            VehicleReference = vehicle.Reference,
                            UserName = user.UserName,
                            NotifyType = notifyType
                        });
                    }
                    else
                    {
                        // Actualizamos la información de notificación
                        notification.Data = destiny;
                        notification.DtExpire = DateTime.Now.AddMinutes(minutes);
                        notification.NotifyType = notifyType;
                    }
                }
            }

            return true;
        }
        /// <summary>
        /// Notificamos la posición de un vehículo
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="position"></param>
        public void NotifyPosition(Guid vehicleId, PositionAPI position)
        {
            List<NotifyElement> lstNotify = null;
            lock (LockDictionary)
            {
                if (!notifications.ContainsKey(vehicleId))
                    return;
                var list = notifications[vehicleId];
                if (list == null || !list.Any())
                    return;

                lstNotify = list.GetRange(0, list.Count);
            }
            // Lanzamos un hilo para las notificaciones
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                // Llamamos a lanzar las notificaciones
                LaunchNotifyList(lstNotify, position);
            }).Start();
        }

        /// <summary>
        /// Metodo generado para lanzarse en un hilo y notificar la posicion 
        /// </summary>
        /// <param name="lstCalls"></param>
        /// <param name="position"></param>
        private void LaunchNotifyList(List<NotifyElement> lstCalls, PositionAPI position)
        {
            if (lstCalls == null || position == null)
                return;
            // Enviamos las notificaciones
            foreach(var call in lstCalls)
            {
                try
                {
                    switch(call.NotifyType)
                    {
                        case NotifyType.Webhook: LaunchWebhook(call, position);break;
                        case NotifyType.MQTT: LaunchMQTT(call, position);break;
                    }                    
                }
                catch (Exception)
                {
                    // No dejaremos que cualquier excepción aqui evite el resto de notificaciones
                }
            }
        }
        /// <summary>
        ///  Lanzamos una notificación Webhook
        /// </summary>
        /// <param name="notify"></param>
        /// <param name="position"></param>
        private void LaunchWebhook(NotifyElement notify, PositionAPI position)
        {
            try
            {
                // Enviamos la notificación al webhook sin esperar la respuesta
                using (HttpClient client = new HttpClient())
                {
                    client.PostAsJsonAsync(notify.Data, new NotifyWebhook
                    {
                        Position = position,
                        VehicleReference = notify.VehicleReference,
                        UserName = notify.UserName
                    });
                }
            }catch(Exception)
            {

            }
        }

        /// <summary>
        /// Lanzamos una notificación MQTT
        /// </summary>
        /// <param name="notify"></param>
        /// <param name="position"></param>
        private void LaunchMQTT(NotifyElement notify, PositionAPI position)
        {
            //TODO: Por hacer
            /// Se llamará al cliente MQTT para añadir una notificación en el servidor 
            /// con ruta el Usuario/Referencia del Vehículo.
        }

        /// <summary>
        /// Chequeo de caducidad de notificaciones
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerCheckNotification_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerCheckNotification.Enabled = false;

            lock (LockDictionary)
            {
                foreach(var kvp in notifications)
                {
                    // Si no hay elementos continuamos con el siguiente
                    if (kvp.Value == null || !kvp.Value.Any())
                        continue;
                    // Borramos todos los que hayan expirado
                    kvp.Value.RemoveAll(x => x.DtExpire < DateTime.Now);
                }
            }

            timerCheckNotification.Enabled = true;
        }
    }
}
