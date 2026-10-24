using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommerceLab.Modules.Venda.Infrastructure.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicialVenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "venda");

            migrationBuilder.CreateTable(
                name: "pedidos",
                schema: "venda",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "text", nullable: false),
                    id_cliente = table.Column<Guid>(type: "uuid", nullable: false),
                    id_endereco = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pedidos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pedido_itens",
                schema: "venda",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_pedido = table.Column<Guid>(type: "uuid", nullable: false),
                    id_produto = table.Column<Guid>(type: "uuid", nullable: false),
                    nome_produto = table.Column<string>(type: "text", nullable: false),
                    sku = table.Column<string>(type: "text", nullable: false),
                    quantidade = table.Column<int>(type: "integer", nullable: false),
                    valor_unitario = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pedido_itens", x => x.id);
                    table.ForeignKey(
                        name: "fk_pedidos_items",
                        column: x => x.id_pedido,
                        principalSchema: "venda",
                        principalTable: "pedidos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pedido_itens_pedido_id",
                schema: "venda",
                table: "pedido_itens",
                column: "id_pedido");

            migrationBuilder.CreateIndex(
                name: "ix_pedidos_codigo",
                schema: "venda",
                table: "pedidos",
                column: "codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pedido_itens",
                schema: "venda");

            migrationBuilder.DropTable(
                name: "pedidos",
                schema: "venda");
        }
    }
}
