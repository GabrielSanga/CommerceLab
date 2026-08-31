using CommerceLab.Modules.Catalogo.Application.Produtos.CadastrarProduto;

namespace CommerceLab.Modules.Catalogo.Presentation.Produtos;

internal sealed record CadastrarProdutoRequest(string? Nome, string? Descricao, string? Sku, decimal Preco, int QuantidadeInicial = 0)
{
    public CadastrarProdutoCommand ParaCommand() => new(Nome, Descricao, Sku, Preco, QuantidadeInicial);
}
