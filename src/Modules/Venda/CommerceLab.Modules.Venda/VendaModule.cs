using CommerceLab.Modules.Venda.Presentation.Pedido;
using CommerceLab.Shared.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceLab.Modules.Venda;

public class VendaModule : IModule
{
    public string Name => "Venda";

    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var venda = endpoints.MapGroup("/api/venda").WithTags("Venda");

        PedidoEndpoint.Map(venda.MapGroup("/pedidos"));
    }
}