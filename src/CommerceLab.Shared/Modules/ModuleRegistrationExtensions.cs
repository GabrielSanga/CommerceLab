using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceLab.Shared.Modules;

/// <summary>
/// Ponto de entrada usado pelo Host (composition root) para plugar os módulos.
/// </summary>
public static class ModuleRegistrationExtensions
{
    /// <summary>
    /// Descobre os módulos e deixa cada um registrar as próprias dependências.
    /// </summary>
    public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration configuration)
    {
        foreach (var module in ModuleDiscovery.Discover())
        {
            // O próprio módulo fica no container para que o Host consiga enumerá-lo depois (endpoints, log, health checks).
            services.AddSingleton<IModule>(module);

            module.AddModule(services, configuration);
        }

        return services;
    }

    /// <summary>
    /// Mapeia os endpoints de cada módulo registrado por <see cref="AddModules"/>.
    /// </summary>
    public static IEndpointRouteBuilder MapModules(this IEndpointRouteBuilder endpoints)
    {
        foreach (var module in endpoints.ServiceProvider.GetServices<IModule>())
        {
            module.MapEndpoints(endpoints);
        }

        return endpoints;
    }
}
