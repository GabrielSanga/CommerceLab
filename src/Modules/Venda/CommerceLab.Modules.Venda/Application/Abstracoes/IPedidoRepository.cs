using CommerceLab.Modules.Venda.Domain;

namespace CommerceLab.Modules.Venda.Application.Abstracoes;

internal interface IPedidoRepository
{

    public Task AdicionarAsync(Pedido pedido, CancellationToken cancellationToken);

}
