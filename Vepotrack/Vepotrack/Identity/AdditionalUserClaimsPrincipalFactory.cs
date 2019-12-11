﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;

namespace Vepotrack.API.Identity
{
    public class AdditionalUserClaimsPrincipalFactory
          : UserClaimsPrincipalFactory<UserApp, UserRol>
    {
        public AdditionalUserClaimsPrincipalFactory(
            UserManager<UserApp> userManager,
            RoleManager<UserRol> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        public async override Task<ClaimsPrincipal> CreateAsync(UserApp user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;

            var claims = new List<Claim>();
            
            // Asignamos el 'claim' del rol 
            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "admin"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "user"));
            }

            identity.AddClaims(claims);
            return principal;
        }
    }
}
