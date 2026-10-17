using CommerceLab.Modules.Cliente.Application.Abstracoes;
using CommerceLab.Modules.Cliente.Application.CadastrarEnderecoCliente;
using CommerceLab.Shared.Results;
using ClienteDomain = CommerceLab.Modules.Cliente.Domain.Cliente;

namespace CommerceLab.Modules.Cliente.UnitTests.Application.Cliente;

public class CadastrarEnderecoClienteHandlerTests
{
    [Fact]
    public async Task HandleAsync_DeveRetornarNaoEncontradoSemCommit_QuandoClienteNaoExistir()
    {
        var clientes = new ClienteRepositorySpy(cliente: null);
        var unitOfWork = new UnitOfWorkSpy();
        var handler = new CadastrarEnderecoClienteHandler(clientes, unitOfWork);
        var comando = new CadastrarEnderecoClienteCommand(Guid.NewGuid(), "Rua A", "123", "São Paulo", "SP", "01001-000");

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("CLIENTE_NAO_ENCONTRADO", resultado.Erro.Codigo);
        Assert.Equal(TipoErro.NaoEncontrado, resultado.Erro.Tipo);
        Assert.Equal(0, unitOfWork.QuantidadeCommits);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarFalhaSemCommit_QuandoEnderecoForInvalido()
    {
        var cliente = CriarClienteValido();
        var clientes = new ClienteRepositorySpy(cliente);
        var unitOfWork = new UnitOfWorkSpy();
        var handler = new CadastrarEnderecoClienteHandler(clientes, unitOfWork);
        var comando = new CadastrarEnderecoClienteCommand(cliente.Id, "Rua A", "123", "São Paulo", "SP", "   ");

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("CEP_INVALIDO", resultado.Erro.Codigo);
        Assert.Empty(cliente.Enderecos);
        Assert.Equal(0, unitOfWork.QuantidadeCommits);
    }

    [Fact]
    public async Task HandleAsync_DevePersistirEndereco_QuandoComandoForValido()
    {
        var cliente = CriarClienteValido();
        var clientes = new ClienteRepositorySpy(cliente);
        var unitOfWork = new UnitOfWorkSpy();
        var handler = new CadastrarEnderecoClienteHandler(clientes, unitOfWork);
        var comando = new CadastrarEnderecoClienteCommand(cliente.Id, "Rua A", "123", "São Paulo", "SP", "01001-000");

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.Null(resultado.Erro);
        Assert.NotNull(resultado.Valor);
        Assert.Equal(1, unitOfWork.QuantidadeCommits);

        var endereco = Assert.Single(cliente.Enderecos);
        Assert.Equal(endereco.Id, resultado.Valor.Id);
        Assert.Equal(endereco.Logradouro, resultado.Valor.Logradouro);
        Assert.Equal(endereco.Numero, resultado.Valor.Numero);
        Assert.Equal(endereco.Cidade, resultado.Valor.Cidade);
        Assert.Equal(endereco.Estado, resultado.Valor.Estado);
        Assert.Equal(endereco.Cep, resultado.Valor.Cep);
    }

    private static ClienteDomain CriarClienteValido() =>
        ClienteDomain.Criar("João Silva", "cliente@email.com", "12345678900").Valor!;

    private sealed class UnitOfWorkSpy : IUnitOfWork
    {
        public int QuantidadeCommits { get; private set; }

        public Task CommitAsync(CancellationToken cancellationToken)
        {
            QuantidadeCommits++;
            return Task.CompletedTask;
        }
    }

    private sealed class ClienteRepositorySpy(ClienteDomain? cliente) : IClienteRepository
    {
        public Task<bool> TryAdicionarAsync(ClienteDomain cliente, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<ClienteDomain?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(cliente);
    }
}
