using CommerceLab.Modules.Catalogo.Contracts.Estoques;
using CommerceLab.Modules.Venda.Domain;

namespace CommerceLab.Modules.Venda.Application.CadastrarPedido.Saga;

internal sealed class CadastrarPedidoContexto(Pedido pedido, IReadOnlyCollection<ItemBaixaEstoque> itensBaixa)
{
    public Pedido Pedido { get; } = pedido;

    public IReadOnlyCollection<ItemBaixaEstoque> ItensBaixa { get; } = itensBaixa;
}
