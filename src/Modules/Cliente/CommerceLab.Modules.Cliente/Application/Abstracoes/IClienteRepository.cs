using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceLab.Modules.Cliente.Application.Abstracoes
{
    internal interface IClienteRepository
    {

        Task<bool> TryAdicionarAsync(Domain.Cliente cliente, CancellationToken cancellationToken);

    }
}
