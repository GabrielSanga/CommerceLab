using CommerceLab.Modules.Catalogo.Contracts.Produtos;
using CommerceLab.Modules.Venda.Application.Abstracoes;
using CommerceLab.Modules.Venda.Domain;
using CommerceLab.Shared.Results;

namespace CommerceLab.Modules.Venda.Application.CadastrarPedido;

internal sealed class CadastrarPedidoHandler(
    IPedidoRepository pedidoRepository,
    IProdutoSnapshotService produtoSnapshots,
    IUnitOfWork unitOfWork)
{

    public async Task<Result<PedidoCriadoResult>> HandleAsync(CadastrarPedidoCommand comando, CancellationToken cancellationToken)
    {
        if (comando.Itens is null || comando.Itens.Count == 0)
        {
            return Result<PedidoCriadoResult>.Falha(Erro.Validacao("ITENS_VAZIOS", "A lista de itens não pode ser vazia."));
        }

        var pedidoCriado = Pedido.Criar(comando.IdCliente, comando.IdEndereco);
        if (!pedidoCriado.Sucesso)
        {
            return Result<PedidoCriadoResult>.Falha(pedidoCriado.Erro!);
        }

        var pedido = pedidoCriado.Valor!;

        foreach (var itemComando in comando.Itens)
        {
            var snapshot = await produtoSnapshots.ObterPorIdAsync(itemComando.IdProduto, cancellationToken);
            if (snapshot is null)
            {
                return Result<PedidoCriadoResult>.Falha(Erro.NaoEncontrado("PRODUTO_NAO_ENCONTRADO", $"Não existe produto com o identificador '{itemComando.IdProduto}'."));
            }

            if (!snapshot.Ativo)
            {
                return Result<PedidoCriadoResult>.Falha(Erro.Validacao("PRODUTO_INATIVO", $"O produto '{snapshot.Nome}' está inativo e não pode ser vendido."));
            }

            var itemAdicionado = pedido.AdicionarItem(snapshot.Id, snapshot.Nome, snapshot.Sku, itemComando.Quantidade, snapshot.Preco);
            if (!itemAdicionado.Sucesso)
            {
                return Result<PedidoCriadoResult>.Falha(itemAdicionado.Erro!);
            }
        }

        await pedidoRepository.AdicionarAsync(pedido, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return Result<PedidoCriadoResult>.Ok(new PedidoCriadoResult(pedido.Id, pedido.Codigo, pedido.ValorTotal, pedido.Itens.Count, pedido.DataCriacao));
    }

}
