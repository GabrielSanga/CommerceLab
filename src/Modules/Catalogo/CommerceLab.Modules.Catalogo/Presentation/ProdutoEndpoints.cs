using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CommerceLab.Modules.Catalogo.Presentation
{
    internal static class ProdutoEndpoints
    {
        public static void Map(IEndpointRouteBuilder produtos)
        {
            produtos.MapGet("/", Listar);
        }

        private static IResult Listar()
        {
            return Results.Ok(new[] { "Listando produtos..." });
        }
    }
}
