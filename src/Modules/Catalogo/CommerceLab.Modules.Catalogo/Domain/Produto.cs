using System.Text.RegularExpressions;
using CommerceLab.Shared.Results;

namespace CommerceLab.Modules.Catalogo.Domain;

internal sealed partial class Produto
{
    private const int NOMETAMANHOMINIMO = 3;
    private const int NOMETAMANHOMAXIMO = 200;
    private const int DESCRICAOTAMANHOMAXIMO = 2000;

    private Produto(Guid id, string nome, string? descricao, string sku, decimal preco, DateTime criadoEm)
    {
        Id = id;
        Nome = nome;
        Descricao = descricao;
        Sku = sku;
        Preco = preco;
        Ativo = true;
        CriadoEm = criadoEm;
        AtualizadoEm = criadoEm;
    }

    public Guid Id { get; }

    public string Nome { get; }

    public string? Descricao { get; }

    /// <summary>Sempre normalizado: sem espaços nas pontas e em caixa alta.</summary>
    public string Sku { get; }

    public decimal Preco { get; }

    public bool Ativo { get; }

    public DateTime CriadoEm { get; }

    public DateTime AtualizadoEm { get; }

    public static Result<Produto> Criar(string? nome, string? descricao, string? sku, decimal preco)
    {
        var nomeNormalizado = nome?.Trim() ?? string.Empty;
        if (nomeNormalizado.Length is < NOMETAMANHOMINIMO or > NOMETAMANHOMAXIMO)
        {
            return Result<Produto>.Falha(Erro.Validacao("PRODUTO_NOME_INVALIDO", $"O nome do produto deve ter entre {NOMETAMANHOMINIMO} e {NOMETAMANHOMAXIMO} caracteres."));
        }

        var descricaoNormalizada = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        if (descricaoNormalizada is { Length: > DESCRICAOTAMANHOMAXIMO })
        {
            return Result<Produto>.Falha(Erro.Validacao("PRODUTO_DESCRICAO_INVALIDA", $"A descrição do produto não pode passar de {DESCRICAOTAMANHOMAXIMO} caracteres."));
        }

        var skuNormalizado = (sku ?? string.Empty).Trim().ToUpperInvariant();
        if (!SkuFormato().IsMatch(skuNormalizado))
        {
            return Result<Produto>.Falha(Erro.Validacao("PRODUTO_SKU_INVALIDO", "O SKU deve ter de 3 a 32 caracteres, usando apenas letras, números e hífen, e começar por letra ou número."));
        }

        if (preco <= 0m || decimal.Round(preco, 2) != preco)
        {
            return Result<Produto>.Falha(Erro.Validacao("PRODUTO_PRECO_INVALIDO", "O preço deve ser maior que zero e ter no máximo duas casas decimais."));
        }

        var produto = new Produto(Guid.NewGuid(), nomeNormalizado, descricaoNormalizada, skuNormalizado, preco, DateTime.UtcNow);

        return Result<Produto>.Ok(produto);
    }

    [GeneratedRegex("^[A-Z0-9][A-Z0-9-]{2,31}$")]
    private static partial Regex SkuFormato();
}
