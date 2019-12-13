using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vepotrack.API.Models;
using Vepotrack.API.Services.Interfaces;

namespace Vepotrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/Order/P34242
        [HttpGet("{reference}", Name = "Get")]
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
        [HttpGet("Unique/{referenceUnique}", Name = "Get")]
        public async Task<OrderAPI> GetUnique(String referenceUnique)
        {
            return await _orderService.GetOrderUniqueReference(referenceUnique);
        }

        // POST: api/Order
        [Authorize(Policy = "IsVehicle")]
        [HttpPost]        
        public async Task<IActionResult> Post([FromBody] OrderDataAPI value)
        {
            if (await _orderService.AddOrder(value))
                return Ok();
            return BadRequest();
                
        }

        // PUT: api/Order/5
        [Authorize(Policy = "IsVehicle")]
        [HttpPut("{reference}")]
        public async Task<IActionResult> Put(String reference, [FromBody] OrderDataAPI value)
        {
            if (await _orderService.UpdateOrder(reference, value))
                return Ok();
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


    }
}
