using CommerceLab.Modules.Catalogo.Application.Abstracoes;
using CommerceLab.Modules.Catalogo.Contracts.Estoques;
using CommerceLab.Shared.Results;

namespace CommerceLab.Modules.Catalogo.Infrastructure.Integracao;

internal sealed class EstoqueService(IEstoqueRepository estoques, IUnitOfWork unitOfWork) : IEstoqueService
{
    public async Task<Result> BaixarAsync(IReadOnlyCollection<ItemBaixaEstoque> itens, CancellationToken cancellationToken)
    {
        if (itens is null || itens.Count == 0)
        {
            return Result.Falha(Erro.Validacao("BAIXA_ESTOQUE_ITENS_VAZIOS", "A lista de itens para baixa não pode ser vazia."));
        }

        var idsProdutos = itens.Select(item => item.IdProduto).Distinct().ToArray();
        var estoquesPorProduto = await estoques.ObterPorProdutosParaAtualizacaoAsync(idsProdutos, cancellationToken);

        foreach (var item in itens)
        {
            if (!estoquesPorProduto.TryGetValue(item.IdProduto, out var estoque))
            {
                return Result.Falha(Erro.NaoEncontrado("ESTOQUE_NAO_ENCONTRADO", $"Não existe estoque para o produto '{item.IdProduto}'."));
            }

            var debito = estoque.Debitar(item.Quantidade);
            if (!debito.Sucesso)
            {
                return Result.Falha(debito.Erro!);
            }
        }

        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Ok();
    }
}
