using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Complete.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSignificantChangeProjectAndTaskTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "significant_change_project",
                schema: "complete",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: false),
                    completed_at = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: true),
                    state = table.Column<int>(type: "int", nullable: false),
                    prepare_id = table.Column<int>(type: "int", nullable: true),
                    assigned_to_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    assigned_at = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: true),
                    region = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    trust_ukprn = table.Column<int>(type: "int", nullable: false),
                    trust_name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    academy_urn = table.Column<int>(type: "int", nullable: false),
                    significant_date = table.Column<DateOnly>(type: "date", nullable: true),
                    team = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    sharepoint_folder_link = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_significant_change_project", x => x.id);
                    table.ForeignKey(
                        name: "FK_significant_change_project_users_assigned_to_user_id",
                        column: x => x.assigned_to_user_id,
                        principalSchema: "complete",
                        principalTable: "users",
                        principalColumn: "id");
                });

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
                        name: "FK_significant_change_project_tasks_data_significant_change_project_project_id",
                        column: x => x.project_id,
                        principalSchema: "complete",
                        principalTable: "significant_change_project",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_significant_change_project_assigned_to_user_id",
                schema: "complete",
                table: "significant_change_project",
                column: "assigned_to_user_id");

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

            migrationBuilder.DropTable(
                name: "significant_change_project",
                schema: "complete");
        }
    }
}
