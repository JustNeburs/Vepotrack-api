using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;
using Vepotrack.API.Models;

namespace Vepotrack.API.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Convierte un objeto de usuario de base de datos en un Usuario de API para 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserAPI ToUserAPI(this UserApp user)
        {
            if (user == null)
                return null;
            return new UserAPI()
            {
                Id = user.Id.ToString(),
                Username = user.UserName,
                LastLogin = user.LastLogin
            };
        }
    }
}
