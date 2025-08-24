using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaintenanceExtApp.Migrations
{
    /// <inheritdoc />
    public partial class AddHostTypeToHos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HostType",
                table: "Hosts",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HostType",
                table: "Hosts");
        }
    }
}
