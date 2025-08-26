using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Massora.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Init7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "VehicleFuelHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                table: "Drivers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFuelHistories_CompanyId",
                table: "VehicleFuelHistories",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_VehicleId",
                table: "Drivers",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_Vehicles_VehicleId",
                table: "Drivers",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleFuelHistories_Companies_CompanyId",
                table: "VehicleFuelHistories",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_Vehicles_VehicleId",
                table: "Drivers");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleFuelHistories_Companies_CompanyId",
                table: "VehicleFuelHistories");

            migrationBuilder.DropIndex(
                name: "IX_VehicleFuelHistories_CompanyId",
                table: "VehicleFuelHistories");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_VehicleId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "VehicleFuelHistories");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "Drivers");
        }
    }
}
