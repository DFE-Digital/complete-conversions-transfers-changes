using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Complete.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AllConfirmStatutoryConsultationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "statutory_consultation_complete  ",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "statutory_consultation_not_applicable",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "statutory_consultation_complete  ",
                schema: "complete",
                table: "conversion_tasks_data");

            migrationBuilder.DropColumn(
                name: "statutory_consultation_not_applicable",
                schema: "complete",
                table: "conversion_tasks_data");
        }
    }
}
