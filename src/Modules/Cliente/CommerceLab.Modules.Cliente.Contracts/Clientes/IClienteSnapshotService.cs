namespace CommerceLab.Modules.Cliente.Contracts.Clientes;

public interface IClienteSnapshotService
{
    Task<ClienteSnapshot?> ObterPorIdAsync(Guid idCliente, CancellationToken cancellationToken);
}
