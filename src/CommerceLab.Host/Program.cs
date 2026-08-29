using CommerceLab.Shared.Modules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddOpenApi();

builder.Services.AddModules(builder.Configuration);

var app = builder.Build();

app.Logger.LogInformation("Módulos carregados: {Modulos}",  string.Join(", ", app.Services.GetServices<IModule>().Select(module => module.Name)));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CommerceLab API");
    });
}

app.UseHttpsRedirection();

app.MapModules();

app.Run();
