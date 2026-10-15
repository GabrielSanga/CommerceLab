namespace CommerceLab.Modules.Cliente.Application.Abstracoes;

internal interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken);

}