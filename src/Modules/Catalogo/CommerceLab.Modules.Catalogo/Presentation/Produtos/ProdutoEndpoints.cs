using CommerceLab.Modules.Catalogo.Application.Produtos.CadastrarProduto;
using CommerceLab.Modules.Catalogo.Application.Produtos.ListarProdutos;
using CommerceLab.Modules.Catalogo.Application.Produtos.ObterProdutoPorId;
using CommerceLab.Shared.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CommerceLab.Modules.Catalogo.Presentation.Produtos;

internal static class ProdutoEndpoints
{
    public static void Map(IEndpointRouteBuilder produtos)
    {
        produtos.MapPost("/", Cadastrar)
            .WithName("CadastrarProduto")
            .WithSummary("Cadastra um novo produto e sua posição de estoque")
            .Produces<ProdutoCriadoResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);

        produtos.MapGet("/{id:guid}", ObterPorId)
            .WithName("ObterProdutoPorId")
            .WithSummary("Obtém um produto pelo identificador, com a posição de estoque")
            .Produces<ProdutoDetalheResult>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        produtos.MapGet("/", Listar)
            .WithName("ListarProdutos")
            .WithSummary("Lista produtos, com filtro opcional por status e por termo em nome ou SKU")
            .Produces<IReadOnlyList<ProdutoResumoResult>>();
    }

    private static async Task<IResult> Cadastrar(CadastrarProdutoRequest request, CadastrarProdutoHandler handler, CancellationToken cancellationToken)
    {
        var resultado = await handler.HandleAsync(request.ParaCommand(), cancellationToken);

        return resultado.Sucesso
            ? Results.Created($"/api/catalogo/produtos/{resultado.Valor!.Id}", resultado.Valor)
            : resultado.Erro!.ParaHttp();
    }

    private static async Task<IResult> ObterPorId(Guid id, ObterProdutoPorIdHandler handler, CancellationToken cancellationToken)
    {
        var resultado = await handler.HandleAsync(new ObterProdutoPorIdQuery(id), cancellationToken);

        return resultado.Sucesso
            ? Results.Ok(resultado.Valor)
            : resultado.Erro!.ParaHttp();
    }

    private static async Task<IResult> Listar(bool? ativo, string? termo, ListarProdutosHandler handler, CancellationToken cancellationToken)
    {
        var resultado = await handler.HandleAsync(new ListarProdutosQuery(ativo, termo), cancellationToken);

        return resultado.Sucesso
            ? Results.Ok(resultado.Valor)
            : resultado.Erro!.ParaHttp();
    }
}
