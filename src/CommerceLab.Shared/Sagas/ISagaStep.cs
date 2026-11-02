using CommerceLab.Shared.Results;

namespace CommerceLab.Shared.Sagas;

public interface ISagaStep<in TContexto>
{
    string Nome { get; }

    Task<Result> ExecutarAsync(TContexto contexto, CancellationToken cancellationToken);

    Task CompensarAsync(TContexto contexto, CancellationToken cancellationToken);
}
