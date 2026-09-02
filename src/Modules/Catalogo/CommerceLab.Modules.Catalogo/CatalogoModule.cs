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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceLab.Modules.Catalogo;

public sealed class CatalogoModule : IModule
{
    public string Name => "Catalogo";

    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {
        AddData(services, configuration);
        AddHandlers(services);
    }

    private static void AddData(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Catalogo");

        services.AddDbContext<CatalogoDBContext>(options => options.UseNpgsql(
            connectionString,
            npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", CatalogoDBContext.SCHEMA)));

        // Mesma instância do DbContext: repositório e commit precisam do mesmo ChangeTracker.
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CatalogoDBContext>());

        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IEstoqueRepository, EstoqueRepository>();
    }

    private static void AddHandlers(IServiceCollection services)
    {
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
