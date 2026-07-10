using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemplateService.Domain.Departments;

namespace TemplateService.Infrastructure.Postgres.Configurations;

public sealed class DepartmentPositionsConfiguration : IEntityTypeConfiguration<DepartmentPositions>
{
    public void Configure(EntityTypeBuilder<DepartmentPositions> builder)
    {
        builder.ToTable("department_positions");

        builder.HasKey(dp => dp.Id);

        builder.Property(dp => dp.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(dp => dp.DepartmentId)
            .HasColumnName("department_id")
            .IsRequired();

        builder.Property(dp => dp.PositionIds)
            .HasColumnName("position_ids")
            .IsRequired()
            .HasColumnType("jsonb");

        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(dp => dp.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
