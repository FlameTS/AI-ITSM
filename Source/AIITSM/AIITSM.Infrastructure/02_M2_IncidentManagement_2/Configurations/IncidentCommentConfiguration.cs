
using AIITSM.Domain._02_M2_IncidentManagement_2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIITSM.Infrastructure._02_M2_IncidentManagement_2.Configurations
{
    public class IncidentCommentConfiguration
        : IEntityTypeConfiguration<IncidentComment>
    {
        public void Configure(EntityTypeBuilder<IncidentComment> builder)
        {
            builder.ToTable("IncidentComments");

            builder.HasKey(x => x.CommentId);

            builder.Property(x => x.CommentText)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();
        }
    }
}