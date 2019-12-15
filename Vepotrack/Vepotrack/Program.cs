using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Vepotrack.API.Persistence.Contexts;

namespace Vepotrack.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Creamos el logger del programa 
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
             .WriteTo.Console(
                   LogEventLevel.Verbose,
                   "{NewLine}{Timestamp:HH:mm:ss} [{Level}] ({CorrelationToken}) {Message}{NewLine}{Exception}")
            .CreateLogger();

            try
            {
                Log.Information("Iniciamos la API");
                var host = CreateWebHostBuilder(args).Build();
                // Nos aseguramos que se generan las dependencias
                using (var scope = host.Services.CreateScope())
                using (var context = scope.ServiceProvider.GetService<ApiDbContext>())
                {
                    context.Database.EnsureCreated();
                }
                // Iniciamos 
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error inexperado");
            }
            finally
            {
                // Cerramos y guardamos el log
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
    }
}
