using CommerceLab.Modules.Venda.Application.Abstracoes;
using CommerceLab.Modules.Venda.Domain;

namespace CommerceLab.Modules.Venda.Infrastructure.Persistencia;

internal sealed class PedidoRepository(VendaDBContext contexto) : IPedidoRepository
{
    public Task AdicionarAsync(Pedido pedido, CancellationToken cancellationToken)
    {
        contexto.Pedidos.Add(pedido);

        return Task.CompletedTask;
    }
}
