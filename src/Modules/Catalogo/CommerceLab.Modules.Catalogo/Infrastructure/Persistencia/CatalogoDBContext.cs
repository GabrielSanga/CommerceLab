using CommerceLab.Modules.Catalogo.Application.Abstracoes;
using CommerceLab.Modules.Catalogo.Domain;
using CommerceLab.Modules.Catalogo.Infrastructure.Persistencia.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CommerceLab.Modules.Catalogo.Infrastructure.Persistencia
{
    internal sealed class CatalogoDBContext(DbContextOptions<CatalogoDBContext> options) : DbContext(options), IUnitOfWork
    {
        public DbSet<Produto> Produtos => Set<Produto>();

        public DbSet<Estoque> Estoques => Set<Estoque>();

        public Task CommitAsync(CancellationToken cancellationToken) => SaveChangesAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("catalogo");

            modelBuilder.ApplyConfiguration(new ProdutoConfiguration());
            modelBuilder.ApplyConfiguration(new EstoqueConfiguration());
        }
    }
}
