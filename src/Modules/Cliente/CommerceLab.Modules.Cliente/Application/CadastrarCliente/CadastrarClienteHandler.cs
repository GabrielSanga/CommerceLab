using CommerceLab.Modules.Cliente.Application.Abstracoes;
using CommerceLab.Shared.Results;

namespace CommerceLab.Modules.Cliente.Application.CadastrarCliente
{
    internal class CadastrarClienteHandler(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
    {

        public async Task<Result<ClienteCriadoResult>> HandleAsync(CadastrarClienteCommand comando, CancellationToken cancellationToken)
        {
            var clienteCriado = Domain.Cliente.Criar(comando.nome, comando.email, comando.documento);

            if (!clienteCriado.Sucesso)
            {
                return Result<ClienteCriadoResult>.Falha(clienteCriado.Erro!);
            }

            var cliente = clienteCriado.Valor!;

            if (!await clienteRepository.TryAdicionarAsync(cliente, cancellationToken))
            {
                return Result<ClienteCriadoResult>.Falha(Erro.Conflito("EMAIL_JA_EXISTENTE", $"Já existe um cliente com o email '{cliente.Email}'!"));
            }

            await unitOfWork.CommitAsync(cancellationToken);

            var clienteRetorno = new ClienteCriadoResult(cliente.Id, cliente.Nome, cliente.Email, cliente.Documento, cliente.CriadoEm);

            return Result<ClienteCriadoResult>.Ok(clienteRetorno);
        }

    }
}
