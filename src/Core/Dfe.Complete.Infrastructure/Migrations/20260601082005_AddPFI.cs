using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Complete.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPFI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "private_finance_initiative_cleared",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "private_finance_initiative_documents_sent_to_sopu_for_clearance",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "private_finance_initiative_draft_saved_in_trust_sharepoint_folder",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "private_finance_initiative_final_version_saved_in_sharepoint_folder",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "private_finance_initiative_master_funding_agreement_pfi_clauses_inserted",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "private_finance_initiative_not_applicable",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "private_finance_initiative_received",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "private_finance_initiative_signed_by_all_stakeholders",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "private_finance_initiative_supplementary_funding_agreement_pfi_clauses_inserted",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "private_finance_initiative_cleared",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "private_finance_initiative_documents_sent_to_sopu_for_clearance",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "private_finance_initiative_draft_saved_in_trust_sharepoint_folder",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "private_finance_initiative_final_version_saved_in_sharepoint_folder",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "private_finance_initiative_master_funding_agreement_pfi_clauses_inserted",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "private_finance_initiative_not_applicable",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "private_finance_initiative_received",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "private_finance_initiative_signed_by_all_stakeholders",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "private_finance_initiative_supplementary_funding_agreement_pfi_clauses_inserted",
                schema: "complete",
                table: "conversion_tasks_data");
        }
    }
}
