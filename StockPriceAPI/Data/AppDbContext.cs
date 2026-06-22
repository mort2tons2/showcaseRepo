using Microsoft.EntityFrameworkCore;
using StockPriceAPI.Stocks;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<StockPriceRecord> StockPriceRecords { get; set; }
    //public DbSet<StockPriceRecord> StockPriceRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new StockPriceRecordEntityTypeConfiguration());

        // Scans and applies all configurations found in the current assembly
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    /*protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<StockPriceRecord>(entity =>
        {
            // Table mapping
            entity.ToTable("stock_prices", "public");

            // Primary Key (SERIAL)
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)

                .HasColumnName("Id")
                .UseIdentityByDefaultColumn();

            // Ticker Column
            entity.Property(e => e.Ticker)
                .IsRequired()
                .HasColumnName("Ticker")
                .HasColumnType("varchar(10)");

            // Price Column
            entity.Property(e => e.Price)
                .IsRequired()
                .HasColumnName("Price")
                .HasPrecision(12, 6); // Equivalent to NUMERIC(12, 6)

            // Timestamp Column
            entity.Property(e => e.Timestamp)
                .HasColumnName("Timestamp")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("(NOW() AT TIME ZONE 'UTC')");

            // Indexes
            entity.HasIndex(e => e.Ticker, "idx_stock_prices_ticker");
            entity.HasIndex(e => e.Timestamp, "idx_stock_prices_timestamp");
        });

        

        
    }*/
}

/*builder.Entity<StockPriceRecord>(entity =>
        {

        });*/

/*var stockPriceRecord = builder.Entity<StockPriceRecord>();

        stockPriceRecord.HasKey(x => x.Id);*/