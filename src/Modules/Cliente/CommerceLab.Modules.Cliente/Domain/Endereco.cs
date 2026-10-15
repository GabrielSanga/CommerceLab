using CommerceLab.Shared.Results;

namespace CommerceLab.Modules.Cliente.Domain;

internal sealed class Endereco
{
    public Endereco(Guid id, Guid idCliente, string logradouro, string numero, string cidade, string estado, string cep)
    {
        Id = id;
        IdCliente = idCliente;
        Logradouro = logradouro;
        Numero = numero;
        Cidade = cidade;
        Estado = estado;
        Cep = cep;
    }

    public Guid Id { get; }

    public Guid IdCliente { get; }

    public string Logradouro { get; }

    public string Numero { get; }

    public string Cidade { get; }

    public string Estado { get; }

    public string Cep { get; }

    public DateTime CriadoEm { get; } = DateTime.UtcNow;

    public static Result<Endereco> Criar(Guid idCliente, string? logradouro, string? numero, string? cidade, string? estado, string? cep)
    {
        if (string.IsNullOrWhiteSpace(logradouro))
        {
            return Result<Endereco>.Falha(Erro.Validacao("LOGRADOURO_INVALIDO", "O logradouro do endereço é obrigatório."));
        }
        if (string.IsNullOrWhiteSpace(numero))
        {
            return Result<Endereco>.Falha(Erro.Validacao("NUMERO_INVALIDO", "O número do endereço é obrigatório."));
        }
        if (string.IsNullOrWhiteSpace(cidade))
        {
            return Result<Endereco>.Falha(Erro.Validacao("CIDADE_INVALIDA", "A cidade do endereço é obrigatória."));
        }
        if (string.IsNullOrWhiteSpace(estado))
        {
            return Result<Endereco>.Falha(Erro.Validacao("ESTADO_INVALIDO", "O estado do endereço é obrigatório."));
        }
        if (string.IsNullOrWhiteSpace(cep))
        {
            return Result<Endereco>.Falha(Erro.Validacao("CEP_INVALIDO", "O CEP do endereço é obrigatório."));
        }

        var endereco = new Endereco(Guid.NewGuid(), idCliente, logradouro, numero, cidade, estado, cep);

        return Result<Endereco>.Ok(endereco);
    }

}