using Microsoft.EntityFrameworkCore;

namespace TemplateService.Infrastructure.Postgres;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}