namespace CommerceLab.Modules.Catalogo.Application.Abstracoes
{
    internal interface IUnitOfWork
    {
        Task CommitAsync(CancellationToken cancellationToken);
    }
}
