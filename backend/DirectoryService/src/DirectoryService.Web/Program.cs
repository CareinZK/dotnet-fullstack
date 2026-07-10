using Microsoft.EntityFrameworkCore;
using DirectoryService.Infrastructure.Postgres;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapHealthChecks("/health");

await app.RunAsync().ConfigureAwait(false);