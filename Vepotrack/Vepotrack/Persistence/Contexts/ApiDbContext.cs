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
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
                        
            // Creamos los roles
            this.Roles.Add(new UserRol
            {
                Name = "User"
            });
            this.Roles.Add(new UserRol
            {
                Name = "Admin"
            });

            SaveChanges();

            //Inicializamos la BD con el usuario Admin, con password 123456
            Guid idAdmin = Guid.NewGuid();

            //var userStore = new UserStore<UserApp>(this);
            //var user = new UserApp
            //{
            //    UserName = "Admin",
            //    PasswordHash = userManager.PasswordHasher.HashPassword("123456"),
            //    LockoutEnabled = true,
            //};
            //userManager.Create(user);
            //userManager.AddToRole(user.Id, "Admin");
            //var userAdmin = new UserApp
            //{
            //    Id = idAdmin,
            //    UserName = "Admin",
            //    BackHash = "_back",
            //    PasswordHash = "07A9FD54218BC62F6D16FBED0E45064429B202FD7A3BCE0135C11C90812225F9AC0F965E6BBAF133C4A81EB6D480939E8E8A46D5DB620BF92E1A9FEEC7528F18"
            //};

           
        }

    }
}
