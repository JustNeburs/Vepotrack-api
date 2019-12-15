using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vepotrack.API.Models;
using Vepotrack.API.Services.Interfaces;

namespace Vepotrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IEnumerable<UserAPI>> Get()
        {
            return await _userService.GetAPIUsers();
        }

        [Authorize(Policy = "IsVehicle")]
        [HttpGet("{username}")]
        public async Task<UserAPI> Get(string username)
        {
            return await _userService.GetAPIUser(username);
        }

        // POST: api/User
        [Authorize(Policy = "IsAdmin")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserAPI value)
        {
            // Solo vamos a permitir crear usuarios regulares desde la API
            var ret = await _userService.CreateUser(value);
            if (ret == null)
                return BadRequest();
            return Ok(ret);
        }            

    }
}
