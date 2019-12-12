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
            builder.Entity<Order>().Property(p => p.Created).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<Order>().Property(p => p.Status).IsRequired();
            builder.Entity<Order>().HasMany(p => p.OrderChanges).WithOne(p => p.Order).HasForeignKey(p => p.OrderId);
            builder.Entity<Order>().HasOne(p => p.User).WithMany(p => p.Orders).HasForeignKey(p => p.UserId);
            builder.Entity<Order>().HasOne(p => p.Vehicle).WithMany(p => p.Orders);

            builder.Entity<OrderChanges>().ToTable("OrderChanges");
            builder.Entity<OrderChanges>().HasKey(p => p.Id);
            builder.Entity<OrderChanges>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<OrderChanges>().Property(p => p.OrderId).IsRequired();
            builder.Entity<OrderChanges>().Property(p => p.ChangeId).IsRequired();
            builder.Entity<OrderChanges>().Property(p => p.ChangeDate).IsRequired();
            builder.Entity<OrderChanges>().Property(p => p.FieldChange).IsRequired();
            builder.Entity<OrderChanges>().Property(p => p.OldValue).IsRequired();
            builder.Entity<OrderChanges>().Property(p => p.UserChange).IsRequired();
        }

        /// <summary>
        /// Creamos los roles por defecto cheuqueando primero si existe
        /// </summary>
        /// <param name="roleManager"></param>
        internal static void SeedRoles ( RoleManager<UserRol> roleManager)
        {
            if (!roleManager.RoleExistsAsync(UserRol.AdminRol).Result)
            {
                UserRol role = new UserRol();
                role.Name = UserRol.AdminRol;
                roleManager.CreateAsync(role);
            }

            if (!roleManager.RoleExistsAsync(UserRol.VehicleRol).Result)
            {
                UserRol role = new UserRol();
                role.Name = UserRol.VehicleRol;
                roleManager.CreateAsync(role);
            }

            if (!roleManager.RoleExistsAsync(UserRol.RegularRol).Result)
            {
                UserRol role = new UserRol();
                role.Name = UserRol.RegularRol;
                roleManager.CreateAsync(role);
            }
        }

        /// <summary>
        /// Creamos los usuarios por defecto chequeando previamente si existen
        /// </summary>
        /// <param name="userManager"></param>
        internal static void SeedUser(UserManager<UserApp> userManager)
        {
            //Creamos el usuario admin por defecto
            if (userManager.FindByNameAsync("Admin").Result == null)
            {
                UserApp user = new UserApp();
                user.UserName = "Admin";
                IdentityResult result = userManager.CreateAsync (user, "Ad.123456").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user,UserRol.AdminRol).Wait();
                }
            }
        }

        internal static void SeedData(UserManager<UserApp> userManager,RoleManager<UserRol> roleManager)
        {
            SeedRoles(roleManager);
            SeedUser(userManager);
        }

    }
}
