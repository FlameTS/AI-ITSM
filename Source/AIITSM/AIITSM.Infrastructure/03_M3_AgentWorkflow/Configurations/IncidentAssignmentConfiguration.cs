using AIITSM.Domain._03_M3_AgentWorkflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIITSM.Infrastructure._03_M3_AgentWorkflow.Configurations
{
    public class IncidentAssignmentConfiguration
        : IEntityTypeConfiguration<IncidentAssignment>
    {
        public void Configure(EntityTypeBuilder<IncidentAssignment> builder)
        {
            builder.ToTable("IncidentAssignments");

            builder.HasKey(x => x.AssignmentId);

            builder.Property(x => x.AssignedTo)
                .IsRequired();

            builder.Property(x => x.AssignedAt)
                .IsRequired();
        }
    }
}