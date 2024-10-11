using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FrituurAllRelationships.Migrations
{
    /// <inheritdoc />
    public partial class addconformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "Orders");
        }
    }
}
