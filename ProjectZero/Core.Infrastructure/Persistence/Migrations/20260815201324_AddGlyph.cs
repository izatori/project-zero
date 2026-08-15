using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGlyph : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Glyphs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Character = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Romaji = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ImageFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StrokeAnimationFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsLearned = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Glyphs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GlyphTranslation",
                columns: table => new
                {
                    GlyphId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlyphTranslation");

            migrationBuilder.DropTable(
                name: "Glyphs");
        }
    }
}
