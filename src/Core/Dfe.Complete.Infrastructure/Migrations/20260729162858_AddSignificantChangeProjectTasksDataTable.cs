using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Complete.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSignificantChangeProjectTasksDataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "significant_change_project_tasks_data",
                schema: "complete",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    project_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_significant_change_project_tasks_data", x => x.id);
                    table.ForeignKey(
                        name: "FK_significant_change_project_tasks_data_significant_change_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "complete",
                        principalTable: "significant_change_projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_significant_change_project_tasks_data_project_id",
                schema: "complete",
                table: "significant_change_project_tasks_data",
                column: "project_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "significant_change_project_tasks_data",
                schema: "complete");
        }
    }
}
