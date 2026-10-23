using CommerceLab.Modules.Venda.Application.Abstracoes;
using CommerceLab.Modules.Venda.Domain;
using CommerceLab.Modules.Venda.Infrastructure.Persistencia.Configuration;
using Microsoft.EntityFrameworkCore;

namespace CommerceLab.Modules.Venda.Infrastructure.Persistencia;

internal sealed class VendaDBContext(DbContextOptions<VendaDBContext> options) : DbContext(options), IUnitOfWork
{
    internal const string SCHEMA = "venda";

    public DbSet<Pedido> Pedidos => Set<Pedido>();

    public DbSet<PedidoItem> PedidoItens => Set<PedidoItem>();

    public Task CommitAsync(CancellationToken cancellationToken) => SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SCHEMA);

        modelBuilder.ApplyConfiguration(new PedidoConfiguration());
        modelBuilder.ApplyConfiguration(new PedidoItemConfiguration());
    }
}
