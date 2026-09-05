using CommerceLab.Modules.Catalogo.Domain;

namespace CommerceLab.Modules.Catalogo.UnitTests.Domain;

public class ProdutoTests
{
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
