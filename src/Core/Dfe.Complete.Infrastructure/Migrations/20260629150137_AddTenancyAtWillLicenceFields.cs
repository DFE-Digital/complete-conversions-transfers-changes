using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Complete.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenancyAtWillLicenceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "tenancy_at_will_being_used",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "tenancy_at_will_cleared",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "tenancy_at_will_licence_to_occupy_being_used",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "tenancy_at_will_received",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tenancy_at_will_being_used",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "tenancy_at_will_cleared",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "tenancy_at_will_licence_to_occupy_being_used",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "tenancy_at_will_received",
                schema: "complete",
                table: "conversion_tasks_data");
        }
    }
}
