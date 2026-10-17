using ClienteEntity = CommerceLab.Modules.Cliente.Domain.Cliente;

namespace CommerceLab.Modules.Cliente.UnitTests.Domain;

public class ClienteTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_DeveRetornarFalha_QuandoNomeForInvalido(string? nome)
    {
        var resultado = ClienteEntity.Criar(nome, "cliente@email.com", "12345678900");

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("NOME_INVALIDO", resultado.Erro.Codigo);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_DeveRetornarFalha_QuandoEmailForInvalido(string? email)
    {
        var resultado = ClienteEntity.Criar("Jo�o Silva", email, "12345678900");

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("EMAIL_INVALIDO", resultado.Erro.Codigo);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_DeveRetornarFalha_QuandoDocumentoForInvalido(string? documento)
    {
        var resultado = ClienteEntity.Criar("Jo�o Silva", "cliente@email.com", documento);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("DOCUMENTO_INVALIDO", resultado.Erro.Codigo);
    }

    [Fact]
    public void Criar_DeveRetornarCliente_QuandoDadosForemValidos()
    {
        var nome = "Jo�o Silva";
        var email = "cliente@email.com";
        var documento = "12345678900";
        var antes = DateTime.UtcNow;

        var resultado = ClienteEntity.Criar(nome, email, documento);

        Assert.True(resultado.Sucesso);
        Assert.Null(resultado.Erro);
        Assert.NotNull(resultado.Valor);
        Assert.NotEqual(Guid.Empty, resultado.Valor.Id);
        Assert.Equal(nome, resultado.Valor.Nome);
        Assert.Equal(email, resultado.Valor.Email);
        Assert.Equal(documento, resultado.Valor.Documento);
        Assert.InRange(resultado.Valor.CriadoEm, antes, DateTime.UtcNow);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AdicionarEndereco_DeveRetornarFalhaSemAdicionar_QuandoLogradouroForInvalido(string? logradouro)
    {
        var cliente = CriarClienteValido();

        var resultado = cliente.AdicionarEndereco(logradouro, "123", "São Paulo", "SP", "01001-000");

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("LOGRADOURO_INVALIDO", resultado.Erro.Codigo);
        Assert.Empty(cliente.Enderecos);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AdicionarEndereco_DeveRetornarFalhaSemAdicionar_QuandoNumeroForInvalido(string? numero)
    {
        var cliente = CriarClienteValido();

        var resultado = cliente.AdicionarEndereco("Rua A", numero, "São Paulo", "SP", "01001-000");

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("NUMERO_INVALIDO", resultado.Erro.Codigo);
        Assert.Empty(cliente.Enderecos);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AdicionarEndereco_DeveRetornarFalhaSemAdicionar_QuandoCidadeForInvalida(string? cidade)
    {
        var cliente = CriarClienteValido();

        var resultado = cliente.AdicionarEndereco("Rua A", "123", cidade, "SP", "01001-000");

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("CIDADE_INVALIDA", resultado.Erro.Codigo);
        Assert.Empty(cliente.Enderecos);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AdicionarEndereco_DeveRetornarFalhaSemAdicionar_QuandoEstadoForInvalido(string? estado)
    {
        var cliente = CriarClienteValido();

        var resultado = cliente.AdicionarEndereco("Rua A", "123", "São Paulo", estado, "01001-000");

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("ESTADO_INVALIDO", resultado.Erro.Codigo);
        Assert.Empty(cliente.Enderecos);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AdicionarEndereco_DeveRetornarFalhaSemAdicionar_QuandoCepForInvalido(string? cep)
    {
        var cliente = CriarClienteValido();

        var resultado = cliente.AdicionarEndereco("Rua A", "123", "São Paulo", "SP", cep);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("CEP_INVALIDO", resultado.Erro.Codigo);
        Assert.Empty(cliente.Enderecos);
    }

    [Fact]
    public void AdicionarEndereco_DeveAdicionarNaColecao_QuandoDadosForemValidos()
    {
        var cliente = CriarClienteValido();

        var resultado = cliente.AdicionarEndereco("Rua A", "123", "São Paulo", "SP", "01001-000");

        Assert.True(resultado.Sucesso);
        Assert.Null(resultado.Erro);
        Assert.NotNull(resultado.Valor);

        var endereco = Assert.Single(cliente.Enderecos);
        Assert.Equal(resultado.Valor.Id, endereco.Id);
        Assert.Equal(cliente.Id, endereco.IdCliente);
        Assert.Equal("Rua A", endereco.Logradouro);
        Assert.Equal("123", endereco.Numero);
        Assert.Equal("São Paulo", endereco.Cidade);
        Assert.Equal("SP", endereco.Estado);
        Assert.Equal("01001-000", endereco.Cep);
    }

    private static ClienteEntity CriarClienteValido() =>
        ClienteEntity.Criar("João Silva", "cliente@email.com", "12345678900").Valor!;
}

