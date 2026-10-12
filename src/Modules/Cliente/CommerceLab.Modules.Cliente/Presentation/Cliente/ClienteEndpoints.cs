using CommerceLab.Modules.Cliente.Application.CadastrarCliente;
using CommerceLab.Shared.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CommerceLab.Modules.Cliente.Presentation.Cliente;

internal static class ClienteEndpoints
{
    public static void Map(IEndpointRouteBuilder clientes)
    {
        clientes.MapPost("/", Cadastrar)
            .WithName("CadastrarCliente")
            .WithSummary("Cadastra um novo cliente")
            .Produces<ClienteCriadoResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> Cadastrar(CadastrarClienteRequest request, CadastrarClienteHandler handler, CancellationToken cancellationToken)
    {
        var resultado = await handler.HandleAsync(request.ParaCommand(), cancellationToken);

        return resultado.Sucesso
            ? Results.Created($"/api/cliente/clientes/{resultado.Valor!.Id}", resultado.Valor)
            : resultado.Erro!.ParaHttp();
    }
}

