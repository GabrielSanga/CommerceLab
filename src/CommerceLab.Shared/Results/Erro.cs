namespace CommerceLab.Shared.Results;

public sealed record Erro(string Codigo, string Mensagem, TipoErro Tipo)
{
    public static Erro Validacao(string codigo, string mensagem) =>
        new(codigo, mensagem, TipoErro.Validacao);

    public static Erro NaoEncontrado(string codigo, string mensagem) =>
        new(codigo, mensagem, TipoErro.NaoEncontrado);

    public static Erro Conflito(string codigo, string mensagem) =>
        new(codigo, mensagem, TipoErro.Conflito);
}
