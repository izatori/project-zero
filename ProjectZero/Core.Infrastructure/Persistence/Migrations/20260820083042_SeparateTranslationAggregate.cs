using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeparateTranslationAggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GlyphId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JapaneseWriting = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RomajiWriting = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Translation = table.Column<string>(type: "nvarchar(1023)", maxLength: 1023, nullable: false),
                    ImageFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsLearned = table.Column<bool>(type: "bit", nullable: false),
                    IsFavourite = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translations_Glyphs_GlyphId",
                        column: x => x.GlyphId,
                        principalTable: "Glyphs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Translations_GlyphId",
                table: "Translations",
                column: "GlyphId");

            // Migrate existing translation data into the new Translations table.
            // Each existing row gets a fresh Guid identity while preserving all fields
            // (including learned/favourite flags and image file names).
            migrationBuilder.Sql(
                """
                INSERT INTO Translations (Id, GlyphId, JapaneseWriting, RomajiWriting, Translation, ImageFileName, IsLearned, IsFavourite)
                SELECT NEWID(), GlyphId, JapaneseWriting, RomajiWriting, Translation, ImageFileName, IsLearned, IsFavourite
                FROM GlyphTranslation;
                """);

            migrationBuilder.DropTable(
                name: "GlyphTranslation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.CreateTable(
                name: "GlyphTranslation",
                columns: table => new
                {
                    GlyphId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsFavourite = table.Column<bool>(type: "bit", nullable: false),
                    IsLearned = table.Column<bool>(type: "bit", nullable: false),
                    JapaneseWriting = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RomajiWriting = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Translation = table.Column<string>(type: "nvarchar(1023)", maxLength: 1023, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlyphTranslation", x => new { x.GlyphId, x.Id });
                    table.ForeignKey(
                        name: "FK_GlyphTranslation_Glyphs_GlyphId",
                        column: x => x.GlyphId,
                        principalTable: "Glyphs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
