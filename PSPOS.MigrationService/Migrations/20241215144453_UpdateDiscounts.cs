using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSPOS.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDiscounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductOrServiceGroupId",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "ProductOrServiceId",
                table: "Discounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductOrServiceGroupId",
                table: "Discounts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductOrServiceId",
                table: "Discounts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
