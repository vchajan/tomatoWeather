using Microsoft.EntityFrameworkCore;

public class ForecastContext : DbContext
{
    public DbSet<ForecastEntry> ForecastEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=forecasts.db");
}
