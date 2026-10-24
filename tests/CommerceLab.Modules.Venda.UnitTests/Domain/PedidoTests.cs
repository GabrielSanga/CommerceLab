using CommerceLab.Modules.Venda.Domain;
using CommerceLab.Modules.Venda.Domain.Enum;

namespace CommerceLab.Modules.Venda.UnitTests.Domain;

public class PedidoTests
{
    private static Pedido PedidoValido() => Pedido.Criar(Guid.NewGuid(), Guid.NewGuid()).Valor!;

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoClienteForVazio()
    {
        var resultado = Pedido.Criar(Guid.Empty, Guid.NewGuid());

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("CLIENTE_VAZIO", resultado.Erro.Codigo);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoEnderecoForVazio()
    {
        var resultado = Pedido.Criar(Guid.NewGuid(), Guid.Empty);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("ENDERECO_VAZIO", resultado.Erro.Codigo);
    }

    [Fact]
    public void Criar_DeveCriarPedidoSemItens_QuandoDadosForemValidos()
    {
        var idCliente = Guid.NewGuid();
        var idEndereco = Guid.NewGuid();

        var resultado = Pedido.Criar(idCliente, idEndereco);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);

        var pedido = resultado.Valor;

        Assert.NotEqual(Guid.Empty, pedido.Id);
        Assert.False(string.IsNullOrWhiteSpace(pedido.Codigo));
        Assert.Equal(idCliente, pedido.IdCliente);
        Assert.Equal(idEndereco, pedido.IdEndereco);
        Assert.Equal(PedidoStatusEnum.Criado, pedido.Status);
        Assert.Empty(pedido.Itens);
        Assert.Equal(0m, pedido.ValorTotal);
    }

    [Fact]
    public void AdicionarItem_DeveRetornarFalha_QuandoProdutoForVazio()
    {
        var pedido = PedidoValido();

        var resultado = pedido.AdicionarItem(Guid.Empty, "Teclado", "TEC-001", 1, 100m);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("PEDIDO_ITEM_PRODUTO_VAZIO", resultado.Erro.Codigo);
        Assert.Empty(pedido.Itens);
    }

    [Fact]
    public void AdicionarItem_DeveRetornarFalha_QuandoQuantidadeForInvalida()
    {
        var pedido = PedidoValido();

        var resultado = pedido.AdicionarItem(Guid.NewGuid(), "Teclado", "TEC-001", 0, 100m);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("PEDIDO_ITEM_QUANTIDADE_INVALIDA", resultado.Erro.Codigo);
        Assert.Empty(pedido.Itens);
    }

    [Fact]
    public void AdicionarItem_DeveRetornarFalha_QuandoValorUnitarioForInvalido()
    {
        var pedido = PedidoValido();

        var resultado = pedido.AdicionarItem(Guid.NewGuid(), "Teclado", "TEC-001", 1, 0m);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("PEDIDO_ITEM_VALOR_INVALIDO", resultado.Erro.Codigo);
        Assert.Empty(pedido.Itens);
    }

    [Fact]
    public void AdicionarItem_DeveAdicionarItensECalcularValorTotal_QuandoDadosForemValidos()
    {
        var pedido = PedidoValido();

        var primeiro = pedido.AdicionarItem(Guid.NewGuid(), "Teclado", "TEC-001", 2, 100m);
        var segundo = pedido.AdicionarItem(Guid.NewGuid(), "Mouse", "MOU-001", 1, 50m);

        Assert.True(primeiro.Sucesso);
        Assert.True(segundo.Sucesso);
        Assert.Equal(2, pedido.Itens.Count);
        Assert.Equal(250m, pedido.ValorTotal);
        Assert.All(pedido.Itens, item => Assert.Equal(pedido.Id, item.IdPedido));
    }

    [Fact]
    public void AdicionarItem_DeveGuardarSnapshotDoProdutoNoItem()
    {
        var pedido = PedidoValido();
        var idProduto = Guid.NewGuid();

        var resultado = pedido.AdicionarItem(idProduto, "Teclado", "TEC-001", 1, 299.90m);

        Assert.True(resultado.Sucesso);

        var pedidoItem = Assert.Single(pedido.Itens);

        Assert.Equal(idProduto, pedidoItem.IdProduto);
        Assert.Equal("Teclado", pedidoItem.NomeProduto);
        Assert.Equal("TEC-001", pedidoItem.Sku);
        Assert.Equal(299.90m, pedidoItem.ValorUnitario);
        Assert.Equal(299.90m, pedidoItem.Subtotal);
    }
}
