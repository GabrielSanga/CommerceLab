using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceLab.Shared.Modules;

/// <summary>
/// Contrato que todo módulo do monolito precisa implementar para se plugar no Host.
/// O Host não conhece o conteúdo do módulo: ele apenas chama estes dois métodos.
/// </summary>
public interface IModule
{
    /// <summary>Nome do módulo. Usado em log e diagnóstico.</summary>
    string Name { get; }

    /// <summary>
    /// Registra no container tudo o que o módulo precisa (DbContext, handlers, repositórios, opções). Chamado uma única vez, na composição da aplicação.
    /// </summary>
    void AddModule(IServiceCollection services, IConfiguration configuration);

    /// <summary>
    /// Expõe a API pública HTTP do módulo. Os endpoints moram dentro do módulo, não no Host.
    /// </summary>
    void MapEndpoints(IEndpointRouteBuilder endpoints);
}
