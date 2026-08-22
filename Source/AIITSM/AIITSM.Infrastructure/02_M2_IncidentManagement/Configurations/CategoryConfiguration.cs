using AIITSM.Domain._02_M2_IncidentManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIITSM.Infrastructure._02_M2_IncidentManagement.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(x => x.CategoryId);

            builder.Property(x => x.CategoryName)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
