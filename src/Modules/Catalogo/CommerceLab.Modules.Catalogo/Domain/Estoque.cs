using CommerceLab.Shared.Results;

namespace CommerceLab.Modules.Catalogo.Domain;

internal sealed class Estoque
{
    private Estoque(Guid id, Guid produtoId, int quantidadeDisponivel, DateTime atualizadoEm)
    {
        Id = id;
        ProdutoId = produtoId;
        QuantidadeDisponivel = quantidadeDisponivel;
        QuantidadeReservada = 0;
        AtualizadoEm = atualizadoEm;
    }

    public Guid Id { get; }

    public Guid ProdutoId { get; }

    public int QuantidadeDisponivel { get; private set; }

    public int QuantidadeReservada { get; }

    public DateTime AtualizadoEm { get; private set; }

    public static Result<Estoque> Criar(Guid produtoId, int quantidadeInicial)
    {
        if (produtoId == Guid.Empty)
        {
            return Result<Estoque>.Falha(Erro.Validacao("ESTOQUE_PRODUTO_ID_INVALIDO", "O identificador do produto deve ser válido."));
        }

        if (quantidadeInicial < 0)
        {
            return Result<Estoque>.Falha(Erro.Validacao("ESTOQUE_QUANTIDADE_INVALIDA", "A quantidade inicial de estoque não pode ser negativa."));
        }

        var estoque = new Estoque(Guid.NewGuid(), produtoId, quantidadeInicial, DateTime.UtcNow);

        return Result<Estoque>.Ok(estoque);
    }

    public Result Debitar(int quantidade)
    {
        if (quantidade <= 0)
        {
            return Result.Falha(Erro.Validacao("ESTOQUE_DEBITO_INVALIDO", "A quantidade a baixar deve ser maior que zero."));
        }

        if (quantidade > QuantidadeDisponivel)
        {
            return Result.Falha(Erro.Conflito("ESTOQUE_INSUFICIENTE", $"Estoque insuficiente: disponível {QuantidadeDisponivel}, solicitado {quantidade}."));
        }

        QuantidadeDisponivel -= quantidade;
        AtualizadoEm = DateTime.UtcNow;

        return Result.Ok();
    }
}
