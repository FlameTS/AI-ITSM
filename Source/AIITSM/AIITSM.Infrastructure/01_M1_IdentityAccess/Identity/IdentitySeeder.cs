using Microsoft.AspNetCore.Identity;

namespace AITSM.Infrastructure._01_M1_IdentityAccess.Identity
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
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

            string adminEmail = "admin@aitsm.com";
            string adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
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