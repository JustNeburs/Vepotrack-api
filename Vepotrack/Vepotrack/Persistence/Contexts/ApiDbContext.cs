using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;

namespace Vepotrack.API.Persistence.Contexts
{
    /// <summary>
    /// Contexto base del EF Core 
    /// </summary>
    public class ApiDbContext : IdentityDbContext<UserApp, UserRol, Guid>
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<OrderChanges> OrderChanges { get; set; }
        public DbSet<VehiclePosition> VehiclePositions { get; set; }

        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Order>().ToTable("Orders");
            builder.Entity<Order>().HasKey(p => p.Id);
            builder.Entity<Order>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<Order>().Property(p => p.Reference).IsRequired();
            builder.Entity<Order>().HasIndex(p => p.Reference).IsUnique();
            builder.Entity<Order>().Property(p => p.ReferenceUniqueAccess).IsRequired();
            builder.Entity<Order>().HasIndex(p => p.ReferenceUniqueAccess).IsUnique();
            builder.Entity<Order>().Property(p => p.Created).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<Order>().Property(p => p.Status).IsRequired();
            builder.Entity<Order>().HasMany(p => p.OrderChanges).WithOne(p => p.Order).HasForeignKey(p => p.OrderId);
            builder.Entity<Order>().HasOne(p => p.User).WithMany(p => p.Orders).HasForeignKey(p => p.UserId);
            builder.Entity<Order>().HasOne(p => p.Vehicle).WithMany(p => p.Orders).HasForeignKey(p => p.VehicleId);

            builder.Entity<OrderChanges>().ToTable("OrderChanges");
            builder.Entity<OrderChanges>().HasKey(p => p.Id);
            builder.Entity<OrderChanges>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<OrderChanges>().Property(p => p.OrderId).IsRequired();
            builder.Entity<OrderChanges>().Property(p => p.ChangeId).IsRequired();
            builder.Entity<OrderChanges>().Property(p => p.ChangeDate).IsRequired();
            builder.Entity<OrderChanges>().Property(p => p.FieldChange).IsRequired();
            builder.Entity<OrderChanges>().Property(p => p.OldValue).IsRequired();
            builder.Entity<OrderChanges>().Property(p => p.UserChange).IsRequired();

            builder.Entity<Vehicle>().ToTable("Vehicles");
            builder.Entity<Vehicle>().HasKey(p => p.Id);
            builder.Entity<Vehicle>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<Vehicle>().Property(p => p.Reference).IsRequired();
            builder.Entity<Vehicle>().HasIndex(p => p.Reference).IsUnique();
            builder.Entity<Vehicle>().Property(p => p.Name).IsRequired();
            builder.Entity<Vehicle>().HasOne(p => p.User).WithOne(p => p.Vehicle);

            builder.Entity<VehiclePosition>().ToTable("VehiclePositions");
            builder.Entity<VehiclePosition>().HasKey(p => p.Id);
            builder.Entity<VehiclePosition>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<VehiclePosition>().Property(p => p.VehicleId).IsRequired();
            builder.Entity<VehiclePosition>().Property(p => p.SetDate).IsRequired();
            builder.Entity<VehiclePosition>().Property(p => p.Latitude).IsRequired();
            builder.Entity<VehiclePosition>().Property(p => p.Longitude).IsRequired();
            builder.Entity<VehiclePosition>().Property(p => p.Precision).IsRequired();
            builder.Entity<VehiclePosition>().HasOne(p => p.Vehicle).WithMany();
        }

        /// <summary>
        /// Creamos los roles por defecto cheuqueando primero si existe
        /// </summary>
        /// <param name="roleManager"></param>
        public static void SeedRoles ( RoleManager<UserRol> roleManager)
        {
            // Creamos el rol de administrador si no existe
            if (!roleManager.RoleExistsAsync(UserRol.AdminRol).Result)
            {
                UserRol role = new UserRol
                {
                    Name = UserRol.AdminRol
                };
                roleManager.CreateAsync(role);
            }

            // Creamos el rol de Usuario de vehiculo si no existe
            if (!roleManager.RoleExistsAsync(UserRol.VehicleRol).Result)
            {
                UserRol role = new UserRol
                {
                    Name = UserRol.VehicleRol
                };
                roleManager.CreateAsync(role);
            }

            // Creamos el rol de usuario regular si no existe
            if (!roleManager.RoleExistsAsync(UserRol.RegularRol).Result)
            {
                UserRol role = new UserRol
                {
                    Name = UserRol.RegularRol
                };
                roleManager.CreateAsync(role);
            }
        }

        /// <summary>
        /// Creamos los usuarios por defecto chequeando previamente si existen
        /// </summary>
        /// <param name="userManager"></param>
        public static void SeedUser(UserManager<UserApp> userManager)
        {
            //Creamos el usuario admin por defecto
            if (userManager.FindByNameAsync("Admin").Result == null)
            {
                UserApp user = new UserApp();
                user.UserName = "Admin";
                user.Name = "Admin";
                IdentityResult result = userManager.CreateAsync (user, "Ad.123456").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user,UserRol.AdminRol).Wait();
                }
            }

            //Creamos un usuario de vehiculo por defecto
            if (userManager.FindByNameAsync("Vehicle01").Result == null)
            {
                UserApp user = new UserApp();
                user.UserName = "Vehicle01";
                user.Name = "Vehículo 01";
                IdentityResult result = userManager.CreateAsync(user, "Ve.123456").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, UserRol.VehicleRol).Wait();
                }
            }

            //Creamos un usuario regular por defecto
            if (userManager.FindByNameAsync("Regular01").Result == null)
            {
                UserApp user = new UserApp();
                user.UserName = "Regular01";
                user.Name = "Usuario Regular 01";
                IdentityResult result = userManager.CreateAsync(user, "Re.123456").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, UserRol.RegularRol).Wait();
                }
            }
        }

        public static void SeedData(UserManager<UserApp> userManager,RoleManager<UserRol> roleManager)
        {
            SeedRoles(roleManager);
            SeedUser(userManager);
        }

    }
}
