using CommerceLab.Modules.Catalogo.Domain;

namespace CommerceLab.Modules.Catalogo.UnitTests.Domain;

public class EstoqueTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(int.MinValue)]
    public void Criar_DeveRetornarFalha_QuandoQuantidadeInicialForNegativa(int quantidadeInicial)
    {
        var produtoId = Guid.NewGuid();

        var resultado = Estoque.Criar(produtoId, quantidadeInicial);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("ESTOQUE_QUANTIDADE_INVALIDA", resultado.Erro.Codigo);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void Criar_DeveRetornarEstoque_QuandoQuantidadeInicialForValida(int quantidadeInicial)
    {
        var produtoId = Guid.NewGuid();

        var resultado = Estoque.Criar(produtoId, quantidadeInicial);

        Assert.True(resultado.Sucesso);
        Assert.Null(resultado.Erro);
        Assert.NotNull(resultado.Valor);
        Assert.Equal(produtoId, resultado.Valor.ProdutoId);
        Assert.Equal(quantidadeInicial, resultado.Valor.QuantidadeDisponivel);
        Assert.Equal(0, resultado.Valor.QuantidadeReservada);
    }
}
