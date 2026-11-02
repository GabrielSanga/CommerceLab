using System.Net;
using System.Net.Http.Json;
using CommerceLab.IntegrationTests.Fixtures;
using CommerceLab.Modules.Cliente.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceLab.IntegrationTests.Clientes;

[Collection("integration")]
public sealed class CadastrarClienteTests(IntegrationTestFactory factory)
{
    private readonly IntegrationTestFactory _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Cadastrar_DevePersistirCliente_QuandoDadosForemValidos()
    {
        var email = $"integracao-{Guid.NewGuid():N}@commercelab.test";
        var documento = Guid.NewGuid().ToString("N");
        var request = new CadastrarClienteRequest("Cliente de Integração", email, documento);
        var criadoAntesDe = DateTime.UtcNow;

        var response = await _client.PostAsJsonAsync("/api/cliente/clientes", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var criado = await response.Content.ReadFromJsonAsync<ClienteCriadoResponse>();

        Assert.NotNull(criado);
        Assert.NotEqual(Guid.Empty, criado.Id);
        Assert.Equal(request.Nome, criado.Nome);
        Assert.Equal(email, criado.Email);
        Assert.Equal(documento, criado.Documento);
        Assert.InRange(criado.CriadoEm, criadoAntesDe.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
        Assert.NotNull(response.Headers.Location);
        Assert.EndsWith($"/api/cliente/clientes/{criado.Id}", response.Headers.Location.ToString());

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ClienteDBContext>();

        var persistido = await db.Clientes.AsNoTracking().SingleOrDefaultAsync(cliente => cliente.Id == criado.Id);

        Assert.NotNull(persistido);
        Assert.Equal(criado.Id, persistido.Id);
        Assert.Equal(criado.Nome, persistido.Nome);
        Assert.Equal(criado.Email, persistido.Email);
        Assert.Equal(criado.Documento, persistido.Documento);
    }

    private sealed record CadastrarClienteRequest(string Nome, string Email, string Documento);

    private sealed record ClienteCriadoResponse(Guid Id, string Nome, string Email, string Documento, DateTime CriadoEm);
}
