using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;
using Vepotrack.API.Models;
using Vepotrack.API.Persistence.Contexts;
using Vepotrack.API.Repositories.Interfaces;
using Vepotrack.API.Services;
using Vepotrack.API.Services.Interfaces;
using Vepotrack.Controllers;
using Xunit;
using static VepoTestXUnit.TestHelpers;

namespace VepoTestXUnit
{
    public class UserUnitTest
    {
        private readonly Mock<IUserRepository> _mockRepo;              

        public UserUnitTest()
        {
            _mockRepo = new Mock<IUserRepository>();
        }

        /// <summary>
        /// Test sencillo de chequeo de Usuario y Contexto
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestBaseMock_Ok()
        {
            var mockLogger = new Mock<ILogger<UserService>>();
            var dbcontext = TestHelpers.DbContext();
            var context = TestHelpers.Context(Guid.NewGuid(), "Admin", new string[] { UserRol.AdminRol });
            var userManager = new FakeUserManager(new UserStore<UserApp, UserRol, ApiDbContext, Guid>(dbcontext));
            var roleManager = new FakeRoleManager(new RoleStore<UserRol, ApiDbContext, Guid>(dbcontext));
                        
            var signManager = new FakeSignInManager(userManager, roleManager, TestHelpers.Options(), context.Object);
            ApiDbContext.SeedData(userManager, roleManager);

            var userservice = new UserService(GetIConfigurationRoot(), userManager , signManager, _mockRepo.Object, context.Object , mockLogger.Object);

            LoginController login = new LoginController(userservice);
            // Probamos a loguearnos, que como tenemos generado el falso usuario dara el Ok
            var loginResult = await login.Authenticate(new LoginRequest
            {
                Username = "Admin",
                Password = "Ad.123456"
            });

            // Assert
            Assert.IsType<OkObjectResult>(loginResult);

            UserController controller = new UserController(userservice);
            // Probamos a añadir el usuario, que como esta moqueado dará el Ok tambien
            var result = await controller.Post(new UserAPI
            {
                Username = "Admin",
                Password = "Ad.123456"
            });

            Assert.IsAssignableFrom<ObjectResult>(result);

            var objectResponse = result as ObjectResult; 

            Assert.Equal(200, objectResponse.StatusCode);            
        }


        [Fact]
        public async Task TestBaseMock_FailVehicleRol()
        {
            var mockLogger = new Mock<ILogger<UserService>>();
            var dbcontext = TestHelpers.DbContext();
            var context = Context(Guid.NewGuid(), "Vehicle", new string[] { UserRol.VehicleRol });
            var userManager = new FakeUserManager(new UserStore<UserApp, UserRol, ApiDbContext, Guid>(dbcontext));
            var roleManager = new FakeRoleManager(new RoleStore<UserRol, ApiDbContext, Guid>(dbcontext));
            var signManager = new FakeSignInManager(userManager, roleManager, TestHelpers.Options(), context.Object);

            var userservice = new UserService(GetIConfigurationRoot(), userManager, signManager, _mockRepo.Object, context.Object, mockLogger.Object);
            ApiDbContext.SeedData(userManager, roleManager);

            LoginController login = new LoginController(userservice);
            // Probamos a loguearnos, que como tenemos generado el falso usuario dara el Ok
            var loginResult = await login.Authenticate(new LoginRequest
            {
                Username = "Admin",
                Password = "Ad.123456"
            });

            // Assert
            Assert.IsType<OkObjectResult>(loginResult);

            UserController controller = new UserController(userservice);
            // Probamos a añadir el usuario, que como esta moqueado dará el Fallo por ser usurio vehiculo
            var result = await controller.Post(new UserAPI
            {
                Username = "Admin",
                Password = "Ad.123456"
            });

            Assert.IsAssignableFrom<StatusCodeResult>(result);

            var objectResponse = result as StatusCodeResult;

            Assert.Equal(400, objectResponse.StatusCode);
        }

        [Fact]
        public async Task TestBaseMock_FailRegularRol()
        {
            var mockLogger = new Mock<ILogger<UserService>>();
            var dbcontext = TestHelpers.DbContext();
            var context = Context(Guid.NewGuid(), "Vehicle", new string[] { UserRol.RegularRol });
            var userManager = new FakeUserManager(new UserStore<UserApp, UserRol, ApiDbContext, Guid>(dbcontext));
            var roleManager = new FakeRoleManager(new RoleStore<UserRol, ApiDbContext, Guid>(dbcontext));
            var signManager = new FakeSignInManager(userManager, roleManager, TestHelpers.Options(), context.Object);

            var userservice = new UserService(GetIConfigurationRoot(), userManager, signManager, _mockRepo.Object, context.Object, mockLogger.Object);
            ApiDbContext.SeedData(userManager, roleManager);

            LoginController login = new LoginController(userservice);
            // Probamos a loguearnos, que como tenemos generado el falso usuario dara el Ok
            var loginResult = await login.Authenticate(new LoginRequest
            {
                Username = "Admin",
                Password = "Ad.123456"
            });

            // Assert
            Assert.IsType<OkObjectResult>(loginResult);

            UserController controller = new UserController(userservice);
            // Probamos a añadir el usuario, que como esta moqueado dará el Fallo por ser usuario regular
            var result = await controller.Post(new UserAPI
            {
                Username = "Admin",
                Password = "Ad.123456"
            });

            Assert.IsAssignableFrom<StatusCodeResult>(result);

            var objectResponse = result as StatusCodeResult;

            Assert.Equal(400, objectResponse.StatusCode);
        }


        public static IConfigurationRoot GetIConfigurationRoot()
        {
            var myConfiguration = new Dictionary<string, string>
            {
                {"SecretKey", "asdwda121a4sd8w4das8d*w8d*asd@#"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();
        }       

        

    }
}
