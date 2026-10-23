using CommerceLab.Shared.Results;

namespace CommerceLab.Modules.Venda.Domain;

internal sealed class PedidoItem
{
    private PedidoItem(Guid id, Guid idPedido, Guid idProduto, string nomeProduto, string sku, int quantidade, decimal valorUnitario)
    {
        Id = id;
        IdPedido = idPedido;
        IdProduto = idProduto;
        NomeProduto = nomeProduto;
        Sku = sku;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
    }

    public Guid Id { get; }

    public Guid IdPedido { get; }

    public Guid IdProduto { get; }

    public string NomeProduto { get; }

    public string Sku { get; }

    public int Quantidade { get; }

    public decimal ValorUnitario { get; }

    public decimal Subtotal => Quantidade * ValorUnitario;

    public static Result<PedidoItem> Criar(Guid idPedido, Guid idProduto, string nomeProduto, string sku, int quantidade, decimal valorUnitario)
    {
        if (idProduto == Guid.Empty)
            return Result<PedidoItem>.Falha(Erro.Validacao("PEDIDO_ITEM_PRODUTO_VAZIO", "Id do produto não pode ser vazio."));

        if (quantidade <= 0)
            return Result<PedidoItem>.Falha(Erro.Validacao("PEDIDO_ITEM_QUANTIDADE_INVALIDA", "A quantidade deve ser maior que zero."));

        if (valorUnitario <= 0m)
            return Result<PedidoItem>.Falha(Erro.Validacao("PEDIDO_ITEM_VALOR_INVALIDO", "O valor unitário deve ser maior que zero."));

        var pedidoItem = new PedidoItem(Guid.NewGuid(), idPedido, idProduto, nomeProduto, sku, quantidade, valorUnitario);

        return Result<PedidoItem>.Ok(pedidoItem);
    }

}
