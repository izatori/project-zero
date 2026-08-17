using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGlyphTranslationFavourite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFavourite",
                table: "GlyphTranslation",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavourite",
                table: "GlyphTranslation");
        }
    }
}
