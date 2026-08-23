using AITSM.Application._01_M1_IdentityAccess.Interfaces;
using AITSM.Infrastructure._01_M1_IdentityAccess.Identity;
using AIITSM.Web._01_M1_IdentityAccess.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AIITSM.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // -------------------------------------------------
            // MVC
            // -------------------------------------------------
            builder.Services.AddControllersWithViews();


            // -------------------------------------------------
            // Database Context
            // -------------------------------------------------
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString(
                        "DefaultConnection")));


            // -------------------------------------------------
            // ASP.NET Core Identity
            // -------------------------------------------------
            builder.Services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    // Prevent duplicate email addresses
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            // -------------------------------------------------
            // Authentication Cookie Configuration
            // -------------------------------------------------
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });


            // -------------------------------------------------
            // Current User Service
            // Allows other modules to access the logged-in user
            // without directly depending on HttpContext
            // -------------------------------------------------
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<
                ICurrentUserService,
                CurrentUserService>();


            // -------------------------------------------------
            // Build Application
            // -------------------------------------------------
            var app = builder.Build();


            // -------------------------------------------------
            // Seed Identity Roles + Initial Administrator
            // -------------------------------------------------
            using (var scope = app.Services.CreateScope())
            {
                var roleManager =
                    scope.ServiceProvider
                        .GetRequiredService<RoleManager<ApplicationRole>>();

                var userManager =
                    scope.ServiceProvider
                        .GetRequiredService<UserManager<ApplicationUser>>();

                await IdentitySeeder.SeedRolesAndAdminAsync(
                    roleManager,
                    userManager,
                    builder.Configuration);
            }


            // -------------------------------------------------
            // HTTP Request Pipeline
            // -------------------------------------------------
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");

                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();


            // -------------------------------------------------
            // Authentication & Authorization
            // IMPORTANT: Authentication must come first
            // -------------------------------------------------
            app.UseAuthentication();

            app.UseAuthorization();


            // -------------------------------------------------
            // Static Assets
            // -------------------------------------------------
            app.MapStaticAssets();


            // -------------------------------------------------
            // MVC Routing
            // -------------------------------------------------
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();


            // -------------------------------------------------
            // Run Application
            // -------------------------------------------------
            app.Run();
        }
    }
}