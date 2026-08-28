using AIITSM.Application._02_M2_IncidentManagement;
using AIITSM.Application._02_M2_IncidentManagement_2.Attachments;
using AIITSM.Application._02_M2_IncidentManagement_2.Communication;
using AIITSM.Application._02_M2_IncidentManagement_2.Feedback;
using AIITSM.Application._02_M2_IncidentManagement_2.Notifications;
using AIITSM.Application._03_M3_AgentWorkflow;
using AIITSM.Application._04_M4_Administration.Interfaces;
using AIITSM.Application._06_M6_AI.Providers;
using AIITSM.Application._06_M6_AI.Services;
using AIITSM.Application._07_M7_Automation;
using AIITSM.Infrastructure._06_M6_AI.Services;
using AIITSM.Application.Reporting;
using AIITSM.Infrastructure._02_M2_IncidentManagement;
using AIITSM.Infrastructure._02_M2_IncidentManagement_2.Attachments;
using AIITSM.Infrastructure._02_M2_IncidentManagement_2.Communication;
using AIITSM.Infrastructure._02_M2_IncidentManagement_2.Feedback;
using AIITSM.Infrastructure._02_M2_IncidentManagement_2.Notifications;
using AIITSM.Infrastructure._03_M3_AgentWorkflow;
using AIITSM.Infrastructure._04_M4_Administration.Services;
using AIITSM.Infrastructure._05_M5_Reporting;
using AIITSM.Infrastructure._06_M6_AI;
using AIITSM.Infrastructure._06_M6_AI.Providers;

using AIITSM.Infrastructure._07_M7_Automation;
using AIITSM.Web._01_M1_IdentityAccess.Services;
using AIITSM.Web.Common;
using AITSM.Application._01_M1_IdentityAccess.Interfaces;
using AITSM.Infrastructure._01_M1_IdentityAccess.Identity;
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
            
            Env.TraversePath().Load();

            var geminiKey =
                Environment.GetEnvironmentVariable("GEMINI_API_KEY");

            Console.WriteLine(
                string.IsNullOrWhiteSpace(geminiKey)
                    ? "❌ GEMINI_API_KEY not loaded"
                    : "✅ GEMINI_API_KEY loaded");

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            
            // M2 / M3 / M6
            
            builder.Services.AddDbContext<AIITSMDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString(
                        "AIITSMDatabase")));

            
            // M1
            
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString(
                        "DefaultConnection")));

            
            // M1
            
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

            
            builder.Services.AddHttpContextAccessor();

            // M1 Identity current-user service
            builder.Services.AddScoped<
                IdentityCurrentUserService,
                CurrentUserService>();

            // Temporary M2 current-user
            
            builder.Services.AddScoped<
                IncidentCurrentUserService,
                DemoCurrentUserService>();

            
            // M6 - AI
            
            builder.Services.AddScoped<
                IAIAnalysisService,
                AIAnalysisService>();

            builder.Services.AddScoped<
                IAIProvider,
                GeminiProvider>();

            builder.Services.AddScoped<
                IChatService,
                ChatService > ();


            // M2 - Incident Management

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

            
            // M3 
            
            builder.Services.AddScoped<
                IIncidentAssignmentService,
                IncidentAssignmentService>();
            
            // M4 Administration
            builder.Services.AddScoped<
                IUserAdministrationService,
                UserAdministrationService>();

            builder.Services.AddScoped<
                ICategoryAdministrationService,
                CategoryAdministrationService>();

            
            // M5 - Reporting
            
            builder.Services.AddScoped<
                IReportingService,
                ReportingService>();

            //M7
            builder.Services.AddScoped<
                IAutomationService,
                AutomationService>();

            var app = builder.Build();

            

            
            // Seed Identity roles and administrator
            
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

            
            // HTTP Pipeline
            
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