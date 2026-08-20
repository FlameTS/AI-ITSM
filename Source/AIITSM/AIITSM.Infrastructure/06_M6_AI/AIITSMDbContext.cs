using AIITSM.Domain._06_M6_AI;
using Microsoft.EntityFrameworkCore;

namespace AIITSM.Infrastructure._06_M6_AI
{
    public class AIITSMDbContext : DbContext
    {
        public AIITSMDbContext(DbContextOptions<AIITSMDbContext> options)
            : base(options)
        {
        }

        public DbSet<AIAnalysis> AIAnalyses { get; set; }
        public DbSet<AIAnalysisRelatedIncident> AIAnalysisRelatedIncidents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AIITSMDbContext).Assembly);
        }
    }
}