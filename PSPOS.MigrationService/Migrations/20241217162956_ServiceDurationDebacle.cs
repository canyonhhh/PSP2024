using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSPOS.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class ServiceDurationDebacle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Interval",
                table: "Services");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Interval",
                table: "Services",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
