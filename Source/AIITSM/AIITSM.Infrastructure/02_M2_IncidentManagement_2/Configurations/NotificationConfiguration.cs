using AIITSM.Domain._02_M2_IncidentManagement_2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIITSM.Infrastructure._02_M2_IncidentManagement_2.Configurations
{
    public class NotificationConfiguration
        : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(x => x.NotificationId);

            builder.Property(x => x.Message)
                .IsRequired();

            builder.Property(x => x.IsRead)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();
        }
    }
}