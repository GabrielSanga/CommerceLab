using CommerceLab.Shared.Results;

namespace CommerceLab.Modules.Catalogo.Contracts.Estoques;

public interface IEstoqueService
{
    Task<Result> BaixarAsync(IReadOnlyCollection<ItemBaixaEstoque> itens, CancellationToken cancellationToken);

    Task<Result> EstornarAsync(IReadOnlyCollection<ItemEstornoEstoque> itens, CancellationToken cancellationToken);
}
