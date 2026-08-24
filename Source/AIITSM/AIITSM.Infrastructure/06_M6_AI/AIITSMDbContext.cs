using AIITSM.Domain._03_M3_AgentWorkflow;
using AIITSM.Domain._02_M2_IncidentManagement;
using AIITSM.Domain._06_M6_AI;
using AIITSM.Domain._02_M2_IncidentManagement_2;
using Microsoft.EntityFrameworkCore;
using AIITSM.Domain._07_M7_Automation;


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

        // Added for M2 — Incident Management.
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Category> Categories { get; set; }

        //M2 Extension
        public DbSet<IncidentComment> IncidentComments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<IncidentAttachment> IncidentAttachments { get; set; }
        public DbSet<IncidentFeedback> IncidentFeedback { get; set; }

        //M3
        public DbSet<IncidentAssignment> IncidentAssignments { get; set; }

        //M7
        public DbSet<Escalation> Escalations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AIITSMDbContext).Assembly);
        }
    }
}
