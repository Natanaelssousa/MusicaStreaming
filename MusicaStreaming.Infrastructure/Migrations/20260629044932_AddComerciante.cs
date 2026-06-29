using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicaStreaming.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddComerciante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comerciante",
                table: "Transacoes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "N/A");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comerciante",
                table: "Transacoes");
        }
    }
}
