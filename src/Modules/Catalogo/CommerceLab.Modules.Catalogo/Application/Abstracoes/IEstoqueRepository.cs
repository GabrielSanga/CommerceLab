using CommerceLab.Modules.Catalogo.Domain;

namespace CommerceLab.Modules.Catalogo.Application.Abstracoes;

internal interface IEstoqueRepository
{
    Task AdicionarAsync(Estoque estoque, CancellationToken cancellationToken);

    Task<Estoque?> ObterPorProdutoIdAsync(Guid produtoId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, Estoque>> ObterPorProdutosAsync(IReadOnlyCollection<Guid> produtoIds, CancellationToken cancellationToken);
}
