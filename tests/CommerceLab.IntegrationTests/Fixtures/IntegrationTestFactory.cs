using CommerceLab.Modules.Catalogo.Infrastructure.Persistencia;
using CommerceLab.Modules.Cliente.Infrastructure.Persistencia;
using CommerceLab.Modules.Venda.Infrastructure.Persistencia;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace CommerceLab.IntegrationTests.Fixtures;

public sealed class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("commercelab_tests")
        .WithUsername("commercelab")
        .WithPassword("commercelab")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var connectionString = _postgresContainer.GetConnectionString();

            SubstituirDbContext<CatalogoDBContext>(services, connectionString, CatalogoDBContext.SCHEMA);
            SubstituirDbContext<ClienteDBContext>(services, connectionString, ClienteDBContext.SCHEMA);
            SubstituirDbContext<VendaDBContext>(services, connectionString, VendaDBContext.SCHEMA);
        });
    }

    public async Task InitializeAsync()
    {
        // 1. Sobe o container PostgreSQL descartável para os testes de integração.
        await _postgresContainer.StartAsync();

        // 2. Força a construção do host
        _ = Services;

        // 3. Roda as migrations de todos os módulos para o banco de teste
        using var scope = Services.CreateScope();
        var services = scope.ServiceProvider;

        await services.GetRequiredService<CatalogoDBContext>().Database.MigrateAsync();
        await services.GetRequiredService<ClienteDBContext>().Database.MigrateAsync();
        await services.GetRequiredService<VendaDBContext>().Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgresContainer.DisposeAsync();
    }

    private static void SubstituirDbContext<TContext>(IServiceCollection services, string connectionString, string schema)
        where TContext : DbContext
    {
        services.RemoveAll<DbContextOptions<TContext>>();
        services.RemoveAll<IDbContextOptionsConfiguration<TContext>>();
        services.RemoveAll<TContext>();

        services.AddDbContext<TContext>(options => options.UseNpgsql(
            connectionString,
            npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", schema)));
    }
}
