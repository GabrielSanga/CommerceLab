namespace CommerceLab.Modules.Cliente.Application.CadastrarEnderecoCliente;

internal record EnderecoClienteCriadoResult(Guid Id, string Logradouro, string Numero, string Cidade, string Estado, string Cep)
{
}