using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;
using Vepotrack.API.Models;
using Vepotrack.API.Persistence.Contexts;
using Vepotrack.API.Repositories.Interfaces;

namespace Vepotrack.API.Repositories
{
    /// <summary>
    /// Repositorio para el acceso a datos de los pedidos
    /// </summary>
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        public OrderRepository(ApiDbContext context) : base(context)
        {
        }

        public async Task<bool> AddOrder(OrderDataAPI value)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateOrder(string reference, OrderDataAPI value)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Assign(string referenceOrder, string referenceVehicle)
        {
            throw new NotImplementedException();
        }

        public async Task<Order> Find(string reference, string referenceUniqueAccess = null)
        {
            throw new NotImplementedException();
        }

        
    }
}
