using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagementSystem.Infrastructure.Data.Migrations;

  /// <inheritdoc />
  public partial class AddPoointment : Migration
  {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.CreateTable(
              name: "Appointments",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  PatientName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                  PatientEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                  PatientPhoneNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                  DoctorUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                  AppointmentDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                  Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                  Status = table.Column<int>(type: "int", nullable: false),
                  ReviewedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                  ReviewedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                  DecisionNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                  CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                  UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_Appointments", x => x.Id);
                  table.ForeignKey(
                      name: "FK_Appointments_Users_DoctorUserId",
                      column: x => x.DoctorUserId,
                      principalTable: "Users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
              });

          migrationBuilder.CreateIndex(
              name: "IX_Appointments_DoctorUserId_AppointmentDateTime",
              table: "Appointments",
              columns: new[] { "DoctorUserId", "AppointmentDateTime" });

          migrationBuilder.CreateIndex(
              name: "IX_Appointments_Status",
              table: "Appointments",
              column: "Status");
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.DropTable(
              name: "Appointments");
      }
  }
