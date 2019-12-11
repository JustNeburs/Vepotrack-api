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
        Task<IEnumerable<UserApp>> GetUsers();
        Task<UserApp> GetUser(Guid userId);
        Task<UserApp> GetUser(String username);
        Task<UserApp> AddUser(UserApp user);
        Task<UserApp> UpdateUser(UserApp user);
        #endregion

        #region Permisos de usuario
        Task<IEnumerable<UserRol>> GetUserPermission(Guid userId);
        Task SetUserPermission(IEnumerable<UserRol> permissions);
        #endregion
    }
}
