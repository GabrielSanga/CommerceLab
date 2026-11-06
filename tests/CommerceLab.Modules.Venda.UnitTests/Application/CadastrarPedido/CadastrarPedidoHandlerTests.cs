using CommerceLab.Modules.Catalogo.Contracts.Estoques;
using CommerceLab.Modules.Catalogo.Contracts.Produtos;
using CommerceLab.Modules.Cliente.Contracts.Clientes;
using CommerceLab.Modules.Venda.Application.Abstracoes;
using CommerceLab.Modules.Venda.Application.CadastrarPedido;
using CommerceLab.Modules.Venda.Application.CadastrarPedido.Saga;
using CommerceLab.Modules.Venda.Domain;
using CommerceLab.Shared.Results;
using CommerceLab.Shared.Sagas;

namespace CommerceLab.Modules.Venda.UnitTests.Application.CadastrarPedido;

public class CadastrarPedidoHandlerTests
{

    [Fact]
    public async Task HandleAsync_DeveRetornarFalhaSemPersistir_QuandoClienteNaoExistir()
    {
        var snapshots = new ProdutoSnapshotServiceStub();
        var comando = ComandoValido();
        snapshots.Adicionar(new ProdutoSnapshot(comando.Itens.First().IdProduto, "TEC-001", "Teclado", 199.90m, Ativo: true));
        var pedidos = new PedidoRepositorySpy();
        var estoque = new EstoqueServiceSpy();
        var unitOfWork = new UnitOfWorkSpy();
        var handler = CriarHandler(comando, snapshots, pedidos, estoque, unitOfWork, ClienteSnapshotServiceStub.Vazio());

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("CLIENTE_NAO_ENCONTRADO", resultado.Erro.Codigo);
        // O cliente é checado antes de qualquer efeito colateral: nada de baixa nem de persistência.
        Assert.Null(estoque.ItensBaixados);
        Assert.Equal(0, pedidos.QuantidadeAdicoes);
        Assert.Equal(0, unitOfWork.QuantidadeCommits);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarFalhaSemPersistir_QuandoEnderecoNaoForDoCliente()
    {
        var snapshots = new ProdutoSnapshotServiceStub();
        var comando = ComandoValido();
        snapshots.Adicionar(new ProdutoSnapshot(comando.Itens.First().IdProduto, "TEC-001", "Teclado", 199.90m, Ativo: true));
        var pedidos = new PedidoRepositorySpy();
        var estoque = new EstoqueServiceSpy();
        var unitOfWork = new UnitOfWorkSpy();
        var clientes = new ClienteSnapshotServiceStub();
        // O cliente existe, mas o endereço pedido é de outro.
        clientes.Adicionar(new ClienteSnapshot(comando.IdCliente, "Cliente de Teste", [Guid.NewGuid()]));
        var handler = CriarHandler(comando, snapshots, pedidos, estoque, unitOfWork, clientes);

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("ENDERECO_NAO_ENCONTRADO", resultado.Erro.Codigo);
        Assert.Null(estoque.ItensBaixados);
        Assert.Equal(0, pedidos.QuantidadeAdicoes);
        Assert.Equal(0, unitOfWork.QuantidadeCommits);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarFalhaSemPersistir_QuandoProdutoNaoExistir()
    {
        var snapshots = new ProdutoSnapshotServiceStub();
        var comando = ComandoValido();
        var pedidos = new PedidoRepositorySpy();
        var estoque = new EstoqueServiceSpy();
        var unitOfWork = new UnitOfWorkSpy();
        var handler = CriarHandler(comando, snapshots, pedidos, estoque, unitOfWork);

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("PRODUTO_NAO_ENCONTRADO", resultado.Erro.Codigo);
        Assert.Null(estoque.ItensBaixados);
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
        var estoque = new EstoqueServiceSpy();
        var unitOfWork = new UnitOfWorkSpy();
        var handler = CriarHandler(comando, snapshots, pedidos, estoque, unitOfWork);

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("PRODUTO_INATIVO", resultado.Erro.Codigo);
        Assert.Null(estoque.ItensBaixados);
        Assert.Equal(0, pedidos.QuantidadeAdicoes);
        Assert.Equal(0, unitOfWork.QuantidadeCommits);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarFalhaSemPersistir_QuandoListaDeItensForVazia()
    {
        var snapshots = new ProdutoSnapshotServiceStub();
        var pedidos = new PedidoRepositorySpy();
        var estoque = new EstoqueServiceSpy();
        var unitOfWork = new UnitOfWorkSpy();
        var comando = new CadastrarPedidoCommand(Guid.NewGuid(), Guid.NewGuid(), []);
        var handler = CriarHandler(comando, snapshots, pedidos, estoque, unitOfWork);

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("ITENS_VAZIOS", resultado.Erro.Codigo);
        Assert.Equal(0, pedidos.QuantidadeAdicoes);
        Assert.Equal(0, unitOfWork.QuantidadeCommits);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarFalhaSemPersistirSemEstornar_QuandoEstoqueForInsuficiente()
    {
        var snapshots = new ProdutoSnapshotServiceStub();
        var comando = ComandoValido(quantidade: 10);
        snapshots.Adicionar(new ProdutoSnapshot(comando.Itens.First().IdProduto, "TEC-001", "Teclado", 199.90m, Ativo: true));
        var pedidos = new PedidoRepositorySpy();
        var estoque = new EstoqueServiceSpy
        {
            Resultado = Result.Falha(Erro.Conflito("ESTOQUE_INSUFICIENTE", "Estoque insuficiente: disponível 4, solicitado 10."))
        };
        var unitOfWork = new UnitOfWorkSpy();
        var handler = CriarHandler(comando, snapshots, pedidos, estoque, unitOfWork);

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("ESTOQUE_INSUFICIENTE", resultado.Erro.Codigo);
        // A baixa não foi concluída, então não há o que compensar.
        Assert.Null(estoque.ItensEstornados);
        Assert.Equal(0, pedidos.QuantidadeAdicoes);
        Assert.Equal(0, unitOfWork.QuantidadeCommits);
    }

    [Fact]
    public async Task HandleAsync_DeveEstornarEstoque_QuandoPersistenciaDoPedidoFalhar()
    {
        var snapshots = new ProdutoSnapshotServiceStub();
        var comando = ComandoValido(quantidade: 3);
        var idProduto = comando.Itens.First().IdProduto;
        snapshots.Adicionar(new ProdutoSnapshot(idProduto, "TEC-001", "Teclado", 199.90m, Ativo: true));
        var pedidos = new PedidoRepositorySpy();
        var estoque = new EstoqueServiceSpy();
        var unitOfWork = new UnitOfWorkSpy { ExcecaoNoCommit = new InvalidOperationException("falha no banco") };
        var handler = CriarHandler(comando, snapshots, pedidos, estoque, unitOfWork);

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("SAGA_ETAPA_EXCECAO", resultado.Erro.Codigo);

        // A baixa foi concluída antes da falha, então a saga precisa estorná-la.
        var itemEstornado = Assert.Single(estoque.ItensEstornados!);
        Assert.Equal(idProduto, itemEstornado.IdProduto);
        Assert.Equal(3, itemEstornado.Quantidade);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarFalhaSemEstornar_QuandoBaixaDeEstoqueLancarExcecao()
    {
        var snapshots = new ProdutoSnapshotServiceStub();
        var comando = ComandoValido();
        snapshots.Adicionar(new ProdutoSnapshot(comando.Itens.First().IdProduto, "TEC-001", "Teclado", 199.90m, Ativo: true));
        var pedidos = new PedidoRepositorySpy();
        var estoque = new EstoqueServiceSpy { ExcecaoNaBaixa = new InvalidOperationException("módulo Catálogo indisponível") };
        var unitOfWork = new UnitOfWorkSpy();
        var handler = CriarHandler(comando, snapshots, pedidos, estoque, unitOfWork);

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("SAGA_ETAPA_EXCECAO", resultado.Erro.Codigo);
        // A baixa não foi concluída, então não há o que compensar.
        Assert.Null(estoque.ItensEstornados);
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
        var estoque = new EstoqueServiceSpy();
        var unitOfWork = new UnitOfWorkSpy();
        var handler = CriarHandler(comando, snapshots, pedidos, estoque, unitOfWork);

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

    [Fact]
    public async Task HandleAsync_DeveBaixarEstoqueDosItens_QuandoComandoForValido()
    {
        var snapshots = new ProdutoSnapshotServiceStub();
        var comando = ComandoValido(quantidade: 3);
        var idProduto = comando.Itens.First().IdProduto;
        snapshots.Adicionar(new ProdutoSnapshot(idProduto, "TEC-001", "Teclado", 199.90m, Ativo: true));
        var pedidos = new PedidoRepositorySpy();
        var estoque = new EstoqueServiceSpy();
        var unitOfWork = new UnitOfWorkSpy();
        var handler = CriarHandler(comando, snapshots, pedidos, estoque, unitOfWork);

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.Null(estoque.ItensEstornados);

        var itemBaixado = Assert.Single(estoque.ItensBaixados!);
        Assert.Equal(idProduto, itemBaixado.IdProduto);
        Assert.Equal(3, itemBaixado.Quantidade);
    }

    private static CadastrarPedidoHandler CriarHandler(CadastrarPedidoCommand comando, ProdutoSnapshotServiceStub snapshots, PedidoRepositorySpy pedidos, EstoqueServiceSpy estoque, UnitOfWorkSpy unitOfWork, ClienteSnapshotServiceStub? clientes = null)
    {
        // Sem indicação em contrário, o cliente do comando existe e é dono do endereço: os testes de produto e de estoque não precisam montar isso.
        clientes ??= ClienteSnapshotServiceStub.ComClienteDoComando(comando);

        var orquestrador = new SagaOrchestrator<CadastrarPedidoContexto>([new BaixarEstoqueStep(estoque), new PersistirPedidoStep(pedidos, unitOfWork)]);

        return new CadastrarPedidoHandler(clientes, snapshots, orquestrador);
    }

    private static CadastrarPedidoCommand ComandoValido(int quantidade = 1) =>
        new(Guid.NewGuid(), Guid.NewGuid(), [new CadastrarPedidoItemCommand(Guid.NewGuid(), quantidade)]);

    private sealed class ClienteSnapshotServiceStub : IClienteSnapshotService
    {
        private readonly Dictionary<Guid, ClienteSnapshot> _snapshots = [];

        public static ClienteSnapshotServiceStub Vazio() => new();

        public static ClienteSnapshotServiceStub ComClienteDoComando(CadastrarPedidoCommand comando)
        {
            var stub = new ClienteSnapshotServiceStub();
            stub.Adicionar(new ClienteSnapshot(comando.IdCliente, "Cliente de Teste", [comando.IdEndereco]));

            return stub;
        }

        public void Adicionar(ClienteSnapshot snapshot) => _snapshots[snapshot.Id] = snapshot;

        public Task<ClienteSnapshot?> ObterPorIdAsync(Guid idCliente, CancellationToken cancellationToken) =>
            Task.FromResult(_snapshots.GetValueOrDefault(idCliente));
    }

    private sealed class ProdutoSnapshotServiceStub : IProdutoSnapshotService
    {
        private readonly Dictionary<Guid, ProdutoSnapshot> _snapshots = [];

        public void Adicionar(ProdutoSnapshot snapshot) => _snapshots[snapshot.Id] = snapshot;

        public Task<ProdutoSnapshot?> ObterPorIdAsync(Guid idProduto, CancellationToken cancellationToken) =>
            Task.FromResult(_snapshots.GetValueOrDefault(idProduto));
    }

    private sealed class EstoqueServiceSpy : IEstoqueService
    {
        public Result Resultado { get; set; } = Result.Ok();

        public Exception? ExcecaoNaBaixa { get; set; }

        public IReadOnlyCollection<ItemBaixaEstoque>? ItensBaixados { get; private set; }

        public IReadOnlyCollection<ItemEstornoEstoque>? ItensEstornados { get; private set; }

        public Task<Result> BaixarAsync(IReadOnlyCollection<ItemBaixaEstoque> itens, CancellationToken cancellationToken)
        {
            if (ExcecaoNaBaixa is not null)
            {
                return Task.FromException<Result>(ExcecaoNaBaixa);
            }

            ItensBaixados = itens;
            return Task.FromResult(Resultado);
        }

        public Task<Result> EstornarAsync(IReadOnlyCollection<ItemEstornoEstoque> itens, CancellationToken cancellationToken)
        {
            ItensEstornados = itens;
            return Task.FromResult(Result.Ok());
        }
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

        public Exception? ExcecaoNoCommit { get; set; }

        public Task CommitAsync(CancellationToken cancellationToken)
        {
            QuantidadeCommits++;
            return ExcecaoNoCommit is null ? Task.CompletedTask : Task.FromException(ExcecaoNoCommit);
        }
    }
}
