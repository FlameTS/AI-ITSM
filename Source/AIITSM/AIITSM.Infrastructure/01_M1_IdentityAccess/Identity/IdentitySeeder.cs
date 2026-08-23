using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace AITSM.Infrastructure._01_M1_IdentityAccess.Identity
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            string[] roles =
            {
                "Employee",
                "HelpDeskAgent",
                "ITAdministrator",
                "ITManager"
            };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(
                        new ApplicationRole
                        {
                            Name = roleName
                        });
                }
            }

            var adminEmail =
                configuration["BootstrapAdmin:Email"];

            var adminPassword =
                configuration["BootstrapAdmin:Password"];

            var adminFullName =
                configuration["BootstrapAdmin:FullName"];

            if (string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(adminPassword))
            {
                return;
            }

            var adminUser =
                await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = adminFullName ?? "System Administrator",
                    IsActive = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    adminUser,
                    adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        adminUser,
                        "ITAdministrator");
                }
            }
        }
    }
}