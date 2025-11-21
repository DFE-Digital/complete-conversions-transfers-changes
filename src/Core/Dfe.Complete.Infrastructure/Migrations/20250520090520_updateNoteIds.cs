using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Complete.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateNoteIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_rails_7f2323ad43",
                schema: "complete",
                table: "notes");

            migrationBuilder.DropForeignKey(
                name: "fk_rails_99e097b079",
                schema: "complete",
                table: "notes");

            migrationBuilder.DropForeignKey(
                name: "fk_rails_bba1c6b145",
                schema: "complete",
                table: "projects");

            migrationBuilder.AlterColumn<Guid>(
                name: "regional_delivery_officer_id",
                schema: "complete",
                table: "projects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                schema: "complete",
                table: "notes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "project_id",
                schema: "complete",
                table: "notes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "body",
                schema: "complete",
                table: "notes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_rails_7f2323ad43",
                schema: "complete",
                table: "notes",
                column: "user_id",
                principalSchema: "complete",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_rails_99e097b079",
                schema: "complete",
                table: "notes",
                column: "project_id",
                principalSchema: "complete",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_rails_bba1c6b145",
                schema: "complete",
                table: "projects",
                column: "regional_delivery_officer_id",
                principalSchema: "complete",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_rails_7f2323ad43",
                schema: "complete",
                table: "notes");

            migrationBuilder.DropForeignKey(
                name: "fk_rails_99e097b079",
                schema: "complete",
                table: "notes");

            migrationBuilder.DropForeignKey(
                name: "fk_rails_bba1c6b145",
                schema: "complete",
                table: "projects");

            migrationBuilder.AlterColumn<Guid>(
                name: "regional_delivery_officer_id",
                schema: "complete",
                table: "projects",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                schema: "complete",
                table: "notes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "project_id",
                schema: "complete",
                table: "notes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "body",
                schema: "complete",
                table: "notes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "fk_rails_7f2323ad43",
                schema: "complete",
                table: "notes",
                column: "user_id",
                principalSchema: "complete",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_rails_99e097b079",
                schema: "complete",
                table: "notes",
                column: "project_id",
                principalSchema: "complete",
                principalTable: "projects",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_rails_bba1c6b145",
                schema: "complete",
                table: "projects",
                column: "regional_delivery_officer_id",
                principalSchema: "complete",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
