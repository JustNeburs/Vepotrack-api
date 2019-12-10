using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;

namespace Vepotrack.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        #region Usuarios
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(Guid userId);
        Task<User> GetUser(String username);
        Task<User> AddUser(User user);
        Task<User> UpdateUser(User user);
        #endregion

        #region Permisos de usuario
        Task<IEnumerable<UserPermission>> GetUserPermission(Guid userId);
        Task SetUserPermission(IEnumerable<UserPermission> permissions);
        #endregion
    }
}
