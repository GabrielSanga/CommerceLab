using CommerceLab.Modules.Cliente.Application.CadastrarCliente;
using CommerceLab.Modules.Cliente.Application.CadastrarEnderecoCliente;
using CommerceLab.Modules.Cliente.Domain;
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

        clientes.MapPost($"/{{idCliente}}/enderecos", CadastrarEndereco)
            .WithName("CadastrarEndereco")
            .WithSummary("Cadastra um novo endereço")
            .Produces<Endereco>(StatusCodes.Status201Created)
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

    private static async Task<IResult> CadastrarEndereco(Guid idCliente, CadastrarEnderecoClienteRequest request, CadastrarEnderecoClienteHandler handler, CancellationToken cancellationToken)
    {
        var resultado = await handler.HandleAsync(request.ParaCommand(idCliente), cancellationToken);

        return resultado.Sucesso
            ? Results.Created($"/api/cliente/enderecos/{resultado.Valor!.Id}", resultado.Valor)
            : resultado.Erro!.ParaHttp();
    }

}

