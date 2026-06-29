using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicaStreaming.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoritoArtista : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoritosArtista",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArtistaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataFavoritacao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoritosArtista", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoritosArtista_UsuarioId",
                table: "FavoritosArtista",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoritosArtista_UsuarioId_ArtistaId",
                table: "FavoritosArtista",
                columns: new[] { "UsuarioId", "ArtistaId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoritosArtista");
        }
    }
}
