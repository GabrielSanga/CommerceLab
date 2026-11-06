using CommerceLab.IntegrationTests.Fixtures;
using CommerceLab.Modules.Catalogo.Application.Abstracoes;
using CommerceLab.Modules.Catalogo.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace CommerceLab.IntegrationTests.Catalogo;

[Collection("integration")]
public sealed class ProdutoEndpointsTests(IntegrationTestFactory factory)
{

    private readonly IntegrationTestFactory _factory = factory;

    [Fact]
    public async Task Cadastrar_DevePersistirProdutoEstoque_QuandoDadosForemValidos()
    {
        var client = _factory.CreateClient();

        var sku = $"TEC-MEC-ELETRONIC";
        var request = new CadastrarProdutoRequest("Teclado", "Teclado Mecânico", sku, 100m, 10);

        var response = await client.PostAsJsonAsync("/api/catalogo/produtos", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var criado = await response.Content.ReadFromJsonAsync<ProdutoCriadoResult>();

        Assert.NotNull(criado);
        Assert.NotEqual(Guid.Empty, criado.Id);
        Assert.Equal(request.nome, criado.Nome);
        Assert.Equal(request.sku, criado.Sku);
        Assert.Equal(request.preco, criado.Preco);
        Assert.Equal(request.quantidadeInicial, criado.QuantidadeDisponivel);

        var db = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<CatalogoDBContext>();

        var produtoRepository = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<IProdutoRepository>();
        var estoqueRepository = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<IEstoqueRepository>();

        var produtoPersistido = await produtoRepository.ObterPorIdAsync(criado.Id, CancellationToken.None);
        var estoquePersistido = await estoqueRepository.ObterPorProdutoIdAsync(criado.Id, CancellationToken.None);

        Assert.NotNull(produtoPersistido);
        Assert.Equal(criado.Id, produtoPersistido.Id);
        Assert.Equal(criado.Nome, produtoPersistido.Nome);
        Assert.Equal(criado.Sku, produtoPersistido.Sku);
        Assert.Equal(criado.Preco, produtoPersistido.Preco);

        Assert.NotNull(estoquePersistido);
        Assert.Equal(criado.Id, estoquePersistido.ProdutoId);
        Assert.Equal(criado.QuantidadeDisponivel, estoquePersistido.QuantidadeDisponivel);
    }
}

internal sealed record CadastrarProdutoRequest(string nome, string descricao, string sku, decimal preco, int quantidadeInicial);

internal sealed record ProdutoCriadoResult(Guid Id, string Sku, string Nome, decimal Preco, bool Ativo, DateTime CriadoEm, int QuantidadeDisponivel);