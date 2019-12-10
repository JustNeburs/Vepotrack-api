using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Vepotrack.API.Models;
using Vepotrack.API.Services.Interfaces;

namespace Vepotrack.Controllers
{


    /// <summary>
    /// login controller class for authenticate users
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public LoginController(IConfiguration configuration,IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [HttpGet]
        [Route("ping")]
        public ActionResult EchoPing()
        {
            Thread.Sleep(800);
            return Ok(new { Success = true, Msg = DateTime.Now.ToString() });
        }

        [HttpPost]
        [Route("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Authenticate(LoginRequest login)
        {
            // Si no viene todo relleno indicamos que esta mal la petición
            if (String.IsNullOrEmpty(login?.Username) || String.IsNullOrEmpty(login.Password))
                return BadRequest();

            // Validamos el usuario contra el servicio de usuarios
            var token = await _userService.Authenticate(login);
            if (!String.IsNullOrEmpty(token))
                return Ok(token);
            // Si el token no viene relleno no esta autorizado
            return Unauthorized();
            
        }
    }
}