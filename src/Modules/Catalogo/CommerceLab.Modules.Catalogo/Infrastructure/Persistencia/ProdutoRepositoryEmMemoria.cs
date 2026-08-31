using System.Collections.Concurrent;
using CommerceLab.Modules.Catalogo.Application.Abstracoes;
using CommerceLab.Modules.Catalogo.Domain;

namespace CommerceLab.Modules.Catalogo.Infrastructure.Persistencia;

internal sealed class ProdutoRepositoryEmMemoria : IProdutoRepository
{
    private readonly ConcurrentDictionary<Guid, Produto> _produtos = new();

    /// <summary>Índice de unicidade de SKU. Faz o papel do índice único do banco.</summary>
    private readonly ConcurrentDictionary<string, Guid> _skus = new(StringComparer.Ordinal);

    public Task<bool> TryAdicionarAsync(Produto produto, CancellationToken cancellationToken)
    {
        // Reservar o SKU primeiro: quem perder a corrida sai daqui sem ter gravado nada.
        if (!_skus.TryAdd(produto.Sku, produto.Id))
        {
            return Task.FromResult(false);
        }

        _produtos[produto.Id] = produto;

        return Task.FromResult(true);
    }

    public Task<Produto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_produtos.GetValueOrDefault(id));

    public Task<IReadOnlyList<Produto>> ListarAsync(
        bool? ativo, string? termo, CancellationToken cancellationToken)
    {
        var consulta = _produtos.Values.AsEnumerable();

        if (ativo is not null)
        {
            consulta = consulta.Where(produto => produto.Ativo == ativo);
        }

        if (!string.IsNullOrWhiteSpace(termo))
        {
            var procurado = termo.Trim();
            consulta = consulta.Where(produto => produto.Nome.Contains(procurado, StringComparison.OrdinalIgnoreCase) || produto.Sku.Contains(procurado, StringComparison.OrdinalIgnoreCase));
        }

        IReadOnlyList<Produto> resultado = [.. consulta.OrderBy(produto => produto.Nome, StringComparer.OrdinalIgnoreCase)];

        return Task.FromResult(resultado);
    }
}
