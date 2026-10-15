using CommerceLab.Modules.Cliente.Application.Abstracoes;
using Microsoft.EntityFrameworkCore;

namespace CommerceLab.Modules.Cliente.Infrastructure.Persistencia
{
    internal class ClienteRepository(ClienteDBContext contexto) : IClienteRepository
    {
        public async Task<bool> TryAdicionarAsync(Domain.Cliente cliente, CancellationToken cancellationToken)
        {
            if (await contexto.Clientes.AnyAsync(existente => existente.Email == cliente.Email, cancellationToken))
            {
                return false;
            }

            contexto.Clientes.Add(cliente);

            return true;
        }

        public async Task<Domain.Cliente?> ObterPorIdAsync(Guid idCliente, CancellationToken cancellationToken)
        {
            return await contexto.Clientes.FirstOrDefaultAsync(cliente => cliente.Id == idCliente, cancellationToken);
        }

    }
}
