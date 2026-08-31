namespace CommerceLab.Shared.Results;

public enum TipoErro
{
    /// <summary>Entrada não satisfaz uma regra do domínio.</summary>
    Validacao,

    /// <summary>Recurso pedido não existe.</summary>
    NaoEncontrado,

    /// <summary>Estado atual impede a operação (ex.: identificador já usado).</summary>
    Conflito,
}
