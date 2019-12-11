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

        public async Task<UserApp> AddUser(UserApp user)
        {
            var ret = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return ret.Entity;
        }

        public async Task<UserApp> GetUser(Guid userId)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<UserApp> GetUser(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<UserRol>> GetUserPermission(Guid userId)
        {
            return await _context.Roles.Join(_context.UserRoles.Where(x => x.UserId == userId), rol => rol.Id, rolUser => rolUser.RoleId, (rol, rolUser) => rol).ToListAsync();
        }

        public async Task<IEnumerable<UserApp>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task SetUserPermission(IEnumerable<UserRol> permissions)
        {
            throw new NotImplementedException();
        }

        public async Task<UserApp> UpdateUser(UserApp user)
        {
            var ret = _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return ret.Entity;
        }
    }
}
