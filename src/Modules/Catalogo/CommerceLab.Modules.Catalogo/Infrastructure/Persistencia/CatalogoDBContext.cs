using CommerceLab.Modules.Catalogo.Application.Abstracoes;
using CommerceLab.Modules.Catalogo.Domain;
using CommerceLab.Modules.Catalogo.Infrastructure.Persistencia.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CommerceLab.Modules.Catalogo.Infrastructure.Persistencia
{
    internal sealed class CatalogoDBContext(DbContextOptions<CatalogoDBContext> options) : DbContext(options), IUnitOfWork
    {
        /// <summary>Schema do módulo. O histórico de migrations mora aqui também, para o Catalogo evoluir o banco sem coordenar com outros módulos.</summary>
        internal const string SCHEMA = "catalogo";

        public DbSet<Produto> Produtos => Set<Produto>();

        public DbSet<Estoque> Estoques => Set<Estoque>();

        public Task CommitAsync(CancellationToken cancellationToken) => SaveChangesAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SCHEMA);

            modelBuilder.ApplyConfiguration(new ProdutoConfiguration());
            modelBuilder.ApplyConfiguration(new EstoqueConfiguration());
        }
    }
}
