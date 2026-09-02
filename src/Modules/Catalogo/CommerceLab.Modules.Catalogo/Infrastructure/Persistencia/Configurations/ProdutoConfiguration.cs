using CommerceLab.Modules.Catalogo.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceLab.Modules.Catalogo.Infrastructure.Persistencia.Configurations
{
    internal sealed class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("produtos");

            builder.HasKey(produto => produto.Id).HasName("pk_produtos");

            builder.Property(produto => produto.Id).HasColumnName("id");
            builder.Property(produto => produto.Nome).HasColumnName("nome").HasMaxLength(Produto.NOMETAMANHOMAXIMO).IsRequired();
            builder.Property(produto => produto.Descricao).HasColumnName("descricao").HasMaxLength(Produto.DESCRICAOTAMANHOMAXIMO);
            builder.Property(produto => produto.Sku).HasColumnName("sku").HasMaxLength(32).IsRequired();
            builder.Property(produto => produto.Preco).HasColumnName("preco").HasPrecision(18, 2);
            builder.Property(produto => produto.Ativo).HasColumnName("ativo").IsRequired();
            builder.Property(produto => produto.CriadoEm).HasColumnName("criado_em");
            builder.Property(produto => produto.AtualizadoEm).HasColumnName("atualizado_em");

            builder.HasIndex(produto => produto.Sku).IsUnique().HasDatabaseName("ix_produtos_sku");
        }
    }
}
