using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ArtSite.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddShowsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shows",
                columns: table => new
                {
                    ShowId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    LocationId = table.Column<int>(type: "integer", nullable: true),
                    Dates = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ShowInfo = table.Column<string>(type: "text", nullable: true),
                    ShowType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ArtistStatement = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shows", x => x.ShowId);
                    table.ForeignKey(
                        name: "FK_Shows_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShowArtworks",
                columns: table => new
                {
                    ShowArtworkId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShowId = table.Column<int>(type: "integer", nullable: false),
                    ArtWorkId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowArtworks", x => x.ShowArtworkId);
                    table.ForeignKey(
                        name: "FK_ShowArtworks_ArtWorks_ArtWorkId",
                        column: x => x.ArtWorkId,
                        principalTable: "ArtWorks",
                        principalColumn: "ArtWorkId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowArtworks_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "ShowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShowImages",
                columns: table => new
                {
                    ShowImageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShowId = table.Column<int>(type: "integer", nullable: false),
                    BucketPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AltText = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ImageType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowImages", x => x.ShowImageId);
                    table.ForeignKey(
                        name: "FK_ShowImages_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "ShowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShowArtworks_ArtWorkId",
                table: "ShowArtworks",
                column: "ArtWorkId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowArtworks_ShowId_ArtWorkId",
                table: "ShowArtworks",
                columns: new[] { "ShowId", "ArtWorkId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShowImages_ShowId",
                table: "ShowImages",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_LocationId",
                table: "Shows",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShowArtworks");

            migrationBuilder.DropTable(
                name: "ShowImages");

            migrationBuilder.DropTable(
                name: "Shows");
        }
    }
}
