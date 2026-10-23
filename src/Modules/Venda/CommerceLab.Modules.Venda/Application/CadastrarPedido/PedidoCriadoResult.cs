namespace CommerceLab.Modules.Venda.Application.CadastrarPedido;

internal sealed record PedidoCriadoResult(Guid Id, string Codigo, decimal ValorTotal, int QuantidadeItens, DateTime DataCriacao);
