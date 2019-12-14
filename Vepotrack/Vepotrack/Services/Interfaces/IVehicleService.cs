using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vepotrack.API.Models;

namespace Vepotrack.API.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleAPI>> GetList();
        Task<VehicleAPI> GetVehicle(string reference);
        Task<IActionResult> AddVehicle(VehicleAPI value);
        Task<IActionResult> UpdateVehicle(string reference, VehicleAPI value);
        Task<IActionResult> AddVehiclePosition(string reference, PositionAPI value);
        Task<IEnumerable<PositionAPI>> GetVehiclePositions(DateTime date);
    }
}
