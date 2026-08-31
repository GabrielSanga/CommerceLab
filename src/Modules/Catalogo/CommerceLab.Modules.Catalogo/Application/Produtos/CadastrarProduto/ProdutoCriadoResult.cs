namespace CommerceLab.Modules.Catalogo.Application.Produtos.CadastrarProduto;

internal sealed record ProdutoCriadoResult(Guid Id, string Sku, string Nome, decimal Preco, bool Ativo, DateTime CriadoEm, int QuantidadeDisponivel);
