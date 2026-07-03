using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Complete.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConfirmSchoolBankDetailsTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "confirm_school_bank_details_sent",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "confirm_school_bank_details_submitted",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "confirm_school_bank_details_sent",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "confirm_school_bank_details_submitted",
                schema: "complete",
                table: "conversion_tasks_data");
        }
    }
}
