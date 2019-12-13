using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;
using Vepotrack.API.Models;

namespace Vepotrack.API.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> Find(string reference, string referenceUniqueAccess = null);
        Task<bool> AddOrder(OrderDataAPI value);
        Task<bool> UpdateOrder(string reference, OrderDataAPI value);
        Task<bool> Assign(string referenceOrder, string referenceVehicle);
    }
}
