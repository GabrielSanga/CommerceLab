using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceLab.Modules.Cliente.Infrastructure.Persistencia.Configurations
{
    internal class ClienteConfiguration : IEntityTypeConfiguration<Domain.Cliente>
    {
        public void Configure(EntityTypeBuilder<Domain.Cliente> builder)
        {
            builder.ToTable("clientes");

            builder.HasKey(Cliente => Cliente.Id).HasName("pk_clientes");

            builder.Property(Cliente => Cliente.Id).HasColumnName("id");
            builder.Property(Cliente => Cliente.Nome).HasColumnName("nome").IsRequired();
            builder.Property(Cliente => Cliente.Email).HasColumnName("email").IsRequired();
            builder.Property(Cliente => Cliente.Documento).HasColumnName("documento").IsRequired();
            builder.Property(Cliente => Cliente.CriadoEm).HasColumnName("criado_em");

            builder.HasIndex(Cliente => Cliente.Email).IsUnique().HasDatabaseName("ix_clientes_email");
        }
    }
}
