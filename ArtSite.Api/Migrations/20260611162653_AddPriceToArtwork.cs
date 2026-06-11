using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtSite.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceToArtwork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ArtWorks",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "ArtWorks");
        }
    }
}
