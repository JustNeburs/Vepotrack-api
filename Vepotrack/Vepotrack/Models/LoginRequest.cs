using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vepotrack.API.Models
{
    /// <summary>
    /// Clase para las peticiones de login
    /// </summary>
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
