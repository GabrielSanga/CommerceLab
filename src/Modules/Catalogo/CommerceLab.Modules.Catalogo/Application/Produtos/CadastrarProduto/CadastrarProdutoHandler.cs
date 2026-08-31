using CommerceLab.Modules.Catalogo.Application.Abstracoes;
using CommerceLab.Modules.Catalogo.Domain;
using CommerceLab.Shared.Results;

namespace CommerceLab.Modules.Catalogo.Application.Produtos.CadastrarProduto;

internal sealed class CadastrarProdutoHandler(IProdutoRepository produtos, IEstoqueRepository estoques)
{
    public async Task<Result<ProdutoCriadoResult>> HandleAsync(CadastrarProdutoCommand comando, CancellationToken cancellationToken)
    {
        var produtoCriado = Produto.Criar(comando.Nome, comando.Descricao, comando.Sku, comando.Preco);
        if (!produtoCriado.Sucesso)
        {
            return Result<ProdutoCriadoResult>.Falha(produtoCriado.Erro!);
        }

        var produto = produtoCriado.Valor!;

        var estoqueCriado = Estoque.Criar(produto.Id, comando.QuantidadeInicial);
        if (!estoqueCriado.Sucesso)
        {
            return Result<ProdutoCriadoResult>.Falha(estoqueCriado.Erro!);
        }

        var estoque = estoqueCriado.Valor!;

        if (!await produtos.TryAdicionarAsync(produto, cancellationToken))
        {
            return Result<ProdutoCriadoResult>.Falha(Erro.Conflito("PRODUTO_SKU_DUPLICADO", $"Já existe um produto com o SKU '{produto.Sku}'."));
        }

        await estoques.AdicionarAsync(estoque, cancellationToken);

        return Result<ProdutoCriadoResult>.Ok(new ProdutoCriadoResult(produto.Id, produto.Sku, produto.Nome, produto.Preco, produto.Ativo, produto.CriadoEm, estoque.QuantidadeDisponivel));
    }
}
