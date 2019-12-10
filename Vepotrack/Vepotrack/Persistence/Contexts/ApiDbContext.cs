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
    public class ApiDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }

        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("Users");
            builder.Entity<User>().HasKey(p => p.Id);
            builder.Entity<User>().Property(p => p.Id).IsRequired();
            builder.Entity<User>().Property(p => p.Username).IsRequired().HasMaxLength(100);
            builder.Entity<User>().Property(p => p.Hash).IsRequired().HasMaxLength(1024);
            builder.Entity<User>().Property(p => p.BackHash).IsRequired();
            builder.Entity<User>().HasMany(p => p.UserPermissions).WithOne(p => p.User).HasForeignKey(p => p.UserId);

            //Inicializamos la BD con el usuario Admin, con password 123456
            Guid idAdmin = Guid.NewGuid();
            var userAdmin = new User
            {
                Id = idAdmin,
                Username = "Admin",
                BackHash = "_back",
                Hash = "07A9FD54218BC62F6D16FBED0E45064429B202FD7A3BCE0135C11C90812225F9AC0F965E6BBAF133C4A81EB6D480939E8E8A46D5DB620BF92E1A9FEEC7528F18",
                Enabled = true
            };

            builder.Entity<User>().HasData(
                userAdmin
                );

            builder.Entity<UserPermission>().ToTable("UserPermissions");
            builder.Entity<UserPermission>().HasKey(p => new { p.UserId, p.Category });
            builder.Entity<UserPermission>().Property(p => p.Permission).IsRequired();

            // Asignamos todos los permisos
            builder.Entity<UserPermission>().HasData(new UserPermission
            {
                UserId = idAdmin,
                Category = "*",
                Permission = Permission.All
            });
        }

    }
}
