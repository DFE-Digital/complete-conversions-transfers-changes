using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Complete.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEstablishmentAndLaLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Use a staged migration for local_authority_id:
            // 1) add nullable column,
            // 2) backfill from existing establishment/local authority data,
            // 3) fail loudly if any rows cannot be resolved,
            // 4) enforce NOT NULL.
            // This avoids introducing Guid.Empty placeholder values that would violate
            // the FK or hide data quality issues.
            migrationBuilder.AddColumn<Guid>(
                name: "local_authority_id",
                schema: "complete",
                table: "significant_change_project",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE scp
                  SET scp.local_authority_id = la.id
                  FROM complete.significant_change_project scp
                  INNER JOIN complete.gias_establishments ge ON ge.urn = scp.academy_urn
                  INNER JOIN complete.local_authorities la ON la.code = ge.local_authority_code
                  WHERE scp.local_authority_id IS NULL;");

            migrationBuilder.Sql(
                @"IF EXISTS (
                        SELECT 1
                        FROM complete.significant_change_project
                        WHERE local_authority_id IS NULL
                  )
                  BEGIN
                      THROW 51000, 'Cannot enforce NOT NULL on significant_change_project.local_authority_id. Backfill did not resolve all rows.', 1;
                  END");

            migrationBuilder.AlterColumn<Guid>(
                name: "local_authority_id",
                schema: "complete",
                table: "significant_change_project",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_significant_change_project_academy_urn",
                schema: "complete",
                table: "significant_change_project",
                column: "academy_urn");

            migrationBuilder.CreateIndex(
                name: "IX_significant_change_project_local_authority_id",
                schema: "complete",
                table: "significant_change_project",
                column: "local_authority_id");

            migrationBuilder.AddForeignKey(
                name: "FK_significant_change_project_gias_establishments_academy_urn",
                schema: "complete",
                table: "significant_change_project",
                column: "academy_urn",
                principalSchema: "complete",
                principalTable: "gias_establishments",
                principalColumn: "urn");

            migrationBuilder.AddForeignKey(
                name: "FK_significant_change_project_local_authorities_local_authority_id",
                schema: "complete",
                table: "significant_change_project",
                column: "local_authority_id",
                principalSchema: "complete",
                principalTable: "local_authorities",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_significant_change_project_gias_establishments_academy_urn",
                schema: "complete",
                table: "significant_change_project");

            migrationBuilder.DropForeignKey(
                name: "FK_significant_change_project_local_authorities_local_authority_id",
                schema: "complete",
                table: "significant_change_project");

            migrationBuilder.DropIndex(
                name: "IX_significant_change_project_academy_urn",
                schema: "complete",
                table: "significant_change_project");

            migrationBuilder.DropIndex(
                name: "IX_significant_change_project_local_authority_id",
                schema: "complete",
                table: "significant_change_project");

            migrationBuilder.DropColumn(
                name: "local_authority_id",
                schema: "complete",
                table: "significant_change_project");
        }
    }
}
