using CommerceLab.Modules.Cliente.Application.Abstracoes;
using CommerceLab.Modules.Cliente.Infrastructure.Persistencia.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CommerceLab.Modules.Cliente.Infrastructure.Persistencia
{
    internal sealed class ClienteDBContext(DbContextOptions<ClienteDBContext> options) : DbContext(options), IUnitOfWork
    {
        internal const string SCHEMA = "cliente";

        public DbSet<Domain.Cliente> Clientes => Set<Domain.Cliente>();

        public DbSet<Domain.Endereco> Enderecos => Set<Domain.Endereco>();

        public Task CommitAsync(CancellationToken cancellationToken) => SaveChangesAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SCHEMA);

            modelBuilder.ApplyConfiguration(new ClienteConfiguration());
            modelBuilder.ApplyConfiguration(new EnderecoConfiguration());
        }
    }
}
