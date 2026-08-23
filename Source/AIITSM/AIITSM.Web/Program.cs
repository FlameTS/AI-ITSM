using AIITSM.Application._02_M2_IncidentManagement;
using AIITSM.Application._02_M2_IncidentManagement_2.Attachments;
using AIITSM.Application._02_M2_IncidentManagement_2.Communication;
using AIITSM.Application._02_M2_IncidentManagement_2.Feedback;
using AIITSM.Application._02_M2_IncidentManagement_2.Notifications;
using AIITSM.Application._03_M3_AgentWorkflow;
using AIITSM.Application._06_M6_AI.Providers;
using AIITSM.Application._06_M6_AI.Services;
using AIITSM.Application.Common;
using AIITSM.Infrastructure._02_M2_IncidentManagement;
using AIITSM.Infrastructure._02_M2_IncidentManagement_2.Attachments;
using AIITSM.Infrastructure._02_M2_IncidentManagement_2.Communication;
using AIITSM.Infrastructure._02_M2_IncidentManagement_2.Feedback;
using AIITSM.Infrastructure._02_M2_IncidentManagement_2.Notifications;
using AIITSM.Infrastructure._03_M3_AgentWorkflow;
using AIITSM.Infrastructure._06_M6_AI;
using AIITSM.Infrastructure._06_M6_AI.Providers;
using AIITSM.Infrastructure._06_M6_AI.Services;
using AIITSM.Web._01_M1_IdentityAccess.Services;
using AIITSM.Web.Common;
using AITSM.Application._01_M1_IdentityAccess.Interfaces;
using AITSM.Infrastructure._01_M1_IdentityAccess.Identity;
using DotNetEnv;
using Google;
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
            // -------------------------------------------------
            // Environment configuration
            // -------------------------------------------------
            Env.TraversePath().Load();

            var geminiKey =
                Environment.GetEnvironmentVariable("GEMINI_API_KEY");

            Console.WriteLine(
                string.IsNullOrWhiteSpace(geminiKey)
                    ? "❌ GEMINI_API_KEY not loaded"
                    : "✅ GEMINI_API_KEY loaded");

            var builder = WebApplication.CreateBuilder(args);

            // -------------------------------------------------
            // MVC
            // -------------------------------------------------
            builder.Services.AddControllersWithViews();

            // -------------------------------------------------
            // Main project database
            // M2 / M3 / M6
            // -------------------------------------------------
            builder.Services.AddDbContext<AIITSMDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString(
                        "AIITSMDatabase")));

            // -------------------------------------------------
            // Identity database
            // M1
            // -------------------------------------------------
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString(
                        "DefaultConnection")));

            // -------------------------------------------------
            // ASP.NET Core Identity
            // M1
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
            // Current User / HTTP Context
            // -------------------------------------------------
            builder.Services.AddHttpContextAccessor();

            // M1 Identity current-user service
            builder.Services.AddScoped<
                IdentityCurrentUserService,
                CurrentUserService>();

            // Temporary M2 current-user service
            // TODO: Replace with proper integration when M2
            // consumes the M1 identity current-user implementation.
            builder.Services.AddScoped<
                IncidentCurrentUserService,
                DemoCurrentUserService>();

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

            // M2_2 - Communication
            builder.Services.AddScoped<
                IIncidentCommentService,
                IncidentCommentService>();

            // M2_2 - Notifications
            builder.Services.AddScoped<
                INotificationService,
                NotificationService>();

            // M2_2 - Attachments
            builder.Services.AddScoped<
                IIncidentAttachmentService,
                IncidentAttachmentService>();

            // M2_2 - Feedback
            builder.Services.AddScoped<
                IIncidentFeedbackService,
                IncidentFeedbackService>();

            // -------------------------------------------------
            // M3 - Agent Workflow
            // -------------------------------------------------
            builder.Services.AddScoped<
                IIncidentAssignmentService,
                IncidentAssignmentService>();

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