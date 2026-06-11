using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtSite.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStateCountryToLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Locations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Locations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Locations");
        }
    }
}
