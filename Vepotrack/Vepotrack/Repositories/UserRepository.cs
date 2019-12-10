using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;
using Vepotrack.API.Persistence.Contexts;
using Vepotrack.API.Repositories.Interfaces;

namespace Vepotrack.API.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(ApiDbContext context) : base(context)
        {
        }

        public async Task<User> AddUser(User user)
        {
            var ret = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return ret.Entity;
        }

        public async Task<User> GetUser(Guid userId)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<User> GetUser(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<IEnumerable<UserPermission>> GetUserPermission(Guid userId)
        {
            return await _context.UserPermissions.ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task SetUserPermission(IEnumerable<UserPermission> permissions)
        {
            _context.UserPermissions.UpdateRange(permissions);
            await _context.SaveChangesAsync();            
        }

        public async Task<User> UpdateUser(User user)
        {
            var ret = _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return ret.Entity;
        }
    }
}
