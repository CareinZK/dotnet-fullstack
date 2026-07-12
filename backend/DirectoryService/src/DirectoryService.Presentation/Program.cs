using Microsoft.EntityFrameworkCore;
using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Application.Locations;
using FluentValidation;
using DirectoryService.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddValidatorsFromAssemblyContaining<CreateLocationDtoValidator>();
builder.Services.AddScoped<CreateLocation>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapHealthChecks("/health");

await app.RunAsync().ConfigureAwait(false);
app.UseExceptionHandler();