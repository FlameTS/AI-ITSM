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
using Microsoft.EntityFrameworkCore;


namespace AIITSM.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.TraversePath().Load();

            var geminiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");

            Console.WriteLine(
                string.IsNullOrWhiteSpace(geminiKey)
                    ? "❌ GEMINI_API_KEY not loaded"
                    : "✅ GEMINI_API_KEY loaded");

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AIITSMDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("AIITSMDatabase")));

            builder.Services.AddScoped<IAIAnalysisService, AIAnalysisService>();

            builder.Services.AddScoped<IAIProvider, GeminiProvider>();

            // M2 — Incident Management.
            builder.Services.AddScoped<IIncidentService, IncidentService>();

            //M2_2
            builder.Services.AddScoped<
                IIncidentCommentService,
                IncidentCommentService>();

            builder.Services.AddScoped<
                INotificationService,
                NotificationService>();

            builder.Services.AddScoped<
                IIncidentAttachmentService,
                IncidentAttachmentService>();

            builder.Services.AddScoped<IIncidentFeedbackService, IncidentFeedbackService>();

            // TEMP until M1 (Identity/Access) ships real login — see
            // Web/Common/DemoCurrentUserService.cs for details.
            builder.Services.AddScoped<ICurrentUserService, DemoCurrentUserService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

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
