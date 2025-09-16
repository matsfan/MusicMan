using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicMan.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DiscogsReleaseId = table.Column<long>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    Artist = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: true),
                    Country = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    CoverImageUrl = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    Barcode = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AlbumId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AddedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionItems_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Albums_Barcode",
                table: "Albums",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_DiscogsReleaseId",
                table: "Albums",
                column: "DiscogsReleaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollectionItems_AlbumId",
                table: "CollectionItems",
                column: "AlbumId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionItems");

            migrationBuilder.DropTable(
                name: "Albums");
        }
    }
}
