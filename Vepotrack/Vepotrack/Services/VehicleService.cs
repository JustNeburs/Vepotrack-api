using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    }
}
