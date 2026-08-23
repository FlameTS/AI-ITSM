using AIITSM.Domain._06_M6_AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIITSM.Infrastructure._06_M6_AI.Configurations
{
    public class AIAnalysisConfiguration : IEntityTypeConfiguration<AIAnalysis>
    {
        public void Configure(EntityTypeBuilder<AIAnalysis> builder)
        {
            builder.ToTable("AIAnalysis");

            builder.HasKey(x => x.AIAnalysisId);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.SuggestedCategory)
                .HasMaxLength(100);

            builder.Property(x => x.SuggestedPriority)
                .HasMaxLength(50);

            builder.Property(x => x.SuggestedResolution);

            builder.Property(x => x.ConfidenceScore)
                .HasColumnType("decimal(5,2)");

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();
        }
    }
}