using Microsoft.EntityFrameworkCore;
using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Application.Locations;
using FluentValidation;
using DirectoryService.Presentation;
using Npgsql;
using System.Data;
using DirectoryService.Infrastructure.Postgres.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
builder.Services.AddScoped<IDbConnection>(_ =>
{
    var connection = new NpgsqlConnection(builder.Configuration.GetConnectionString("Postgres"));
    connection.Open();
    return connection;
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateLocationDtoValidator>();
builder.Services.AddScoped<CreateLocation>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/", () => "Hello World!");
app.MapHealthChecks("/health");
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.RunAsync().ConfigureAwait(false);
