using CommerceLab.Modules.Catalogo.Domain;

namespace CommerceLab.Modules.Catalogo.UnitTests.Domain;

public class ProdutoTests
{
    [Theory]
    [InlineData("TEC-001", "TEC-001")]
    [InlineData("tec-001", "TEC-001")]
    [InlineData(" tec-001 ", "TEC-001")]
    [InlineData("TEC", "TEC")]
    [InlineData("12345678901234567890123456789012", "12345678901234567890123456789012")]
    public void Criar_DeveNormalizarSku_QuandoSkuForValido(string sku, string skuEsperado)
    {
        var nome = "Teclado";
        var preco = 100m;

        var resultado = Produto.Criar(nome, null, sku, preco);

        Assert.True(resultado.Sucesso);
        Assert.Null(resultado.Erro);
        Assert.NotNull(resultado.Valor);
        Assert.Equal(skuEsperado, resultado.Valor.Sku);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("TE")]
    [InlineData("123456789012345678901234567890123")]
    [InlineData("-TEC-001")]
    [InlineData("TEC 001")]
    [InlineData("TEC_001")]
    [InlineData("TÉC-001")]
    public void Criar_DeveRetornarFalha_QuandoSkuForInvalido(string? sku)
    {
        var nome = "Teclado";
        var preco = 100m;

        var resultado = Produto.Criar(nome, null, sku, preco);

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Valor);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("PRODUTO_SKU_INVALIDO", resultado.Erro.Codigo);
    }

    [Fact]
    public void Criar_DeveRetornarFalha_QuandoPrecoForZero()
    {
        var nome = "Teclado";
        var sku = "TEC-001";
        var preco = 0m;

        var resultado = Produto.Criar(nome, null, sku, preco);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Erro);
        Assert.Equal("PRODUTO_PRECO_INVALIDO", resultado.Erro.Codigo);
    }
}
