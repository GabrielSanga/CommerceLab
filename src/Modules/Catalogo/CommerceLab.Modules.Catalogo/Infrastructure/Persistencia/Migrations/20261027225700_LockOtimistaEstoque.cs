using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommerceLab.Modules.Catalogo.Infrastructure.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class LockOtimistaEstoque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Intencionalmente vazia: xmin é coluna de sistema do PostgreSQL,
            // presente em toda linha. Não há DDL a executar — esta migration
            // existe apenas para registrar a mudança no model snapshot.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
