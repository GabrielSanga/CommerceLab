using CommerceLab.Shared.Modules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.AddModules(builder.Configuration);

var app = builder.Build();

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
