namespace CommerceLab.Modules.Catalogo.Application.Produtos.CadastrarProduto;

internal sealed record CadastrarProdutoCommand(string? Nome, string? Descricao, string? Sku, decimal Preco, int QuantidadeInicial);
