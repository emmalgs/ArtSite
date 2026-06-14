using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ArtSite.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddArtistInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArtistInfos",
                columns: table => new
                {
                    ArtistInfoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CV = table.Column<string>(type: "text", nullable: true),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    ArtistStatement = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistInfos", x => x.ArtistInfoId);
                });

            migrationBuilder.CreateTable(
                name: "ArtistInfoVersions",
                columns: table => new
                {
                    ArtistInfoVersionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArtistInfoId = table.Column<int>(type: "integer", nullable: false),
                    CV = table.Column<string>(type: "text", nullable: true),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    ArtistStatement = table.Column<string>(type: "text", nullable: true),
                    VersionCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChangeDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistInfoVersions", x => x.ArtistInfoVersionId);
                    table.ForeignKey(
                        name: "FK_ArtistInfoVersions_ArtistInfos_ArtistInfoId",
                        column: x => x.ArtistInfoId,
                        principalTable: "ArtistInfos",
                        principalColumn: "ArtistInfoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistInfoVersions_ArtistInfoId",
                table: "ArtistInfoVersions",
                column: "ArtistInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtistInfoVersions");

            migrationBuilder.DropTable(
                name: "ArtistInfos");
        }
    }
}
