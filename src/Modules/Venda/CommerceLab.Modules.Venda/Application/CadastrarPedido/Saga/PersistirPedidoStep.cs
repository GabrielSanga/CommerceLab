using CommerceLab.Modules.Venda.Application.Abstracoes;
using CommerceLab.Shared.Results;
using CommerceLab.Shared.Sagas;

namespace CommerceLab.Modules.Venda.Application.CadastrarPedido.Saga;

internal sealed class PersistirPedidoStep(IPedidoRepository pedidoRepository, IUnitOfWork unitOfWork) : ISagaStep<CadastrarPedidoContexto>
{
    public string Nome => "PersistirPedido";

    public async Task<Result> ExecutarAsync(CadastrarPedidoContexto contexto, CancellationToken cancellationToken)
    {
        await pedidoRepository.AdicionarAsync(contexto.Pedido, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Ok();
    }

    public Task CompensarAsync(CadastrarPedidoContexto contexto, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
