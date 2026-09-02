using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceLab.Shared.Modules;

/// <summary>
/// Contrato que todo módulo do monolito precisa implementar para se plugar no Host.
/// </summary>
public interface IModule
{

    string Name { get; }

    /// <summary>
    /// Registra no container tudo o que o módulo precisa.
    /// </summary>
    void AddModule(IServiceCollection services, IConfiguration configuration);

    /// <summary>
    /// Expõe a API pública HTTP do módulo. Os endpoints moram dentro do módulo, não no Host.
    /// </summary>
    void MapEndpoints(IEndpointRouteBuilder endpoints);
}
