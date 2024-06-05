using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitVital.Migrations
{
    public partial class Update5FitVitalDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendas_Entrenadores_EntrenadorId",
                table: "Agendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendas_Usuarios_UsuarioId",
                table: "Agendas");

            migrationBuilder.DropIndex(
                name: "IX_Agendas_EntrenadorId",
                table: "Agendas");

            migrationBuilder.DropIndex(
                name: "IX_Agendas_UsuarioId",
                table: "Agendas");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Agendas_EntrenadorId",
                table: "Agendas",
                column: "EntrenadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendas_UsuarioId",
                table: "Agendas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendas_Entrenadores_EntrenadorId",
                table: "Agendas",
                column: "EntrenadorId",
                principalTable: "Entrenadores",
                principalColumn: "EntrenadorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendas_Usuarios_UsuarioId",
                table: "Agendas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
