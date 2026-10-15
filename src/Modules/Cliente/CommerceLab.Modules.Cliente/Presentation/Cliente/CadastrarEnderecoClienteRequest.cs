using CommerceLab.Modules.Cliente.Application.CadastrarEnderecoCliente;

namespace CommerceLab.Modules.Cliente.Presentation.Cliente;

internal sealed record CadastrarEnderecoClienteRequest(string? Logradouro, string? Numero, string? Cidade, string? Estado, string? Cep)
{
    public CadastrarEnderecoClienteCommand ParaCommand(Guid idCliente)
    {
        return new CadastrarEnderecoClienteCommand(idCliente, Logradouro, Numero, Cidade, Estado, Cep);
    }
}
