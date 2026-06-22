using Microsoft.EntityFrameworkCore;
using StockPriceAPI.Stocks;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<StockPriceRecord> StockPriceRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new StockPriceRecordEntityTypeConfiguration());

        // eller denne:
        // Scans and applies all configurations found in the current assembly
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}