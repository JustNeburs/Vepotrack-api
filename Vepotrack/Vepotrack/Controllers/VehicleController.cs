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
    public class VehicleController : ControllerBase
    {
        private IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        // GET: api/Vehicle
        [Authorize(Policy = "IsVehicle")]
        [HttpGet]
        public async Task<IEnumerable<VehicleAPI>> Get()
        {
            return await _vehicleService.GetList();
        }

        // GET: api/Vehicle/5
        [Authorize(Policy = "IsVehicle")]
        [HttpGet("{reference}")]
        public async Task<VehicleAPI> Get(string reference)
        {
            return await _vehicleService.GetVehicle(reference);
        }

        // POST: api/Vehicle
        [Authorize(Policy = "IsAdmin")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] VehicleDataAPI value)
        {
            if (value == null)
                return BadRequest();

            if(!await _vehicleService.AddVehicle(value))
                return BadRequest();
            return Ok();
        }

        // PUT: api/Vehicle/5
        [Authorize(Policy = "IsAdmin")]
        [HttpPut("{reference}")]
        public async Task<IActionResult> Put(string reference, [FromBody] VehicleDataAPI value)
        {
            if (String.IsNullOrEmpty(reference) || value == null)
                return BadRequest();

            if (!await _vehicleService.UpdateVehicle(reference, value))
                return BadRequest();
            return Ok();
        }

        // POST: api/Vehicle
        [Authorize(Policy = "IsVehicle")]
        [HttpPost("position/{reference}")]
        public async Task<IActionResult> SetVehiclePosition(string reference, [FromBody] PositionAPI value)
        {
            if (String.IsNullOrEmpty(reference) || value == null)
                return BadRequest();

            if(!await _vehicleService.AddVehiclePosition(reference, value))
                return BadRequest();
            return Ok();
        }
        
        // GET: api/Vehicle/5
        [HttpGet("today/{reference}")]
        public async Task<IEnumerable<PositionAPI>> Today(string reference)
        {
            // Si viene la referencia vacia devolvemos una lista vacia
            if (String.IsNullOrEmpty(reference))
                return new List<PositionAPI>();

            return await _vehicleService.GetVehiclePositions(reference, DateTime.Now.Date);
        }
    }
}
