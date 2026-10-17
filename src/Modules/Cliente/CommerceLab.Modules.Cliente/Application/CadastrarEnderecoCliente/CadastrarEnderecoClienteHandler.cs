using CommerceLab.Modules.Cliente.Application.Abstracoes;
using CommerceLab.Shared.Results;

namespace CommerceLab.Modules.Cliente.Application.CadastrarEnderecoCliente;

internal class CadastrarEnderecoClienteHandler(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
{
    public async Task<Result<EnderecoClienteCriadoResult>> HandleAsync(CadastrarEnderecoClienteCommand comando, CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.ObterPorIdAsync(comando.IdCliente, cancellationToken);
        if (cliente is null)
        {
            return Result<EnderecoClienteCriadoResult>.Falha(Erro.NaoEncontrado("CLIENTE_NAO_ENCONTRADO", "Cliente não encontrado."));
        }

        var endereco = cliente.AdicionarEndereco(comando.Logradouro, comando.Numero, comando.Cidade, comando.Estado, comando.Cep);
        if (!endereco.Sucesso)
        {
            return Result<EnderecoClienteCriadoResult>.Falha(endereco.Erro!);
        }

        await unitOfWork.CommitAsync(cancellationToken);

        return Result<EnderecoClienteCriadoResult>.Ok(new EnderecoClienteCriadoResult(endereco.Valor!.Id, endereco.Valor!.Logradouro, endereco.Valor!.Numero, endereco.Valor!.Cidade, endereco.Valor!.Estado, endereco.Valor!.Cep));
    }
}
