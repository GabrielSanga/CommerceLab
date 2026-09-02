using CommerceLab.Modules.Catalogo.Application.Abstracoes;
using CommerceLab.Modules.Catalogo.Domain;
using Microsoft.EntityFrameworkCore;

namespace CommerceLab.Modules.Catalogo.Infrastructure.Persistencia
{
    internal sealed class EstoqueRepository(CatalogoDBContext contexto) : IEstoqueRepository
    {
        public Task AdicionarAsync(Estoque estoque, CancellationToken cancellationToken)
        {
            contexto.Estoques.Add(estoque);

            return Task.CompletedTask;
        }

        public Task<Estoque?> ObterPorProdutoIdAsync(Guid produtoId, CancellationToken cancellationToken) => contexto.Estoques.AsNoTracking().FirstOrDefaultAsync(estoque => estoque.ProdutoId == produtoId, cancellationToken);

        public async Task<IReadOnlyDictionary<Guid, Estoque>> ObterPorProdutosAsync(IReadOnlyCollection<Guid> produtoIds, CancellationToken cancellationToken)
        {
            if (produtoIds.Count == 0)
            {
                return new Dictionary<Guid, Estoque>();
            }

            return await contexto.Estoques.AsNoTracking().Where(estoque => produtoIds.Contains(estoque.ProdutoId)).ToDictionaryAsync(estoque => estoque.ProdutoId, cancellationToken);
        }
    }
}
