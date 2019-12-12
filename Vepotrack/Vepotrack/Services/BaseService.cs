using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;

namespace Vepotrack.API.Services
{
    public abstract class BaseService
    {
        /// <summary>
        /// Acceso al contexto
        /// </summary>
        protected readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Servicio base con 
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public BaseService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        /// <summary>
        /// Obtenemos si es Administrador
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsAdmin() => _httpContextAccessor.HttpContext.User.IsInRole("Admin");
        
        /// <summary>
        /// Obtenemos si es Vehiculo
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsVehicle() =>  _httpContextAccessor.HttpContext.User.IsInRole("Vehicle");
        
        /// <summary>
        /// Obtenemos si es usuario regular
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsRegularUser() => _httpContextAccessor.HttpContext.User.IsInRole("Regular");



    }
}
