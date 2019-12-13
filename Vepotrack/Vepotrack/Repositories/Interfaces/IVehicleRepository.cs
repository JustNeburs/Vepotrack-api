using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;

namespace Vepotrack.API.Repositories.Interfaces
{
    public interface IVehicleRepository
    {
        Task<Vehicle> Find(string referenceVehicle);
        Task<VehiclePosition> GetLastPosition(Guid? vehicleId);
    }
}
