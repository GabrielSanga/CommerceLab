using CommerceLab.Modules.Catalogo.Contracts.Estoques;
using CommerceLab.Shared.Results;
using CommerceLab.Shared.Sagas;

namespace CommerceLab.Modules.Venda.Application.CadastrarPedido.Saga;

internal sealed class BaixarEstoqueStep(IEstoqueService estoqueService) : ISagaStep<CadastrarPedidoContexto>
{
    public string Nome => "BaixarEstoque";

    public Task<Result> ExecutarAsync(CadastrarPedidoContexto contexto, CancellationToken cancellationToken)
    {
        return estoqueService.BaixarAsync(contexto.ItensBaixa, cancellationToken);
    }

    public Task CompensarAsync(CadastrarPedidoContexto contexto, CancellationToken cancellationToken)
    {
        var itensEstorno = contexto.ItensBaixa
            .Select(item => new ItemEstornoEstoque(item.IdProduto, item.Quantidade))
            .ToArray();

        return estoqueService.EstornarAsync(itensEstorno, cancellationToken);
    }
}
