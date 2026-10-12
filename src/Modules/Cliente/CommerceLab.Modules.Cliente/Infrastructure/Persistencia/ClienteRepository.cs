using CommerceLab.Modules.Cliente.Application.Abstracoes;
using CommerceLab.Modules.Cliente.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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

            await contexto.SaveChangesAsync(cancellationToken);

            return true;
        }

    }
}
