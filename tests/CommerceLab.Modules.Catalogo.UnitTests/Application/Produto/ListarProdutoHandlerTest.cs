using CommerceLab.Modules.Catalogo.Application.Abstracoes;
using CommerceLab.Modules.Catalogo.Application.Produtos.ListarProdutos;
using CommerceLab.Modules.Catalogo.Domain;
using ProdutoDomain = CommerceLab.Modules.Catalogo.Domain.Produto;

namespace CommerceLab.Modules.Catalogo.UnitTests.Application.Produto;

public class ListarProdutoHandlerTest
{
    [Fact]
    public async Task HandleAsync_DeveMapearProdutosComSeusEstoques()
    {
        var produtoComEstoquePositivo = CriarProduto("Teclado", "TEC-001", 100m);
        var produtoComEstoqueZero = CriarProduto("Mouse", "MOU-001", 50m);
        var estoquePositivo = CriarEstoque(produtoComEstoquePositivo.Id, 10);
        var estoqueZero = CriarEstoque(produtoComEstoqueZero.Id, 0);
        var produtos = new ProdutoRepositoryStub([produtoComEstoquePositivo, produtoComEstoqueZero]);
        var estoques = new EstoqueRepositoryStub(new Dictionary<Guid, Estoque>
        {
            [produtoComEstoquePositivo.Id] = estoquePositivo,
            [produtoComEstoqueZero.Id] = estoqueZero
        });
        var handler = new ListarProdutosHandler(produtos, estoques);
        var consulta = new ListarProdutosQuery(null, null);

        var resultado = await handler.HandleAsync(consulta, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.Null(resultado.Erro);
        Assert.NotNull(resultado.Valor);
        Assert.Collection(
            resultado.Valor,
            produto =>
            {
                Assert.Equal(produtoComEstoquePositivo.Id, produto.Id);
                Assert.Equal(produtoComEstoquePositivo.Sku, produto.Sku);
                Assert.Equal(produtoComEstoquePositivo.Nome, produto.Nome);
                Assert.Equal(produtoComEstoquePositivo.Preco, produto.Preco);
                Assert.Equal(produtoComEstoquePositivo.Ativo, produto.Ativo);
                Assert.Equal(estoquePositivo.QuantidadeDisponivel, produto.QuantidadeDisponivel);
            },
            produto =>
            {
                Assert.Equal(produtoComEstoqueZero.Id, produto.Id);
                Assert.Equal(produtoComEstoqueZero.Sku, produto.Sku);
                Assert.Equal(produtoComEstoqueZero.Nome, produto.Nome);
                Assert.Equal(produtoComEstoqueZero.Preco, produto.Preco);
                Assert.Equal(produtoComEstoqueZero.Ativo, produto.Ativo);
                Assert.Equal(estoqueZero.QuantidadeDisponivel, produto.QuantidadeDisponivel);
            });
    }

    private static ProdutoDomain CriarProduto(string nome, string sku, decimal preco)
    {
        var resultado = ProdutoDomain.Criar(nome, null, sku, preco);

        return Assert.IsType<ProdutoDomain>(resultado.Valor);
    }

    private static Estoque CriarEstoque(Guid produtoId, int quantidadeInicial)
    {
        var resultado = Estoque.Criar(produtoId, quantidadeInicial);

        return Assert.IsType<Estoque>(resultado.Valor);
    }

    private sealed class ProdutoRepositoryStub(IReadOnlyList<ProdutoDomain> produtos) : IProdutoRepository
    {
        public Task<IReadOnlyList<ProdutoDomain>> ListarAsync(
            bool? ativo,
            string? termo,
            CancellationToken cancellationToken) => Task.FromResult(produtos);

        public Task<bool> TryAdicionarAsync(ProdutoDomain produto, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<ProdutoDomain?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }

    private sealed class EstoqueRepositoryStub(IReadOnlyDictionary<Guid, Estoque> estoques) : IEstoqueRepository
    {
        public Task<IReadOnlyDictionary<Guid, Estoque>> ObterPorProdutosAsync(
            IReadOnlyCollection<Guid> produtoIds,
            CancellationToken cancellationToken) => Task.FromResult(estoques);

        public Task AdicionarAsync(Estoque estoque, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<Estoque?> ObterPorProdutoIdAsync(Guid produtoId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }
}
