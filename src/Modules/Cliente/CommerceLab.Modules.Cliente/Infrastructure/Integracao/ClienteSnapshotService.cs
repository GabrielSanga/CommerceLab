using CommerceLab.Modules.Cliente.Application.Abstracoes;
using CommerceLab.Modules.Cliente.Contracts.Clientes;

namespace CommerceLab.Modules.Cliente.Infrastructure.Integracao;

internal sealed class ClienteSnapshotService(IClienteRepository clientes) : IClienteSnapshotService
{
    public async Task<ClienteSnapshot?> ObterPorIdAsync(Guid idCliente, CancellationToken cancellationToken)
    {
        var cliente = await clientes.ObterPorIdAsync(idCliente, cancellationToken);

        return cliente is null
            ? null
            : new ClienteSnapshot(cliente.Id, cliente.Nome, cliente.Enderecos.Select(endereco => endereco.Id).ToArray());
    }
}
