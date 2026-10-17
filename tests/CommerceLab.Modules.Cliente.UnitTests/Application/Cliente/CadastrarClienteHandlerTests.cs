using CommerceLab.Modules.Cliente.Application.Abstracoes;
using CommerceLab.Modules.Cliente.Application.CadastrarCliente;
using CommerceLab.Shared.Results;
using ClienteDomain = CommerceLab.Modules.Cliente.Domain.Cliente;

namespace CommerceLab.Modules.Cliente.UnitTests.Application.Cliente;

public class CadastrarClienteHandlerTests
{
    [Theory]
    [InlineData(null, "cliente@email.com", "12345678900", "NOME_INVALIDO")]
    [InlineData("", "cliente@email.com", "12345678900", "NOME_INVALIDO")]
    [InlineData("   ", "cliente@email.com", "12345678900", "NOME_INVALIDO")]
    [InlineData("João Silva", null, "12345678900", "EMAIL_INVALIDO")]
    [InlineData("João Silva", "   ", "12345678900", "EMAIL_INVALIDO")]
    [InlineData("João Silva", "cliente@email.com", null, "DOCUMENTO_INVALIDO")]
    [InlineData("João Silva", "cliente@email.com", "   ", "DOCUMENTO_INVALIDO")]
    public async Task HandleAsync_DeveRetornarFalhaSemPersistir_QuandoClienteForInvalido(string? nome, string? email, string? documento, string codigoEsperado)
    {
        var clientes = new ClienteRepositorySpy();
        var unitOfWork = new UnitOfWorkSpy();
        var handler = new CadastrarClienteHandler(clientes, unitOfWork);
        var comando = new CadastrarClienteCommand(nome, email, documento);

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal(codigoEsperado, resultado.Erro.Codigo);
        Assert.Equal(0, clientes.QuantidadeTentativasAdicionar);
        Assert.Equal(0, unitOfWork.QuantidadeCommits);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarConflitoSemPersistir_QuandoEmailJaExistir()
    {
        var clientes = new ClienteRepositorySpy(resultadoAdicionar: false);
        var unitOfWork = new UnitOfWorkSpy();
        var handler = new CadastrarClienteHandler(clientes, unitOfWork);
        var comando = new CadastrarClienteCommand("João Silva", "cliente@email.com", "12345678900");

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("EMAIL_JA_EXISTENTE", resultado.Erro.Codigo);
        Assert.Equal(TipoErro.Conflito, resultado.Erro.Tipo);
        Assert.Equal(1, clientes.QuantidadeTentativasAdicionar);
        Assert.Equal(0, unitOfWork.QuantidadeCommits);
    }

    [Fact]
    public async Task HandleAsync_DevePersistirCliente_QuandoComandoForValido()
    {
        var clientes = new ClienteRepositorySpy();
        var unitOfWork = new UnitOfWorkSpy();
        var handler = new CadastrarClienteHandler(clientes, unitOfWork);
        var comando = new CadastrarClienteCommand("João Silva", "cliente@email.com", "12345678900");

        var resultado = await handler.HandleAsync(comando, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.Null(resultado.Erro);
        Assert.NotNull(resultado.Valor);
        Assert.Equal(1, clientes.QuantidadeTentativasAdicionar);
        Assert.Equal(1, unitOfWork.QuantidadeCommits);

        var clienteAdicionado = Assert.IsType<ClienteDomain>(clientes.ClienteAdicionado);

        Assert.NotEqual(Guid.Empty, clienteAdicionado.Id);
        Assert.Equal(clienteAdicionado.Id, resultado.Valor.Id);
        Assert.Equal(clienteAdicionado.Nome, resultado.Valor.Nome);
        Assert.Equal(clienteAdicionado.Email, resultado.Valor.Email);
        Assert.Equal(clienteAdicionado.Documento, resultado.Valor.Documento);
        Assert.Equal(clienteAdicionado.CriadoEm, resultado.Valor.CriadoEm);
    }

    private sealed class UnitOfWorkSpy : IUnitOfWork
    {
        public int QuantidadeCommits { get; private set; }

        public Task CommitAsync(CancellationToken cancellationToken)
        {
            QuantidadeCommits++;
            return Task.CompletedTask;
        }
    }

    private sealed class ClienteRepositorySpy(bool resultadoAdicionar = true) : IClienteRepository
    {
        public int QuantidadeTentativasAdicionar { get; private set; }

        public ClienteDomain? ClienteAdicionado { get; private set; }

        public Task<bool> TryAdicionarAsync(ClienteDomain cliente, CancellationToken cancellationToken)
        {
            QuantidadeTentativasAdicionar++;
            ClienteAdicionado = cliente;
            return Task.FromResult(resultadoAdicionar);
        }

        public Task<ClienteDomain?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }
}
