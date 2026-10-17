using CommerceLab.Modules.Cliente.Domain;

namespace CommerceLab.Modules.Cliente.Application.Abstracoes
{
    internal interface IClienteRepository
    {

        Task<bool> TryAdicionarAsync(Domain.Cliente cliente, CancellationToken cancellationToken);

        Task<Domain.Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    }
}
