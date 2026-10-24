using CommerceLab.Modules.Venda.Domain.Enum;
using CommerceLab.Shared.Results;

namespace CommerceLab.Modules.Venda.Domain;

internal sealed class Pedido
{

    private readonly List<PedidoItem> _itens = [];

    private Pedido(Guid id, string codigo, PedidoStatusEnum status, Guid idCliente, Guid idEndereco, DateTime dataCriacao)
    {
        Id = id;
        Codigo = codigo;
        Status = status;
        IdCliente = idCliente;
        IdEndereco = idEndereco;
        DataCriacao = dataCriacao;
    }

    public Guid Id { get; }

    public string Codigo { get; }

    public Guid IdCliente { get; }

    public Guid IdEndereco { get; }

    public IReadOnlyCollection<PedidoItem> Itens => _itens;

    public decimal ValorTotal => _itens.Sum(i => i.Subtotal);

    public PedidoStatusEnum Status { get; }

    public DateTime DataCriacao { get; }

    public static Result<Pedido> Criar(Guid idCliente, Guid idEndereco)
    {
        if (idCliente == Guid.Empty)
            return Result<Pedido>.Falha(Erro.Validacao("CLIENTE_VAZIO", "Id do cliente não pode ser vazio."));

        if (idEndereco == Guid.Empty)
            return Result<Pedido>.Falha(Erro.Validacao("ENDERECO_VAZIO", "Id do endereço não pode ser vazio."));

        var pedido = new Pedido(Guid.NewGuid(), GerarCodigo(), PedidoStatusEnum.Criado, idCliente, idEndereco, DateTime.UtcNow);

        return Result<Pedido>.Ok(pedido);
    }

    /// <summary>Único caminho para incluir itens: o agregado cria e valida seus próprios <see cref="PedidoItem"/>.</summary>
    public Result<PedidoItem> AdicionarItem(Guid idProduto, string nomeProduto, string sku, int quantidade, decimal valorUnitario)
    {
        var itemCriado = PedidoItem.Criar(Id, idProduto, nomeProduto, sku, quantidade, valorUnitario);
        if (!itemCriado.Sucesso)
            return itemCriado;

        _itens.Add(itemCriado.Valor!);

        return itemCriado;
    }

    // Sufixo aleatório evita colisão quando dois pedidos são criados no mesmo segundo.
    private static string GerarCodigo() =>
        $"PED-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
}
