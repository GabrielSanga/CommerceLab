namespace CommerceLab.Modules.Cliente.Application.CadastrarCliente
{
    internal sealed record CadastrarClienteCommand (string? nome, string? email, string? documento)
    {
    }
}
