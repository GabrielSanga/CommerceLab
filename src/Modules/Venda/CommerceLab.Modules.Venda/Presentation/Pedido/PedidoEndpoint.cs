using CommerceLab.Modules.Venda.Application.CadastrarPedido;
using CommerceLab.Shared.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CommerceLab.Modules.Venda.Presentation.Pedido;

internal static class PedidoEndpoint
{
    public static void Map(IEndpointRouteBuilder pedidos)
    {
        pedidos.MapPost("/", Cadastrar).WithName("CadastrarPedido");
    }

    private static async Task<IResult> Cadastrar(CadastrarPedidoRequest request, CadastrarPedidoHandler handler, CancellationToken cancellationToken)
    {
        var resultado = await handler.HandleAsync(request.ParaCommand(), cancellationToken);

        if (!resultado.Sucesso)
            return resultado.Erro!.ParaHttp();

        var pedido = resultado.Valor!;

        return Results.Created($"/api/venda/pedidos/{pedido.Id}", pedido);
    }

}
