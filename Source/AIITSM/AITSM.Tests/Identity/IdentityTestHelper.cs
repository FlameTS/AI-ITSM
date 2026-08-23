using AITSM.Infrastructure._01_M1_IdentityAccess.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AITSM.Tests.Identity
{
    public static class IdentityTestHelper
    {
        public static ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services
    .AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

            return services.BuildServiceProvider();
        }

        public static UserManager<ApplicationUser> GetUserManager(
            ServiceProvider serviceProvider)
        {
            return serviceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();
        }

        public static RoleManager<ApplicationRole> GetRoleManager(
            ServiceProvider serviceProvider)
        {
            return serviceProvider
                .GetRequiredService<RoleManager<ApplicationRole>>();
        }
    }
}