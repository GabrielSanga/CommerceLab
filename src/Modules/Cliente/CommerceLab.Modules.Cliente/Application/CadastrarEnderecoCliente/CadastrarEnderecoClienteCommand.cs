namespace CommerceLab.Modules.Cliente.Application.CadastrarEnderecoCliente;

internal sealed record CadastrarEnderecoClienteCommand(Guid IdCliente, string? Logradouro, string? Numero, string? Cidade, string? Estado, string? Cep)
{
}