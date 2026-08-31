using CommerceLab.Modules.Catalogo.Application.Abstracoes;
using CommerceLab.Shared.Results;

namespace CommerceLab.Modules.Catalogo.Application.Produtos.ListarProdutos;

internal sealed class ListarProdutosHandler(IProdutoRepository produtos, IEstoqueRepository estoques)
{
    public async Task<Result<IReadOnlyList<ProdutoResumoResult>>> HandleAsync(
        ListarProdutosQuery consulta, CancellationToken cancellationToken)
    {
        var encontrados = await produtos.ListarAsync(consulta.Ativo, consulta.Termo, cancellationToken);

        var posicoes = await estoques.ObterPorProdutosAsync([.. encontrados.Select(produto => produto.Id)], cancellationToken);

        IReadOnlyList<ProdutoResumoResult> resumo =
        [
            .. encontrados.Select(produto => new ProdutoResumoResult(
                produto.Id,
                produto.Sku,
                produto.Nome,
                produto.Preco,
                produto.Ativo,
                posicoes.TryGetValue(produto.Id, out var estoque) ? estoque.QuantidadeDisponivel : 0))
        ];

        return Result<IReadOnlyList<ProdutoResumoResult>>.Ok(resumo);
    }
}
