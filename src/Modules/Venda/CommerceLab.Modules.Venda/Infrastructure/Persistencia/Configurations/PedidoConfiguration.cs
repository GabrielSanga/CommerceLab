using CommerceLab.Modules.Venda.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceLab.Modules.Venda.Infrastructure.Persistencia.Configuration
{
    internal class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("pedidos");

            builder.HasKey(Pedido => Pedido.Id).HasName("pk_pedidos");

            builder.Property(Pedido => Pedido.Id).HasColumnName("id");
            builder.Property(Pedido => Pedido.Codigo).HasColumnName("codigo");
            builder.Property(Pedido => Pedido.Status).HasColumnName("status");
            builder.Property(Pedido => Pedido.IdCliente).HasColumnName("id_cliente").IsRequired();
            builder.Property(Pedido => Pedido.IdEndereco).HasColumnName("id_endereco").IsRequired();
            builder.Property(Pedido => Pedido.DataCriacao).HasColumnName("data_criacao");

            builder.HasIndex(Pedido => Pedido.Codigo).IsUnique().HasDatabaseName("ix_pedidos_codigo");

            builder.HasMany(Pedido => Pedido.Itens)
                .WithOne()
                .HasForeignKey(PedidoItem => PedidoItem.IdPedido)
                .HasConstraintName("fk_pedidos_items")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
