using AIITSM.Domain._02_M2_IncidentManagement_2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIITSM.Infrastructure._02_M2_IncidentManagement_2.Configurations
{
    public class IncidentAttachmentConfiguration
        : IEntityTypeConfiguration<IncidentAttachment>
    {
        public void Configure(EntityTypeBuilder<IncidentAttachment> builder)
        {
            builder.ToTable("IncidentAttachments");

            builder.HasKey(x => x.AttachmentId);

            builder.Property(x => x.FileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.StoredFileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.ContentType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.FileSize)
                .IsRequired();

            builder.Property(x => x.UploadedAt)
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();
        }
    }
}