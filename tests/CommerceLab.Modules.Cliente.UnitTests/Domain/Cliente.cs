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
        var resultado = ClienteEntity.Criar("João Silva", email, "12345678900");

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
        var resultado = ClienteEntity.Criar("João Silva", "cliente@email.com", documento);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("DOCUMENTO_INVALIDO", resultado.Erro.Codigo);
    }

    [Fact]
    public void Criar_DeveRetornarCliente_QuandoDadosForemValidos()
    {
        var nome = "João Silva";
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
}

