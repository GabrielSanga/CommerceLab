namespace CommerceLab.Shared.Results;

public readonly struct Result<T>
{
    private Result(bool sucesso, T? valor, Erro? erro)
    {
        Sucesso = sucesso;
        Valor = valor;
        Erro = erro;
    }

    public bool Sucesso { get; }

    public T? Valor { get; }

    public Erro? Erro { get; }

    public static Result<T> Ok(T valor) => new(true, valor, null);

    public static Result<T> Falha(Erro erro) => new(false, default, erro);
}
