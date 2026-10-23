using CommerceLab.Modules.Venda.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceLab.Modules.Venda.Infrastructure.Persistencia.Configuration;

internal class PedidoItemConfiguration : IEntityTypeConfiguration<PedidoItem>
{
    public void Configure(EntityTypeBuilder<PedidoItem> builder)
    {
        builder.ToTable("pedido_itens");

        builder.HasKey(PedidoItem => PedidoItem.Id).HasName("pk_pedido_itens");

        builder.Property(PedidoItem => PedidoItem.Id).HasColumnName("id");
        builder.Property(PedidoItem => PedidoItem.IdPedido).HasColumnName("id_pedido").IsRequired();
        builder.Property(PedidoItem => PedidoItem.IdProduto).HasColumnName("id_produto").IsRequired();
        builder.Property(PedidoItem => PedidoItem.NomeProduto).HasColumnName("nome_produto").IsRequired();
        builder.Property(PedidoItem => PedidoItem.Sku).HasColumnName("sku").IsRequired();
        builder.Property(PedidoItem => PedidoItem.Quantidade).HasColumnName("quantidade").IsRequired();
        builder.Property(PedidoItem => PedidoItem.ValorUnitario).HasColumnName("valor_unitario").IsRequired();

        builder.HasIndex(PedidoItem => PedidoItem.IdPedido).HasDatabaseName("ix_pedido_itens_pedido_id");
    }
}
