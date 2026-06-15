using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Complete.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameChurchSupplementalAgreementColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "church_supplemental_agreement_signed",
                schema: "complete",
                table: "conversion_tasks_data",
                newName: "church_supplemental_agreement_signed_trust");

            migrationBuilder.RenameColumn(
                name: "church_supplemental_agreement_sent",
                schema: "complete",
                table: "conversion_tasks_data",
                newName: "church_supplemental_agreement_final_saved");

            migrationBuilder.RenameColumn(
                name: "church_supplemental_agreement_saved",
                schema: "complete",
                table: "conversion_tasks_data",
                newName: "church_supplemental_agreement_draft_saved");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "church_supplemental_agreement_signed_trust",
                schema: "complete",
                table: "conversion_tasks_data",
                newName: "church_supplemental_agreement_signed");

            migrationBuilder.RenameColumn(
                name: "church_supplemental_agreement_final_saved",
                schema: "complete",
                table: "conversion_tasks_data",
                newName: "church_supplemental_agreement_sent");

            migrationBuilder.RenameColumn(
                name: "church_supplemental_agreement_draft_saved",
                schema: "complete",
                table: "conversion_tasks_data",
                newName: "church_supplemental_agreement_saved");
        }
    }
}
