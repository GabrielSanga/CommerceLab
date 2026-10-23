namespace CommerceLab.Modules.Venda.Application.Abstracoes;

internal interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken);

}
