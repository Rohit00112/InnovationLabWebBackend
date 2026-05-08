using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnovationLab.Landing.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsRegistrationOpen",
                schema: "landing",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxNumberOfTeams",
                schema: "landing",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRegistrationOpen",
                schema: "landing",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "MaxNumberOfTeams",
                schema: "landing",
                table: "Events");
        }
    }
}
