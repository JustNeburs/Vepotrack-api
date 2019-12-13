using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.Models;

namespace Vepotrack.API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderAPI> GetOrder(string reference);
        Task<OrderAPI> GetOrderUniqueReference(string uniqueReference);
        Task<bool> AddOrder(OrderDataAPI value);
        Task<bool> UpdateOrder(String reference, OrderDataAPI value);
        Task<bool> AssignOrderVehicle(string referenceOrder, string referenceVehicle);
    }
}
