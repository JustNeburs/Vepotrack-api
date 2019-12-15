using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vepotrack.API.Models;
using Vepotrack.API.Services.Interfaces;
using Vepotrack.API.Utils;

namespace Vepotrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        /// <summary>
        /// Servicio de funciones de pedidos
        /// </summary>
        private IOrderService _orderService;
        /// <summary>
        /// Servicio de funciones de notificación
        /// </summary>
        private INotifyService _notifyService;

        public OrderController(IOrderService orderService, 
            INotifyService notifyService)
        {
            _orderService = orderService;
            _notifyService = notifyService;
        }

        // GET: api/Order/P34242
        [Authorize(Policy ="IsVehicle")]
        [HttpGet]
        public async Task<IEnumerable<OrderAPI>> GetList()
        {
            return await _orderService.GetOrders();
        }

        // GET: api/Order/P34242
        [HttpGet("{reference}")]
        public async Task<OrderAPI> Get(String reference)
        {
            return await _orderService.GetOrder(reference);
        }

        /// <summary>
        /// Servicio web anonimo para acceso al pedido sin login
        /// </summary>
        /// <param name="ReferenceUnique">Referencia unica asociada al pedido</param>
        /// <returns></returns>        
        [AllowAnonymous]
        [HttpGet("Unique/{referenceUnique}")]
        public async Task<OrderAPI> GetUnique(String referenceUnique)
        {
            return await _orderService.GetOrderUniqueReference(referenceUnique);
        }

        // POST: api/Order
        [Authorize(Policy = "IsVehicle")]
        [HttpPost]        
        public async Task<IActionResult> Post([FromBody] OrderDataAPI value)
        {
            try
            {
                if (await _orderService.AddOrder(value))
                    return Ok();
                
            }catch(Exception ex)
            {
                // Capturamos la excepcion de resultado extendido
                if (ex is ExtendedResultException)
                {
                    var result = ex as ExtendedResultException;
                    return result.ExtendedResult;
                }
            }

            return BadRequest();

        }

        // PUT: api/Order/5
        [Authorize(Policy = "IsVehicle")]
        [HttpPut("{reference}")]
        public async Task<IActionResult> Put(String reference, [FromBody] OrderDataAPI value)
        {
            try
            { 
                if (await _orderService.UpdateOrder(reference, value))
                    return Ok();
            }
            catch (Exception ex)
            {
                // Capturamos la excepcion de resultado extendido
                if (ex is ExtendedResultException)
                {
                    var result = ex as ExtendedResultException;
                    return result.ExtendedResult;
                }
            }

            return BadRequest();
        }

        /// <summary>
        /// Servicio web para facilitar la asignación de pedidos a vehiculos
        /// </summary>
        /// <param name="referenceOrder">Referencia al pedido</param>
        /// <param name="referenceVehicle">Referencia al vehiculo</param>
        /// <returns></returns>
        [Authorize(Policy = "IsVehicle")]
        [HttpPost("assign/{referenceOrder}/{referenceVehicle}")]
        public async Task<IActionResult> Assign(String referenceOrder, String referenceVehicle)
        {
            if (await _orderService.AssignOrderVehicle(referenceOrder, referenceVehicle))
                return Ok();
            return BadRequest();
        }

        /// <summary>
        /// Servicio web para facilitar la asignación de notificación
        /// </summary>
        /// <param name="referenceOrder">Referencia al pedido</param>
        /// <returns></returns>
        [HttpPost("notify/webhook/{referenceOrder}")]
        public async Task<IActionResult> NotifyWebhook(String referenceOrder, [FromBody] OrderDataNotify value)
        {
            if (String.IsNullOrEmpty(referenceOrder) || String.IsNullOrEmpty(value?.WebhookUrl))
                return BadRequest();

            if (await _notifyService.AddNotifyDestiny(referenceOrder, NotifyType.Webhook, value.WebhookUrl))
                return Ok();
            return BadRequest();
        }

        /// <summary>
        /// Servicio web para facilitar la asignación de notificación
        /// </summary>
        /// <param name="referenceOrder">Referencia al pedido</param>
        /// <returns></returns>
        [HttpPost("notify/mqtt/{referenceOrder}")]
        public async Task<IActionResult> NotifyMqtt(String referenceOrder)
        {
            if (String.IsNullOrEmpty(referenceOrder))
                return BadRequest();

            if (await _notifyService.AddNotifyDestiny(referenceOrder, NotifyType.MQTT))
                return Ok();
            return BadRequest();
        }


    }
}
