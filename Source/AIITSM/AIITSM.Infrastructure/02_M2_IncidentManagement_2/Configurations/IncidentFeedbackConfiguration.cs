
using AIITSM.Domain._02_M2_IncidentManagement_2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIITSM.Infrastructure._02_M2_IncidentManagement_2.Configurations
{
    public class IncidentFeedbackConfiguration
        : IEntityTypeConfiguration<IncidentFeedback>
    {
        public void Configure(EntityTypeBuilder<IncidentFeedback> builder)
        {
            builder.ToTable("IncidentFeedback");

            builder.HasKey(x => x.FeedbackId);

            builder.Property(x => x.FeedbackText)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("now()")
                .IsRequired();

            builder.HasIndex(x => new { x.IncidentId, x.UserId })
                .IsUnique();
        }
    }
}