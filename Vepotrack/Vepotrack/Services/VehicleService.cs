using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vepotrack.API.Models;
using Vepotrack.API.Services.Interfaces;

namespace Vepotrack.API.Services
{
    /// <summary>
    /// Servicio para las funciones a realizar con vehiculos
    /// </summary>
    public class VehicleService : BaseService, IVehicleService
    {
        public VehicleService(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        public async Task<IActionResult> AddVehicle(VehicleAPI value)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> AddVehiclePosition(string reference, PositionAPI value)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<VehicleAPI>> GetList()
        {
            throw new NotImplementedException();
        }

        public async Task<VehicleAPI> GetVehicle(string reference)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PositionAPI>> GetVehiclePositions(DateTime date)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> UpdateVehicle(string reference, VehicleAPI value)
        {
            throw new NotImplementedException();
        }
    }
}
