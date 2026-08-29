using CommerceLab.Modules.Catalogo.Presentation;
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
        // Dependências internas do módulo entram aqui:
        // DbContext, repositórios, handlers de Application, options.
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var catalogo = endpoints.MapGroup("/api/catalogo").WithTags("Catalogo");

        ProdutoEndpoints.Map(catalogo.MapGroup("/produtos"));
    }
}
