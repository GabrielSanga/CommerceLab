using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CommerceLab.Modules.Venda.Presentation.Pedido;

internal static class PedidoEndpoint
{
    public static void Map(IEndpointRouteBuilder pedidos)
    {
        pedidos.MapPost("/", Cadastrar)
            .WithName("CadastrarPedido")
            .WithSummary("Cadastra um novo pedido")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> Cadastrar()
    {
        return Results.Created("", null);
    }

}
