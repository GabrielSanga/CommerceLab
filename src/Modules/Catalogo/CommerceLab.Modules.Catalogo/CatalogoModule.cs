using CommerceLab.Modules.Catalogo.Application.Abstracoes;
using CommerceLab.Modules.Catalogo.Application.Produtos.CadastrarProduto;
using CommerceLab.Modules.Catalogo.Application.Produtos.ListarProdutos;
using CommerceLab.Modules.Catalogo.Application.Produtos.ObterProdutoPorId;
using CommerceLab.Modules.Catalogo.Infrastructure.Persistencia;
using CommerceLab.Modules.Catalogo.Presentation.Produtos;
using CommerceLab.Shared.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceLab.Modules.Catalogo;

public sealed class CatalogoModule : IModule
{
    public string Name => "Catalogo";

    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {
        // Singleton: os dicionários em memória são o "banco" e precisam sobreviver às requisições.
        services.AddSingleton<IProdutoRepository, ProdutoRepositoryEmMemoria>();
        services.AddSingleton<IEstoqueRepository, EstoqueRepositoryEmMemoria>();

        services.AddScoped<CadastrarProdutoHandler>();
        services.AddScoped<ObterProdutoPorIdHandler>();
        services.AddScoped<ListarProdutosHandler>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var catalogo = endpoints.MapGroup("/api/catalogo").WithTags("Catalogo");

        ProdutoEndpoints.Map(catalogo.MapGroup("/produtos"));
    }
}
