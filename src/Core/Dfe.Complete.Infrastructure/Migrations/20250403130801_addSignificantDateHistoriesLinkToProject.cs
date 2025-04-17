using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Complete.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addSignificantDateHistoriesLinkToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_significant_date_histories_project_id",
                schema: "complete",
                table: "significant_date_histories",
                column: "project_id");

            migrationBuilder.AddForeignKey(
                name: "FK_significant_date_histories_projects_project_id",
                schema: "complete",
                table: "significant_date_histories",
                column: "project_id",
                principalSchema: "complete",
                principalTable: "projects",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_significant_date_histories_projects_project_id",
                schema: "complete",
                table: "significant_date_histories");

            migrationBuilder.DropIndex(
                name: "IX_significant_date_histories_project_id",
                schema: "complete",
                table: "significant_date_histories");
        }
    }
}
