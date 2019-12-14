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
        [HttpGet]
        public async Task<IEnumerable<VehicleAPI>> Get()
        {
            return await _vehicleService.GetList();
        }

        // GET: api/Vehicle/5
        [HttpGet("{reference}", Name = "Get")]
        public async Task<VehicleAPI> Get(string reference)
        {
            return await _vehicleService.GetVehicle(reference);
        }

        // POST: api/Vehicle
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] VehicleAPI value)
        {
            return await _vehicleService.AddVehicle(value);
        }

        // PUT: api/Vehicle/5
        [HttpPut("{reference}")]
        public async Task<IActionResult> Put(string reference, [FromBody] VehicleAPI value)
        {
            return await _vehicleService.UpdateVehicle(reference, value);
        }

        // POST: api/Vehicle
        [HttpPost("position/{reference}")]
        public async Task<IActionResult> SetVehiclePosition(string reference, [FromBody] PositionAPI value)
        {
            return await _vehicleService.AddVehiclePosition(reference, value);
        }
        // GET: api/Vehicle/5
        [HttpGet("today/{reference}", Name = "Get")]
        public async Task<IEnumerable<PositionAPI>> Today(string reference)
        {
            return await _vehicleService.GetVehiclePositions(DateTime.Now.Date);
        }
    }
}
