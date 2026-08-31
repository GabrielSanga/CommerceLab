namespace CommerceLab.Modules.Catalogo.Application.Produtos.ObterProdutoPorId;

internal sealed record ProdutoDetalheResult(
    Guid Id,
    string Sku,
    string Nome,
    string? Descricao,
    decimal Preco,
    bool Ativo,
    DateTime CriadoEm,
    DateTime AtualizadoEm,
    int QuantidadeDisponivel,
    int QuantidadeReservada);
