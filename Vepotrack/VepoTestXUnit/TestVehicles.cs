using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;
using Vepotrack.API.Models;
using Vepotrack.API.Persistence.Contexts;
using Vepotrack.API.Repositories;
using Vepotrack.API.Services;
using Vepotrack.Controllers;
using Xunit;

namespace VepoTestXUnit
{
    public class TestVehicles
    {
        public TestVehicles()
        {
        }


        [Fact]
        public async Task TestVehicleAdd_OkAdminRol()
        {
            // Generamos un contexto http falso para no tener que hacer login en cada llamada
            var context = TestHelpers.Context(Guid.NewGuid(), "Admin", new string[] { UserRol.AdminRol });
            var controller = GetController(context.Object);

            // Testeamos la operación
            var result = await controller.Post(new Vepotrack.API.Models.VehicleDataAPI
            {
                Reference ="Vehicle01_Test",
                Name = "Vehiculo Nombre Test",
                Plate = "234234DFD"
            });

            // Nos aseguramos que es de tipo StatusCode
            Assert.IsAssignableFrom<StatusCodeResult>(result);

            var status = result as StatusCodeResult;
            // Nos aseguramos que es un 200
            Assert.Equal(200, status.StatusCode);
        }

        [Fact]
        public async Task TestVehicleAdd_FailVehicleRol()
        {
            // Generamos un contexto http falso para no tener que hacer login en cada llamada
            var context = TestHelpers.Context(Guid.NewGuid(), "Vehicle", new string[] { UserRol.VehicleRol });
            var controller = GetController(context.Object);

            // Testeamos la operación
            var result = await controller.Post(new Vepotrack.API.Models.VehicleDataAPI
            {
                Reference = "Vehicle01_Test",
                Name = "Vehiculo Nombre Test",
                Plate = "234234DFD"
            });

            // Nos aseguramos que es de tipo StatusCode
            Assert.IsAssignableFrom<StatusCodeResult>(result);

            var status = result as StatusCodeResult;
            // Nos aseguramos que es un 400
            Assert.Equal(400, status.StatusCode);
        }

        [Fact]
        public async Task TestVehicleAdd_FailRegularRol()
        {
            // Generamos un contexto http falso para no tener que hacer login en cada llamada
            var context = TestHelpers.Context(Guid.NewGuid(), "Regular", new string[] { UserRol.RegularRol });
            var controller = GetController(context.Object);

            // Testeamos la operación
            var result = await controller.Post(new Vepotrack.API.Models.VehicleDataAPI
            {
                Reference = "Vehicle01_Test",
                Name = "Vehiculo Nombre Test",
                Plate = "234234DFD"
            });

            // Nos aseguramos que es de tipo StatusCode
            Assert.IsAssignableFrom<StatusCodeResult>(result);

            var status = result as StatusCodeResult;
            // Nos aseguramos que es un 400
            Assert.Equal(400, status.StatusCode);
        }

        [Theory]
        [InlineData("Vehicle01", true)]
        [InlineData("Vehicle02", false)]
        public async Task TestVehicleGet_OkAdminRol(String vehicleReference, bool found)
        {
            // Generamos un contexto http falso para no tener que hacer login en cada llamada
            var context = TestHelpers.Context(Guid.NewGuid(), "Admin", new string[] { UserRol.AdminRol });
            var controller = GetController(context.Object);

            await controller.Post(new Vepotrack.API.Models.VehicleDataAPI
            {
                Reference = "Vehicle01",
                Name = "Vehiculo Nombre Test",
                Plate = "234234DFD"
            });

            // Testeamos la operación
            var result = await controller.Get(vehicleReference);

            // Vemos si tenemos que encontrar resultado o no
            Assert.Equal(found, result != null);
        }


        [Theory]
        [InlineData("Vehicle01", false)]
        [InlineData("Vehicle02", false)]
        public async Task TestVehicleGet_FailVehicleRol(String vehicleReference, bool found)
        {
            // Generamos un contexto http falso para no tener que hacer login en cada llamada
            var context = TestHelpers.Context(Guid.NewGuid(), "Admin", new string[] { UserRol.VehicleRol });
            var controller = GetController(context.Object);

            await controller.Post(new Vepotrack.API.Models.VehicleDataAPI
            {
                Reference = "Vehicle01",
                Name = "Vehiculo Nombre Test",
                Plate = "234234DFD"
            });

            // Testeamos la operación
            var result = await controller.Get(vehicleReference);

            // Vemos si tenemos que encontrar resultado o no
            Assert.Equal(found, result != null);
        }

        /// <summary>
        /// Función para generar el controlador necesario
        /// </summary>
        /// <param name="contextObj"></param>
        /// <returns></returns>
        protected VehicleController GetController(IHttpContextAccessor contextObj)
        {
            // Generamos la BD de contexto
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                  .UseInMemoryDatabase(Guid.NewGuid().ToString())
                  .Options;
            var repoContext = new ApiDbContext(options);
            
            // Generamos los repositorios necesarios
            var userRepo = new UserRepository(repoContext, new Mock<ILogger<UserRepository>>().Object);
            var orderRepo = new OrderRepository(repoContext, new Mock<ILogger<OrderRepository>>().Object);
            var vehicleRepo = new VehicleRepository(repoContext, new Mock<ILogger<VehicleRepository>>().Object);
            // Pasamos a null la factoria de servicios porque ahora no se va a probar este entorno
            var notifyService = new NotifyService(contextObj, null, new Mock<ILogger<NotifyService>>().Object);
            // Generamos el servicio de vehiculos
            var service = new VehicleService(contextObj, userRepo, orderRepo, vehicleRepo, notifyService, new Mock<ILogger<VehicleService>>().Object);
            // Controlador a testear 
            return new VehicleController(service);
        }
    }
}
