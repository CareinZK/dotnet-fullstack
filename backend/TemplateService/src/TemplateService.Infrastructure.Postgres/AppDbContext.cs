using Microsoft.EntityFrameworkCore;

namespace TemplateService.Infrastructure.Postgres;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options, string connectionString)
        : base(options)
    {
        
        _connectionString = connectionString;
    }

    private readonly string _connectionString;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
}