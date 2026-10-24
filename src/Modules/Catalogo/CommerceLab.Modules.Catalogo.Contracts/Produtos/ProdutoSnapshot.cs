namespace CommerceLab.Modules.Catalogo.Contracts.Produtos;

public sealed record ProdutoSnapshot(Guid Id, string Sku, string Nome, decimal Preco, bool Ativo);