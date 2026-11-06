namespace CommerceLab.Modules.Cliente.Contracts.Clientes;

public sealed record ClienteSnapshot(Guid Id, string Nome, IReadOnlyCollection<Guid> IdsEnderecos);
