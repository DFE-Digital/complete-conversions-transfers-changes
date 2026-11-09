using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Complete.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckAccuracyOfHigherNeedsConfirmPublishedIncorrectNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "check_accuracy_of_higher_needs_confirm_published_incorrect_number",
                schema: "complete",
                table: "conversion_tasks_data",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "check_accuracy_of_higher_needs_confirm_published_incorrect_number",
                schema: "complete",
                table: "conversion_tasks_data");
        }
    }
}
