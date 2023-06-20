using Microsoft.EntityFrameworkCore;

namespace Supreme.Infrastructure.Db;

public sealed class SupremeContext : DbContext
{
    public SupremeContext(
        DbContextOptions<SupremeContext> options
    ) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
