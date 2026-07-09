using TemplateService.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapHealthChecks("/health");
await app.RunAsync().ConfigureAwait(false);