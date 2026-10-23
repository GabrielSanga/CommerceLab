namespace CommerceLab.Modules.Venda.Application.CadastrarPedido;

internal sealed record CadastrarPedidoCommand(Guid IdCliente, Guid IdEndereco, IReadOnlyCollection<CadastrarPedidoItemCommand> Itens);
