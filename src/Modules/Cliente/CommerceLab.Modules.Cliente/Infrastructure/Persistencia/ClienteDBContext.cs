using CommerceLab.Modules.Cliente.Infrastructure.Persistencia.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CommerceLab.Modules.Cliente.Infrastructure.Persistencia
{
    internal sealed class ClienteDBContext(DbContextOptions<ClienteDBContext> options) : DbContext(options)
    {
        internal const string SCHEMA = "cliente";

        public DbSet<Domain.Cliente> Clientes => Set<Domain.Cliente>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SCHEMA);

            modelBuilder.ApplyConfiguration(new ClienteConfiguration());
        }
    }
}
