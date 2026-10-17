using CommerceLab.Shared.Results;

namespace CommerceLab.Modules.Cliente.Domain;

internal sealed class Cliente
{
    private List<Endereco> _enderecos = [];

    private Cliente(Guid id, string nome, string email, string documento, DateTime criadoEm)
    {
        Id = id;
        Nome = nome;
        Email = email;
        Documento = documento;
        CriadoEm = criadoEm;
    }

    public Guid Id { get; }

    public string Nome { get; }

    public string Email { get; }

    public string Documento { get; }

    public DateTime CriadoEm { get; }

    public IReadOnlyCollection<Endereco> Enderecos => _enderecos;

    public static Result<Cliente> Criar(string? nome, string? email, string? documento)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            return Result<Cliente>.Falha(Erro.Validacao("NOME_INVALIDO", "O nome do cliente é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<Cliente>.Falha(Erro.Validacao("EMAIL_INVALIDO", "O email do cliente é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(documento))
        {
            return Result<Cliente>.Falha(Erro.Validacao("DOCUMENTO_INVALIDO", "O documento do cliente é obrigatório."));
        }

        var cliente = new Cliente(Guid.NewGuid(), nome, email, documento, DateTime.UtcNow);

        return Result<Cliente>.Ok(cliente);
    }

    public Result<Endereco> AdicionarEndereco(string? logradouro, string? numero, string? cidade, string? estado, string? cep)
    {
        var enderecoCriado = Endereco.Criar(Id, logradouro, numero, cidade, estado, cep);
        if (!enderecoCriado.Sucesso)
        {
            return enderecoCriado;
        }

        _enderecos.Add(enderecoCriado.Valor!);

        return enderecoCriado;
    }
}

