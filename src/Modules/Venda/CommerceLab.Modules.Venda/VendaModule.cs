using CommerceLab.Modules.Venda.Application.Abstracoes;
using CommerceLab.Modules.Venda.Application.CadastrarPedido;
using CommerceLab.Modules.Venda.Infrastructure.Persistencia;
using CommerceLab.Modules.Venda.Presentation.Pedido;
using CommerceLab.Shared.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceLab.Modules.Venda;

public class VendaModule : IModule
{
    public string Name => "Venda";

    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {
        AddData(services, configuration);
        AddHandlers(services);
    }

    private static void AddData(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Venda");

        services.AddDbContext<VendaDBContext>(options => options.UseNpgsql(
            connectionString,
            npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", VendaDBContext.SCHEMA)));

        // Mesma instância do DbContext: repositório e commit precisam do mesmo ChangeTracker.
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<VendaDBContext>());

        services.AddScoped<IPedidoRepository, PedidoRepository>();
    }

    private static void AddHandlers(IServiceCollection services)
    {
        services.AddScoped<CadastrarPedidoHandler>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var venda = endpoints.MapGroup("/api/venda").WithTags("Venda");

        PedidoEndpoint.Map(venda.MapGroup("/pedidos"));
    }
}
