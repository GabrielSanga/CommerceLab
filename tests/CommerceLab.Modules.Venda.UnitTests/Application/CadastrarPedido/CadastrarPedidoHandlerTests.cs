using CommerceLab.Modules.Catalogo.Contracts.Produtos;
using CommerceLab.Modules.Venda.Application.Abstracoes;
using CommerceLab.Modules.Venda.Application.CadastrarPedido;
using CommerceLab.Modules.Venda.Domain;

namespace CommerceLab.Modules.Venda.UnitTests.Application.CadastrarPedido;

public class CadastrarPedidoHandlerTests
{

    [Fact]
    public async Task HandleAsync_DeveRetornarFalhaSemPersistir_QuandoProdutoNaoExistir()
    {
        var snapshots = new ProdutoSnapshotServiceStub();
        var pedidos = new PedidoRepositorySpy();
        var unitOfWork = new UnitOfWorkSpy();
        var handler = new CadastrarPedidoHandler(pedidos, snapshots, unitOfWork);
        var comando = ComandoValido();

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("PRODUTO_NAO_ENCONTRADO", resultado.Erro.Codigo);
        Assert.Equal(0, pedidos.QuantidadeAdicoes);
        Assert.Equal(0, unitOfWork.QuantidadeCommits);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarFalhaSemPersistir_QuandoProdutoEstiverInativo()
    {
        var snapshots = new ProdutoSnapshotServiceStub();
        var comando = ComandoValido();
        snapshots.Adicionar(new ProdutoSnapshot(comando.Itens.First().IdProduto, "TEC-001", "Teclado", 100m, Ativo: false));
        var pedidos = new PedidoRepositorySpy();
        var unitOfWork = new UnitOfWorkSpy();
        var handler = new CadastrarPedidoHandler(pedidos, snapshots, unitOfWork);

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("PRODUTO_INATIVO", resultado.Erro.Codigo);
        Assert.Equal(0, pedidos.QuantidadeAdicoes);
        Assert.Equal(0, unitOfWork.QuantidadeCommits);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarFalhaSemPersistir_QuandoListaDeItensForVazia()
    {
        var snapshots = new ProdutoSnapshotServiceStub();
        var pedidos = new PedidoRepositorySpy();
        var unitOfWork = new UnitOfWorkSpy();
        var handler = new CadastrarPedidoHandler(pedidos, snapshots, unitOfWork);
        var comando = new CadastrarPedidoCommand(Guid.NewGuid(), Guid.NewGuid(), []);

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("ITENS_VAZIOS", resultado.Erro.Codigo);
        Assert.Equal(0, pedidos.QuantidadeAdicoes);
        Assert.Equal(0, unitOfWork.QuantidadeCommits);
    }

    [Fact]
    public async Task HandleAsync_DevePersistirPedidoComPrecoDoCatalogo_QuandoComandoForValido()
    {
        var snapshots = new ProdutoSnapshotServiceStub();
        var comando = ComandoValido(quantidade: 3);
        var idProduto = comando.Itens.First().IdProduto;
        snapshots.Adicionar(new ProdutoSnapshot(idProduto, "TEC-001", "Teclado", 199.90m, Ativo: true));
        var pedidos = new PedidoRepositorySpy();
        var unitOfWork = new UnitOfWorkSpy();
        var handler = new CadastrarPedidoHandler(pedidos, snapshots, unitOfWork);

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Valor);
        Assert.Equal(1, pedidos.QuantidadeAdicoes);
        Assert.Equal(1, unitOfWork.QuantidadeCommits);

        var pedidoAdicionado = Assert.IsType<Pedido>(pedidos.PedidoAdicionado);
        var item = Assert.Single(pedidoAdicionado.Itens);

        // O valor vem do snapshot do Catálogo (199,90), nunca da requisição.
        Assert.Equal(199.90m, item.ValorUnitario);
        Assert.Equal(3, item.Quantidade);
        Assert.Equal(599.70m, pedidoAdicionado.ValorTotal);
        Assert.Equal(resultado.Valor.Id, pedidoAdicionado.Id);
        Assert.Equal(resultado.Valor.ValorTotal, pedidoAdicionado.ValorTotal);
    }

    private static CadastrarPedidoCommand ComandoValido(int quantidade = 1) =>
        new(Guid.NewGuid(), Guid.NewGuid(), [new CadastrarPedidoItemCommand(Guid.NewGuid(), quantidade)]);

    private sealed class ProdutoSnapshotServiceStub : IProdutoSnapshotService
    {
        private readonly Dictionary<Guid, ProdutoSnapshot> _snapshots = [];

        public void Adicionar(ProdutoSnapshot snapshot) => _snapshots[snapshot.Id] = snapshot;

        public Task<ProdutoSnapshot?> ObterPorIdAsync(Guid idProduto, CancellationToken cancellationToken) =>
            Task.FromResult(_snapshots.GetValueOrDefault(idProduto));
    }

    private sealed class PedidoRepositorySpy : IPedidoRepository
    {
        public int QuantidadeAdicoes { get; private set; }

        public Pedido? PedidoAdicionado { get; private set; }

        public Task AdicionarAsync(Pedido pedido, CancellationToken cancellationToken)
        {
            QuantidadeAdicoes++;
            PedidoAdicionado = pedido;
            return Task.CompletedTask;
        }
    }

    private sealed class UnitOfWorkSpy : IUnitOfWork
    {
        public int QuantidadeCommits { get; private set; }

        public Task CommitAsync(CancellationToken cancellationToken)
        {
            QuantidadeCommits++;
            return Task.CompletedTask;
        }
    }
}
