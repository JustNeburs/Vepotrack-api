using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.Persistence.Contexts;
using Vepotrack.API.Repositories.Interfaces;

namespace Vepotrack.API.Repositories
{
    /// <summary>
    /// Repositorio para el acceso a datos de vehiculos
    /// </summary>
    public class VehicleRepository : BaseRepository, IVehicleRepository
    {
        public VehicleRepository(ApiDbContext context) : base(context)
        {
        }
    }
}
