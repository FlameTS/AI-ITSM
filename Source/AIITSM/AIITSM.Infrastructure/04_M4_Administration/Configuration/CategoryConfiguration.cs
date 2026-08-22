using AIITSM.Domain._04_M4_Administration.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIITSM.Infrastructure._04_M4_Administration.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.CategoryId);

        builder.Property(c => c.CategoryId)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.CategoryName)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(c => c.CategoryName)
            .IsUnique();
    }
}