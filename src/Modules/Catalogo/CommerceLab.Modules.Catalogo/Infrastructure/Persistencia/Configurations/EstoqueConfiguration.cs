using CommerceLab.Modules.Catalogo.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceLab.Modules.Catalogo.Infrastructure.Persistencia.Configurations
{
    internal sealed class EstoqueConfiguration : IEntityTypeConfiguration<Estoque>
    {
        public void Configure(EntityTypeBuilder<Estoque> builder)
        {
            builder.ToTable("estoques");

            builder.HasKey(estoque => estoque.Id).HasName("pk_estoques");

            builder.Property(estoque => estoque.Id).HasColumnName("id");
            builder.Property(estoque => estoque.ProdutoId).HasColumnName("produto_id");
            builder.Property(estoque => estoque.QuantidadeDisponivel).HasColumnName("quantidade_disponivel");
            builder.Property(estoque => estoque.QuantidadeReservada).HasColumnName("quantidade_reservada");
            builder.Property(estoque => estoque.AtualizadoEm).HasColumnName("atualizado_em");

            builder.HasIndex(estoque => estoque.ProdutoId).IsUnique().HasDatabaseName("ix_estoques_produto_id");

            builder.HasOne<Produto>()                       // sem navegação do lado Estoque
                   .WithOne()                               // sem navegação do lado Produto
                   .HasForeignKey<Estoque>(estoque => estoque.ProdutoId)
                   .HasConstraintName("fk_estoques_produtos_produto_id")
                   .IsRequired();
        }
    }
}
