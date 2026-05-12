using HospitalManagementSystem.Core.HospitalAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagementSystem.Infrastructure.Data.Config;

public class HospitalConfiguration : IEntityTypeConfiguration<Hospital>
{
  public void Configure(EntityTypeBuilder<Hospital> builder)
  {
    builder.ToTable("Hospitals");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Name)
      .HasMaxLength(200)
      .IsRequired();

    builder.Property(x => x.Code)
      .HasMaxLength(50)
      .IsRequired();

    builder.Property(x => x.IsActive)
      .IsRequired();

    builder.HasIndex(x => x.Code)
      .IsUnique();
  }
}
