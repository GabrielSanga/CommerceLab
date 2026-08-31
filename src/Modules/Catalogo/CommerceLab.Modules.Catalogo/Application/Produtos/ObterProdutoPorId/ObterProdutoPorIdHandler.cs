using CommerceLab.Modules.Catalogo.Application.Abstracoes;
using CommerceLab.Shared.Results;

namespace CommerceLab.Modules.Catalogo.Application.Produtos.ObterProdutoPorId;

internal sealed class ObterProdutoPorIdHandler(IProdutoRepository produtos, IEstoqueRepository estoques)
{
    public async Task<Result<ProdutoDetalheResult>> HandleAsync(ObterProdutoPorIdQuery consulta, CancellationToken cancellationToken)
    {
        var produto = await produtos.ObterPorIdAsync(consulta.Id, cancellationToken);
        if (produto is null)
        {
            return Result<ProdutoDetalheResult>.Falha(Erro.NaoEncontrado("PRODUTO_NAO_ENCONTRADO", $"Não existe produto com o identificador '{consulta.Id}'."));
        }

        var estoque = await estoques.ObterPorProdutoIdAsync(produto.Id, cancellationToken);

        return Result<ProdutoDetalheResult>.Ok(new ProdutoDetalheResult(produto.Id,produto.Sku, produto.Nome, produto.Descricao, produto.Preco, produto.Ativo, produto.CriadoEm, produto.AtualizadoEm, estoque?.QuantidadeDisponivel ?? 0, estoque?.QuantidadeReservada ?? 0));
    }
}
