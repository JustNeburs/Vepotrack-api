﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        /// Servicio de sesion
        /// </summary>
        private readonly SignInManager<UserApp> _signInManager;
        /// <summary>
        /// Manager de usuarios
        /// </summary>
        private readonly UserManager<UserApp> _userManager;
        /// <summary>
        /// Acceso al contexto
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;
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

        public UserService(IConfiguration configuration, 
            UserManager<UserApp> userManager, 
            SignInManager<UserApp> signInManager, 
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> Authenticate(LoginRequest loginInfo)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(loginInfo.Username, loginInfo.Password, isPersistent: false, lockoutOnFailure: false);

                if (!result.Succeeded)
                    return String.Empty;

                //var frontHash = _configuration.GetValue<string>("FrontHash") ?? String.Empty;

                //if (user.Hash != TextUtils.SHA512($"{frontHash}{loginInfo.Password}{user.BackHash}"))
                //    return String.Empty;

                var user = await _userManager.FindByNameAsync(loginInfo.Username);

                // Actualizmos el último login
                user.LastLogin = DateTime.Now;
                await _userRepository.UpdateUser(user);               

                return GetToken(user);
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Fallo en authenticación: {LoginInfo}", loginInfo);
            }

            return String.Empty;
        }

        public async Task<String> RefreshToken()
        {
            // Buscamos el usuario por nombre
            var user = await _userManager.FindByNameAsync(
               _httpContextAccessor.HttpContext.User.Identity.Name ??
               _httpContextAccessor.HttpContext.User.Claims.Where(c => c.Properties.ContainsKey("unique_name")).Select(c => c.Value).FirstOrDefault()
               );
            return GetToken(user);
        }

        public async Task<UserAPI> GetAPIUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            return user?.ToUserAPI();
        }

        public async Task<IEnumerable<UserAPI>> GetAPIUsers()
        {
            // Convertimos a UserAPI
            return _userManager.Users.Select(x => x.ToUserAPI()).ToList();
        }

        /// <summary>
        /// Funcion para generar el token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private String GetToken(UserApp user)
        {
            // Leemos el secret_key desde nuestro appseting
            var secretKey = _configuration.GetValue<string>("SecretKey");
            var key = Encoding.UTF8.GetBytes(secretKey);

            var utcNow = DateTime.UtcNow;

            // Creamos los claims (pertenencias, características) del usuario
            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString())
                };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = utcNow,
#if DEBUG
                    // Nuestro token va a durar un minuto, en Release durará 1 día
                    Expires = utcNow.AddMinutes(1),
#else
                // Nuestro token va a durar un día
                Expires = utcNow.AddDays(1),
#endif
                // Credenciales para generar el token usando nuestro secretykey y el algoritmo hash 256
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);
            // Devolvemos el token generado
            return tokenHandler.WriteToken(createdToken);
        }
        
    }
}
