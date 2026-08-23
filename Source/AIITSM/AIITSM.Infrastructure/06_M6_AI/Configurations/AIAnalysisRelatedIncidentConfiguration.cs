using AIITSM.Domain._06_M6_AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIITSM.Infrastructure._06_M6_AI.Configurations
{
    public class AIAnalysisRelatedIncidentConfiguration
        : IEntityTypeConfiguration<AIAnalysisRelatedIncident>
    {
        public void Configure(EntityTypeBuilder<AIAnalysisRelatedIncident> builder)
        {
            builder.ToTable("AIAnalysisRelatedIncident");

            builder.HasKey(x => x.AIAnalysisRelatedIncidentId);

            builder.Property(x => x.RelationshipType)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.SimilarityScore)
                .HasColumnType("decimal(5,2)")
                .IsRequired();
        }
    }
}