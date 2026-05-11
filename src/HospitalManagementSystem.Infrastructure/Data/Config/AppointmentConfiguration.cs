using HospitalManagementSystem.Core.AppointmentAggregate;
using HospitalManagementSystem.Core.Model.User;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagementSystem.Infrastructure.Data.Config;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
  public void Configure(EntityTypeBuilder<Appointment> builder)
  {
    builder.ToTable("Appointments");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.PatientName)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(x => x.PatientEmail)
      .HasMaxLength(200)
      .IsRequired();

    builder.Property(x => x.PatientPhoneNumber)
      .HasMaxLength(25)
      .IsRequired();

    builder.Property(x => x.DoctorUserId)
      .HasMaxLength(450)
      .IsRequired();

    builder.Property(x => x.Reason)
      .HasMaxLength(1000)
      .IsRequired();

    builder.Property(x => x.Status)
      .HasConversion<int>()
      .IsRequired();

    builder.Property(x => x.DecisionNote)
      .HasMaxLength(1000);

    builder.HasIndex(x => new { x.DoctorUserId, x.AppointmentDateTime });
    builder.HasIndex(x => x.Status);

    builder.HasOne<ApplicationUser>()
      .WithMany()
      .HasForeignKey(x => x.DoctorUserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
