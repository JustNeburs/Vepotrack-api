using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Vepotrack.API.DataModels;
using Vepotrack.API.Identity;
using Vepotrack.API.Persistence.Contexts;
using Vepotrack.API.Repositories;
using Vepotrack.API.Repositories.Interfaces;
using Vepotrack.API.Services;
using Vepotrack.API.Services.Interfaces;

namespace Vepotrack.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Añadimos autenticación por TOKEN JWT
            var key = Encoding.ASCII.GetBytes(Configuration.GetValue<string>("SecretKey"));

            // Añadimos autenticación por Token en el Bearer por defecto y posibilidad de autenticación por cookie
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<ApiDbContext>(options => {
                options.UseInMemoryDatabase("API-Memory");
            });
            // Añadimos la identidad
            services.AddIdentity<UserApp, UserRol>()
                .AddEntityFrameworkStores<ApiDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IUserClaimsPrincipalFactory<UserApp>, AdditionalUserClaimsPrincipalFactory>();

            services.AddAuthentication();
            // Añadimos los'policy' de acceso
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", policy =>
                {
                    policy.RequireRole(UserRol.AdminRol);
                });
                options.AddPolicy("IsVehicle", policy =>
                {
                    policy.RequireRole(UserRol.VehicleRol, UserRol.AdminRol);
                });
            });
            // Repositorios
            services.AddScoped<IUserRepository, UserRepository>();
            // Servicios
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IVehicleService, VehicleService>();

            // Singleton de acceso al contexto
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, UserManager<UserApp> userManager, RoleManager<UserRol> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
                       
            // Añadimos el log a los request
            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();
            app.UseAuthentication();

            // Inicializamos los datos de usuario y roles
            ApiDbContext.SeedData(userManager, roleManager);

            app.UseMvc();
        }
    }
}
