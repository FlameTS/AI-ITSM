using AITSM.Application._01_M1_IdentityAccess.Interfaces;
using AITSM.Infrastructure._01_M1_IdentityAccess.Identity;
using AIITSM.Web._01_M1_IdentityAccess.Services;

using AIITSM.Application._02_M2_IncidentManagement;
using AIITSM.Application._02_M2_IncidentManagement_2.Attachments;
using AIITSM.Application._02_M2_IncidentManagement_2.Communication;
using AIITSM.Application._02_M2_IncidentManagement_2.Feedback;
using AIITSM.Application._02_M2_IncidentManagement_2.Notifications;



using AIITSM.Application._06_M6_AI.Providers;
using AIITSM.Application._06_M6_AI.Services;

using AIITSM.Application.Common;

using AIITSM.Infrastructure._02_M2_IncidentManagement;
using AIITSM.Infrastructure._02_M2_IncidentManagement_2.Attachments;
using AIITSM.Infrastructure._02_M2_IncidentManagement_2.Communication;
using AIITSM.Infrastructure._02_M2_IncidentManagement_2.Feedback;
using AIITSM.Infrastructure._02_M2_IncidentManagement_2.Notifications;



using AIITSM.Infrastructure._06_M6_AI;
using AIITSM.Infrastructure._06_M6_AI.Providers;
using AIITSM.Infrastructure._06_M6_AI.Services;

using AIITSM.Web.Common;

using DotNetEnv;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// Avoid collision between the two ICurrentUserService interfaces.
using IdentityCurrentUserService =
    AITSM.Application._01_M1_IdentityAccess.Interfaces.ICurrentUserService;

using IncidentCurrentUserService =
    AIITSM.Application.Common.ICurrentUserService;

namespace AIITSM.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Load .env for Gemini configuration
            Env.TraversePath().Load();

            var geminiKey =
                Environment.GetEnvironmentVariable("GEMINI_API_KEY");

            Console.WriteLine(
                string.IsNullOrWhiteSpace(geminiKey)
                    ? "GEMINI_API_KEY not loaded"
                    : "GEMINI_API_KEY loaded");

            var builder = WebApplication.CreateBuilder(args);

            // MVC
            builder.Services.AddControllersWithViews();

            // -------------------------------------------------
            // Main project database
            // Incident Management / AI / Agent Workflow
            // -------------------------------------------------
            builder.Services.AddDbContext<AIITSMDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString(
                        "AIITSMDatabase")));

            // -------------------------------------------------
            // Identity database
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
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            // -------------------------------------------------
            // M1 - Identity current user service
            // -------------------------------------------------
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<
                IdentityCurrentUserService,
                CurrentUserService>();

            // -------------------------------------------------
            // M6 - AI
            // -------------------------------------------------
            builder.Services.AddScoped<
                IAIAnalysisService,
                AIAnalysisService>();

            builder.Services.AddScoped<
                IAIProvider,
                GeminiProvider>();

            // -------------------------------------------------
            // M2 - Incident Management
            // -------------------------------------------------
            builder.Services.AddScoped<
                IIncidentService,
                IncidentService>();

            builder.Services.AddScoped<
                IIncidentCommentService,
                IncidentCommentService>();

            builder.Services.AddScoped<
                INotificationService,
                NotificationService>();

            builder.Services.AddScoped<
                IIncidentAttachmentService,
                IncidentAttachmentService>();

            builder.Services.AddScoped<
                IIncidentFeedbackService,
                IncidentFeedbackService>();

            // -------------------------------------------------
            // M3 - Agent Workflow
            // -------------------------------------------------
           

            // -------------------------------------------------
            // Temporary M2 current-user implementation
            //
            // M2 currently expects int UserId while ASP.NET
            // Identity uses string IDs. Keep this temporarily
            // until Database v2 reconciles the user ID types.
            // -------------------------------------------------
            builder.Services.AddScoped<
                IncidentCurrentUserService,
                DemoCurrentUserService>();

            var app = builder.Build();

            // -------------------------------------------------
            // Seed Identity roles and administrator
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
            // HTTP Pipeline
            // -------------------------------------------------
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}