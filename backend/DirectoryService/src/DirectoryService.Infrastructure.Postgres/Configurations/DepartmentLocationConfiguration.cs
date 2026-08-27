using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public sealed class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("department_locations");

        builder.HasKey(dl => dl.Id);

        builder.Property(dl => dl.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(dl => dl.DepartmentId)
            .HasColumnName("department_id")
            .IsRequired();

        builder.Property(dl => dl.LocationId)
            .HasColumnName("location_id")
            .IsRequired();

        builder.Property(dl => dl.IsPrimaryLocation)
            .HasColumnName("is_primary_location")
            .IsRequired();

        builder.HasIndex(dl => new { dl.DepartmentId, dl.LocationId })
            .IsUnique();

        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(dl => dl.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Location>()
            .WithMany()
            .HasForeignKey(dl => dl.LocationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
