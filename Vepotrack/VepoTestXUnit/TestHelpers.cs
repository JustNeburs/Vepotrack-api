using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;
using Vepotrack.API.Identity;
using Vepotrack.API.Persistence.Contexts;

namespace VepoTestXUnit
{
    public static class TestHelpers
    {
        public static Mock<IHttpContextAccessor> Context(Guid userId, string username = "Admin",string[] roles = null)
        {
            //Mock IHttpContextAccessor
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();

            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            // Generamos la identidad en base al usuario, el id y los roles
            var fakeIdentity = new GenericIdentity(username);
            fakeIdentity.AddClaim(new System.Security.Claims.Claim(UserApp.IdentityIdClaim, userId.ToString()));
            var principal = new GenericPrincipal(fakeIdentity, roles);
            context.User = principal;

            return mockHttpContextAccessor;
        }

        public static ApiDbContext DbContext()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                 .Options;
            return new ApiDbContext(options);
        }

        public static IOptions<IdentityOptions> Options()
        {
            IdentityOptions identityOptions = new IdentityOptions {  };
            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(a => a.Value).Returns(identityOptions);
            return options.Object;
        }

        #region Users

        public class FakeSignInManager : SignInManager<UserApp>
        {
            public FakeSignInManager()
                    : this(new FakeUserManager(), new FakeRoleManager(), new Mock<IOptions<IdentityOptions>>().Object, new Mock<IHttpContextAccessor>().Object)
            { }

            public FakeSignInManager(UserManager<UserApp> userManager, RoleManager<UserRol> roleManager, IOptions<IdentityOptions> options, IHttpContextAccessor context)
                    : base(userManager,
                        context,
                         new AdditionalUserClaimsPrincipalFactory(userManager, roleManager, options),
                         options,
                         new Mock<ILogger<SignInManager<UserApp>>>().Object,
                         new Mock<IAuthenticationSchemeProvider>().Object)
            { }

            public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
            {
                return Task.FromResult(SignInResult.Success);
            }
        }

        public class FakeUserManager : UserManager<UserApp>
        {
            public bool TrueStore { get; set; } = true;

            public FakeUserManager() : this(new Mock<IUserStore<UserApp>>().Object) {
                TrueStore = false;
            }

            public FakeUserManager(IUserStore<UserApp> userStore)
                : base(userStore,
                  new Mock<IOptions<IdentityOptions>>().Object,
                  new Mock<IPasswordHasher<UserApp>>().Object,
                  new IUserValidator<UserApp>[0],
                  new IPasswordValidator<UserApp>[0],
                  new FakeNormalizer(),
                  new Mock<IdentityErrorDescriber>().Object,
                  new Mock<IServiceProvider>().Object,
                  new Mock<ILogger<UserManager<UserApp>>>().Object)
            {
            }

            public override Task<IdentityResult> CreateAsync(UserApp user, string password)
            {
                if (TrueStore)
                    return base.CreateAsync(user, password);
                return Task.FromResult(IdentityResult.Success);
            }

            public override Task<IdentityResult> AddToRoleAsync(UserApp user, string role)
            {
                if (TrueStore)
                    return base.AddToRoleAsync(user, role);
                return Task.FromResult(IdentityResult.Success);
            }

            public override Task<bool> IsInRoleAsync(UserApp user, string role)
            {
                if (TrueStore)
                    return base.IsInRoleAsync(user, role);
                return Task.FromResult(true);
            }

            public override Task<UserApp> FindByNameAsync(string userName)
            {
                if (TrueStore)
                    return base.FindByNameAsync(userName);
                return Task.FromResult(new UserApp
                {
                    UserName = userName,
                    Name = "Mock"
                });
            }
        }

        public class FakeRoleManager : RoleManager<UserRol>
        {
            public bool TrueStore { get; set; } = true;

            public FakeRoleManager():this(new Mock<IRoleStore<UserRol>>().Object)
            {
                TrueStore = false;
            }

            public FakeRoleManager(IRoleStore<UserRol> store):base(store,
                    new IRoleValidator<UserRol>[0],
                    new FakeNormalizer(),
                    new Mock<IdentityErrorDescriber>().Object,
                    new Mock<ILogger<RoleManager<UserRol>>>().Object)
            { }

            public override Task<UserRol> FindByNameAsync(string roleName)
            {
                if (TrueStore)
                {
                    if (roleName == null)
                        return null;
                    return base.FindByNameAsync(roleName);
                }
                return Task.FromResult(new UserRol
                {
                    Id = Guid.NewGuid(),
                    Name = roleName
                });
            }

            public override Task<bool> RoleExistsAsync(string roleName)
            {
                if (TrueStore)
                    return base.RoleExistsAsync(roleName);
                return Task.FromResult(true);
            }

            public override Task<IdentityResult> CreateAsync(UserRol role)
            {
                if (TrueStore)
                    return base.CreateAsync(role);
                return Task.FromResult(IdentityResult.Success);
            }
        }

        public class FakeNormalizer : ILookupNormalizer
        {
            public string Normalize(string key)
            {
                return key;
            }
        }



        #endregion
    }
}
