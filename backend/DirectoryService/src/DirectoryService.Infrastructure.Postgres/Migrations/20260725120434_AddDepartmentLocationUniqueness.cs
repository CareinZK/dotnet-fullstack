using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentLocationUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_department_locations_department_id",
                table: "department_locations");

            migrationBuilder.CreateIndex(
                name: "IX_department_locations_department_id_location_id",
                table: "department_locations",
                columns: new[] { "department_id", "location_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_department_locations_department_id_location_id",
                table: "department_locations");

            migrationBuilder.CreateIndex(
                name: "IX_department_locations_department_id",
                table: "department_locations",
                column: "department_id");
        }
    }
}
