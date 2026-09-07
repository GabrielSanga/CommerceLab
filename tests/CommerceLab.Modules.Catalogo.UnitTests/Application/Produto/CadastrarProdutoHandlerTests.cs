using CommerceLab.Modules.Catalogo.Application.Abstracoes;
using CommerceLab.Modules.Catalogo.Application.Produtos.CadastrarProduto;
using CommerceLab.Modules.Catalogo.Domain;
using ProdutoDomain = CommerceLab.Modules.Catalogo.Domain.Produto;

namespace CommerceLab.Modules.Catalogo.UnitTests.Application.Produto;

public class CadastrarProdutoHandlerTests
{

    [Fact]
    public async Task HandleAsync_DeveRetornarFalhaSemPersistir_QuandoProdutoForInvalido()
    {
        var produtos = new ProdutoRepositorySpy();
        var estoques = new EstoqueRepositorySpy();
        var unitOfWork = new UnitOfWorkSpy();
        var handler = new CadastrarProdutoHandler(produtos, estoques, unitOfWork);
        var comando = new CadastrarProdutoCommand(string.Empty, null, "TEC-001", 100m, 10);

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("PRODUTO_NOME_INVALIDO", resultado.Erro.Codigo);
        Assert.Equal(0, produtos.QuantidadeTentativasAdicionar);
        Assert.Equal(0, estoques.QuantidadeAdicoes);
        Assert.Equal(0, unitOfWork.QuantidadeCommits);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarFalhaSemPersistir_QuandoEstoqueForInvalido()
    {
        var produtos = new ProdutoRepositorySpy();
        var estoques = new EstoqueRepositorySpy();
        var unitOfWork = new UnitOfWorkSpy();
        var handler = new CadastrarProdutoHandler(produtos, estoques, unitOfWork);
        var comando = new CadastrarProdutoCommand("Teclado", null, "TEC-001", 100m, -1);

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("ESTOQUE_QUANTIDADE_INVALIDA", resultado.Erro.Codigo);
        Assert.Equal(0, produtos.QuantidadeTentativasAdicionar);
        Assert.Equal(0, estoques.QuantidadeAdicoes);
        Assert.Equal(0, unitOfWork.QuantidadeCommits);
    }

    [Fact]
    public async Task HandleAsync_DevePersistirProdutoEEstoqueRelacionados_QuandoComandoForValido()
    {
        var produtos = new ProdutoRepositorySpy();
        var estoques = new EstoqueRepositorySpy();
        var unitOfWork = new UnitOfWorkSpy();
        var handler = new CadastrarProdutoHandler(produtos, estoques, unitOfWork);
        var comando = new CadastrarProdutoCommand("Teclado", null, "TEC-001", 100m, 10);

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.Null(resultado.Erro);
        Assert.NotNull(resultado.Valor);
        Assert.Equal(1, produtos.QuantidadeTentativasAdicionar);
        Assert.Equal(1, estoques.QuantidadeAdicoes);
        Assert.Equal(1, unitOfWork.QuantidadeCommits);

        var produtoAdicionado = Assert.IsType<ProdutoDomain>(produtos.ProdutoAdicionado);
        var estoqueAdicionado = Assert.IsType<Estoque>(estoques.EstoqueAdicionado);

        Assert.NotEqual(Guid.Empty, produtoAdicionado.Id);
        Assert.NotEqual(Guid.Empty, estoqueAdicionado.Id);
        Assert.Equal(produtoAdicionado.Id, estoqueAdicionado.ProdutoId);
        Assert.Equal(comando.QuantidadeInicial, estoqueAdicionado.QuantidadeDisponivel);
        Assert.Equal(produtoAdicionado.Id, resultado.Valor.Id);
        Assert.Equal(estoqueAdicionado.QuantidadeDisponivel, resultado.Valor.QuantidadeDisponivel);
    }

    private sealed class ProdutoRepositorySpy : IProdutoRepository
    {
        public int QuantidadeTentativasAdicionar { get; private set; }

        public ProdutoDomain? ProdutoAdicionado { get; private set; }

        public Task<bool> TryAdicionarAsync(ProdutoDomain produto, CancellationToken cancellationToken)
        {
            QuantidadeTentativasAdicionar++;
            ProdutoAdicionado = produto;
            return Task.FromResult(true);
        }

        public Task<ProdutoDomain?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<ProdutoDomain>> ListarAsync(bool? ativo, string? termo, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }

    private sealed class EstoqueRepositorySpy : IEstoqueRepository
    {
        public int QuantidadeAdicoes { get; private set; }

        public Estoque? EstoqueAdicionado { get; private set; }

        public Task AdicionarAsync(Estoque estoque, CancellationToken cancellationToken)
        {
            QuantidadeAdicoes++;
            EstoqueAdicionado = estoque;
            return Task.CompletedTask;
        }

        public Task<Estoque?> ObterPorProdutoIdAsync(Guid produtoId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IReadOnlyDictionary<Guid, Estoque>> ObterPorProdutosAsync(
            IReadOnlyCollection<Guid> produtoIds,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();
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
