using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnovationLab.Landing.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                schema: "landing",
                table: "TeamMembers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "landing",
                table: "TeamMembers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Faculty",
                schema: "landing",
                table: "TeamMembers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                schema: "landing",
                table: "TeamMembers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                schema: "landing",
                table: "TeamMembers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TeamName",
                schema: "landing",
                table: "EventRegistrations",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RegistrationCollege",
                schema: "landing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegistrationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ContactEmail = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    RepresentativeName = table.Column<string>(type: "text", nullable: false),
                    RepresentativePhone = table.Column<string>(type: "text", nullable: false),
                    RepresentativeEmail = table.Column<string>(type: "text", nullable: false),
                    RepresentativeDesignation = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationCollege", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationCollege_EventRegistrations_RegistrationId",
                        column: x => x.RegistrationId,
                        principalSchema: "landing",
                        principalTable: "EventRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationCollege_RegistrationId",
                schema: "landing",
                table: "RegistrationCollege",
                column: "RegistrationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrationCollege",
                schema: "landing");

            migrationBuilder.DropColumn(
                name: "Faculty",
                schema: "landing",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "Gender",
                schema: "landing",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                schema: "landing",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "TeamName",
                schema: "landing",
                table: "EventRegistrations");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                schema: "landing",
                table: "TeamMembers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "landing",
                table: "TeamMembers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IsRegistrationOpen",
                schema: "landing",
                table: "Events",
                type: "integer",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }
    }
}
