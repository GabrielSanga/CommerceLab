using CommerceLab.Modules.Catalogo.Application.Abstracoes;
using CommerceLab.Modules.Catalogo.Domain;
using Microsoft.EntityFrameworkCore;

namespace CommerceLab.Modules.Catalogo.Infrastructure.Persistencia
{
    internal sealed class ProdutoRepository(CatalogoDBContext contexto) : IProdutoRepository
    {
        public async Task<IReadOnlyList<Produto>> ListarAsync(bool? ativo, string? termo, CancellationToken cancellationToken)
        {
            var consulta = contexto.Produtos.AsNoTracking();

            if (ativo is not null)
            {
                consulta = consulta.Where(produto => produto.Ativo == ativo.Value);
            }

            if (!string.IsNullOrWhiteSpace(termo))
            {
                var padrao = $"%{termo.Trim()}%";
                consulta = consulta.Where(produto => EF.Functions.ILike(produto.Nome, padrao)
                                                  || EF.Functions.ILike(produto.Sku, padrao));
            }

            return await consulta.OrderBy(produto => produto.Nome).ToListAsync(cancellationToken);
        }

        public Task<Produto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) => contexto.Produtos.AsNoTracking().FirstOrDefaultAsync(produto => produto.Id == id, cancellationToken);

        public async Task<bool> TryAdicionarAsync(Produto produto, CancellationToken cancellationToken)
        {
            // O índice único é quem garante a correção; este SELECT existe para produzir o 409 com mensagem boa em vez de uma exceção no commit.
            if (await contexto.Produtos.AnyAsync(existente => existente.Sku == produto.Sku, cancellationToken))
            {
                return false;
            }

            contexto.Produtos.Add(produto);

            return true;
        }
    }
}
