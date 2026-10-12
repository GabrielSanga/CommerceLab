namespace CommerceLab.Modules.Cliente.Application.CadastrarCliente
{
    internal sealed record ClienteCriadoResult(Guid Id, string Nome, string Email, string Documento, DateTime CriadoEm);
}
