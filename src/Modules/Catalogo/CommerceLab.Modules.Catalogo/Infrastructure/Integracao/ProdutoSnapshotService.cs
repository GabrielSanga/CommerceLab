using CommerceLab.Modules.Catalogo.Application.Abstracoes;
using CommerceLab.Modules.Catalogo.Contracts.Produtos;

namespace CommerceLab.Modules.Catalogo.Infrastructure.Integracao;

internal sealed class ProdutoSnapshotService(IProdutoRepository produtos) : IProdutoSnapshotService
{
    public async Task<ProdutoSnapshot?> ObterPorIdAsync(Guid idProduto, CancellationToken cancellationToken)
    {
        var produto = await produtos.ObterPorIdAsync(idProduto, cancellationToken);

        return produto is null
            ? null
            : new ProdutoSnapshot(produto.Id, produto.Sku, produto.Nome, produto.Preco, produto.Ativo);
    }
}
