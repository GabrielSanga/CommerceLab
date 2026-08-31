using System.Collections.Concurrent;
using CommerceLab.Modules.Catalogo.Application.Abstracoes;
using CommerceLab.Modules.Catalogo.Domain;

namespace CommerceLab.Modules.Catalogo.Infrastructure.Persistencia;

/// <summary>
/// Armazenamento em memória, chaveado pelo produto — que é como o estoque é sempre consultado.
/// </summary>
internal sealed class EstoqueRepositoryEmMemoria : IEstoqueRepository
{
    private readonly ConcurrentDictionary<Guid, Estoque> _estoquesPorProduto = new();

    public Task AdicionarAsync(Estoque estoque, CancellationToken cancellationToken)
    {
        _estoquesPorProduto[estoque.ProdutoId] = estoque;

        return Task.CompletedTask;
    }

    public Task<Estoque?> ObterPorProdutoIdAsync(Guid produtoId, CancellationToken cancellationToken) =>
        Task.FromResult(_estoquesPorProduto.GetValueOrDefault(produtoId));

    public Task<IReadOnlyDictionary<Guid, Estoque>> ObterPorProdutosAsync(IReadOnlyCollection<Guid> produtoIds, CancellationToken cancellationToken)
    {
        IReadOnlyDictionary<Guid, Estoque> encontrados = produtoIds
            .Select(produtoId => _estoquesPorProduto.GetValueOrDefault(produtoId))
            .OfType<Estoque>()
            .ToDictionary(estoque => estoque.ProdutoId);

        return Task.FromResult(encontrados);
    }
}
