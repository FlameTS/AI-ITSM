using AIITSM.Application._06_M6_AI.Services;
using AIITSM.Infrastructure._06_M6_AI.Services;
using AIITSM.Infrastructure._06_M6_AI;
using Microsoft.EntityFrameworkCore;
using AIITSM.Application._06_M6_AI.Providers;
using AIITSM.Infrastructure._06_M6_AI.Providers;
using DotNetEnv;

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
