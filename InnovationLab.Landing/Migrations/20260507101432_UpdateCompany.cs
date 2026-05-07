using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnovationLab.Landing.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "About",
                schema: "landing",
                table: "Companies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsJobFair",
                schema: "landing",
                table: "Companies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMouSigned",
                schema: "landing",
                table: "Companies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfInterns",
                schema: "landing",
                table: "Companies",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfVacancies",
                schema: "landing",
                table: "Companies",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "OpeningUrls",
                schema: "landing",
                table: "Companies",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                schema: "landing",
                table: "Companies",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "About",
                schema: "landing",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "IsJobFair",
                schema: "landing",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "IsMouSigned",
                schema: "landing",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "NumberOfInterns",
                schema: "landing",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "NumberOfVacancies",
                schema: "landing",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "OpeningUrls",
                schema: "landing",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                schema: "landing",
                table: "Companies");
        }
    }
}
