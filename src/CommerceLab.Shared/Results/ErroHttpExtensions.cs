using Microsoft.AspNetCore.Http;

namespace CommerceLab.Shared.Results;

/// <summary>
/// Tradução única de <see cref="Erro"/> para resposta HTTP.
/// </summary>
public static class ErroHttpExtensions
{
    public static IResult ParaHttp(this Erro erro) => Microsoft.AspNetCore.Http.Results.Problem(
        title: erro.Codigo,
        detail: erro.Mensagem,
        statusCode: erro.Tipo switch
        {
            TipoErro.Validacao => StatusCodes.Status400BadRequest,
            TipoErro.NaoEncontrado => StatusCodes.Status404NotFound,
            TipoErro.Conflito => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError,
        });
}
