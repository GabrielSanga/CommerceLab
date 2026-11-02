using CommerceLab.Shared.Results;
using Microsoft.Extensions.Logging;

namespace CommerceLab.Shared.Sagas;

public sealed class SagaOrchestrator<TContexto>
{
    private readonly IReadOnlyList<ISagaStep<TContexto>> _passos;

    public SagaOrchestrator(IEnumerable<ISagaStep<TContexto>> passos)
    {
        _passos = passos.ToArray();
    }

    public async Task<Result> ExecutarAsync(TContexto contexto, CancellationToken cancellationToken)
    {
        var executados = new Stack<ISagaStep<TContexto>>();

        foreach (var passo in _passos)
        {
            var resultado = await ExecutarPassoAsync(passo, contexto, cancellationToken);
            if (!resultado.Sucesso)
            {
                await CompensarAsync(executados, contexto);

                return resultado;
            }

            executados.Push(passo);
        }

        return Result.Ok();
    }

    private async Task<Result> ExecutarPassoAsync(ISagaStep<TContexto> passo, TContexto contexto, CancellationToken cancellationToken)
    {
        try
        {
            return await passo.ExecutarAsync(contexto, cancellationToken);
        }
        catch
        {
            return Result.Falha(Erro.Falha("SAGA_ETAPA_EXCECAO", $"Falha inesperada na etapa '{passo.Nome}'."));
        }
    }

    private async Task CompensarAsync(Stack<ISagaStep<TContexto>> executados, TContexto contexto)
    {
        while (executados.Count > 0)
        {
            var passo = executados.Pop();

            try
            {
                await passo.CompensarAsync(contexto, CancellationToken.None);
            }
            catch
            {
                // segue compensando as demais etapas.
            }
        }
    }
}
