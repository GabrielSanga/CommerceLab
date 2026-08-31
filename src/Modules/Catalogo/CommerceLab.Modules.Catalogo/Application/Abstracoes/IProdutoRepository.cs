using CommerceLab.Modules.Catalogo.Domain;

namespace CommerceLab.Modules.Catalogo.Application.Abstracoes;

internal interface IProdutoRepository
{

    Task<bool> TryAdicionarAsync(Produto produto, CancellationToken cancellationToken);

    Task<Produto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Produto>> ListarAsync(bool? ativo, string? termo, CancellationToken cancellationToken);
}
