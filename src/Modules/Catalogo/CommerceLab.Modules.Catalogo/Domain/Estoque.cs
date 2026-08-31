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

    public int QuantidadeDisponivel { get; }

    public int QuantidadeReservada { get; }

    public DateTime AtualizadoEm { get; }

    public static Result<Estoque> Criar(Guid produtoId, int quantidadeInicial)
    {
        if (quantidadeInicial < 0)
        {
            return Result<Estoque>.Falha(Erro.Validacao("ESTOQUE_QUANTIDADE_INVALIDA", "A quantidade inicial de estoque não pode ser negativa."));
        }

        var estoque = new Estoque(Guid.NewGuid(), produtoId, quantidadeInicial, DateTime.UtcNow);

        return Result<Estoque>.Ok(estoque);
    }
}
