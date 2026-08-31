namespace CommerceLab.Modules.Catalogo.Application.Produtos.ListarProdutos;

internal sealed record ProdutoResumoResult(Guid Id, string Sku, string Nome, decimal Preco, bool Ativo, int QuantidadeDisponivel);
