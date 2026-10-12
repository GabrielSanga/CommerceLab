using CommerceLab.Modules.Cliente.Application.Abstracoes;
using CommerceLab.Modules.Cliente.Application.CadastrarCliente;
using CommerceLab.Modules.Cliente.Infrastructure.Persistencia;
using CommerceLab.Modules.Cliente.Presentation.Cliente;
using CommerceLab.Shared.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceLab.Modules.Cliente
{
    public sealed class ClienteModule : IModule
    {
        public string Name => "Cliente";

        public void AddModule(IServiceCollection services, IConfiguration configuration)
        {
            AddHandlers(services);
            AddData(services, configuration);
        }

        private static void AddData(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Cliente");

            services.AddDbContext<ClienteDBContext>(options => options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", ClienteDBContext.SCHEMA)));

            services.AddScoped<IClienteRepository, ClienteRepository>();
        }

        private static void AddHandlers(IServiceCollection services)
        {
            services.AddScoped<CadastrarClienteHandler>();
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            var cliente = endpoints.MapGroup("/api/cliente").WithTags("Cliente");

            ClienteEndpoints.Map(cliente.MapGroup("/clientes"));
        }
    }
}
