using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceLab.Modules.Cliente.Infrastructure.Persistencia.Configurations;

internal class EnderecoConfiguration : IEntityTypeConfiguration<Domain.Endereco>
{
    public void Configure(EntityTypeBuilder<Domain.Endereco> builder)
    {
        builder.ToTable("enderecos");

        builder.HasKey(Endereco => Endereco.Id).HasName("pk_enderecos");

        builder.Property(Endereco => Endereco.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(Endereco => Endereco.IdCliente).HasColumnName("cliente_id").IsRequired();
        builder.Property(Endereco => Endereco.Logradouro).HasColumnName("logradouro").IsRequired();
        builder.Property(Endereco => Endereco.Numero).HasColumnName("numero").IsRequired();
        builder.Property(Endereco => Endereco.Cidade).HasColumnName("cidade").IsRequired();
        builder.Property(Endereco => Endereco.Estado).HasColumnName("estado").IsRequired();
        builder.Property(Endereco => Endereco.Cep).HasColumnName("cep").IsRequired();
        builder.Property(Endereco => Endereco.CriadoEm).HasColumnName("criado_em");

        builder.HasIndex(Endereco => Endereco.IdCliente).HasDatabaseName("ix_enderecos_cliente_id");
    }

}