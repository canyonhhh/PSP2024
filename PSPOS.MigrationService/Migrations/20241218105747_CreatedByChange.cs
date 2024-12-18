using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSPOS.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class CreatedByChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Reservations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Reservations",
                type: "text",
                nullable: true);
        }
    }
}
