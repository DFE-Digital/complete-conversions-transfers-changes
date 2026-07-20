using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Complete.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChildrenCentre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "children_centre_academy_trust",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "children_centre_funding_pension_reviewed",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "children_centre_land_lease_shared_agreed",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "children_centre_legal_and_governance_reviewed",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "children_centre_local_authority",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "children_centre_not_applicable",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "children_centre_staffing_and_transfer_reviewed",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "children_centre_academy_trust",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "children_centre_funding_pension_reviewed",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "children_centre_land_lease_shared_agreed",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "children_centre_legal_and_governance_reviewed",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "children_centre_local_authority",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "children_centre_not_applicable",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "children_centre_staffing_and_transfer_reviewed",
                schema: "complete",
                table: "conversion_tasks_data");
        }
    }
}
