using CommerceLab.Modules.Venda.Application.CadastrarPedido;

namespace CommerceLab.Modules.Venda.Presentation.Pedido;

internal sealed record CadastrarPedidoItemRequest(Guid IdProduto, int Quantidade);

internal sealed record CadastrarPedidoRequest(Guid IdCliente, Guid IdEndereco, IReadOnlyCollection<CadastrarPedidoItemRequest> Itens)
{
    public CadastrarPedidoCommand ParaCommand()
    {
        var itens = (Itens ?? [])
            .Select(item => new CadastrarPedidoItemCommand(item.IdProduto, item.Quantidade))
            .ToList();

        return new CadastrarPedidoCommand(IdCliente, IdEndereco, itens);
    }
}
