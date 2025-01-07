using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.Complete.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProjectRegionTypeToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "region",
                schema: "complete",
                table: "projects",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 4000,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "region",
                schema: "complete",
                table: "projects",
                type: "int",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);
        }
    }
}
