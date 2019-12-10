using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;
using Vepotrack.API.Models;
using Vepotrack.API.Repositories.Interfaces;
using Vepotrack.API.Services.Interfaces;
using Vepotrack.API.Utils;

namespace Vepotrack.API.Services
{
    public class UserService : IUserService
    {
        /// <summary>
        /// Variable donde se establecera el repositorio de usuarios
        /// </summary>
        private readonly IUserRepository _userRepository;
        /// <summary>
        /// Variable donde se establecera la configuración
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<UserService> _logger;

        public UserService(IConfiguration configuration, IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> Authenticate(LoginRequest loginInfo)
        {
            try
            {
                User user = await _userRepository.GetUser(loginInfo.Username);
                if (user == null)
                    return String.Empty;

                var frontHash = _configuration.GetValue<string>("FrontHash") ?? String.Empty;

                if (user.Hash != TextUtils.SHA512($"{frontHash}{loginInfo.Password}{user.BackHash}"))
                    return String.Empty;

                // Actualizmos el último login
                user.LastLogin = DateTime.Now;
                await _userRepository.UpdateUser(user);

                // Leemos el secret_key desde nuestro appseting
                var secretKey = _configuration.GetValue<string>("SecretKey");
                var key = Encoding.UTF8.GetBytes(secretKey);

                // Creamos los claims (pertenencias, características) del usuario
                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
#if DEBUG
                    // Nuestro token va a durar un minuto, en Release durará 1 día
                    Expires = DateTime.UtcNow.AddMinutes(1),
#else
                // Nuestro token va a durar un día
                Expires = DateTime.UtcNow.AddDays(1),
#endif
                    // Credenciales para generar el token usando nuestro secretykey y el algoritmo hash 256
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };


                var tokenHandler = new JwtSecurityTokenHandler();
                var createdToken = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(createdToken);
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Fallo en authenticación: {LoginInfo}", loginInfo);
            }

            return String.Empty;
        }

        public async Task<UserAPI> GetAPIUser(string username)
        {
            User user = await _userRepository.GetUser(username);
            return FromUser(user);
        }

        public async Task<IEnumerable<UserAPI>> GetAPIUsers()
        {
            // Obtenemos el listado de usuarios
            IEnumerable<User> lstUsers = await _userRepository.GetUsers();
            // Convertimos a UserAPI
            return lstUsers.Select(x => FromUser(x)).ToList();
        }

        protected static UserAPI FromUser(User user)
        {
            if (user == null)
                return null;
            return new UserAPI()
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                LastLogin = user.LastLogin
            };
        }
    }
}
