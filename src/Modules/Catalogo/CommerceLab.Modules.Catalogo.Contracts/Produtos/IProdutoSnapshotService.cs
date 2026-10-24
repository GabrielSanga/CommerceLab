namespace CommerceLab.Modules.Catalogo.Contracts.Produtos;

public interface IProdutoSnapshotService
{
    Task<ProdutoSnapshot?> ObterPorIdAsync(Guid idProduto, CancellationToken cancellationToken);
}
