using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.Models;

namespace Vepotrack.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserAPI>> GetAPIUsers();
        Task<UserAPI> GetAPIUser(String username);
        Task<String> Authenticate(LoginRequest loginInfo);
    }
}
