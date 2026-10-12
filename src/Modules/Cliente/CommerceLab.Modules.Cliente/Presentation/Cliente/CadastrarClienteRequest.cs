using CommerceLab.Modules.Cliente.Application.CadastrarCliente;

namespace CommerceLab.Modules.Cliente.Presentation.Cliente
{
    internal sealed record CadastrarClienteRequest (string? nome, string? email, string? documento)
    {
        public CadastrarClienteCommand ParaCommand()
        {
            return new CadastrarClienteCommand(nome, email, documento);
        }
    }
}
