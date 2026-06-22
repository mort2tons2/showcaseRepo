using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StockPriceAPI.Stocks
{
    public class StockPriceRecordEntityTypeConfiguration : IEntityTypeConfiguration<StockPriceRecord>
    {
        public void Configure(EntityTypeBuilder<StockPriceRecord> builder)
        {
            builder.ToTable("stock_price_record", "public");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("Id");
                //.UseIdentityByDefaultColumn();

            builder.Property(e => e.Ticker)
                .IsRequired()
                .HasColumnName("Ticker")
                .HasColumnType("varchar(10)");

            builder.Property(e => e.Price)
                .IsRequired()
                .HasColumnName("Price")
                .HasPrecision(12, 6); // Equivalent to NUMERIC(12, 6)

            builder.Property(e => e.Timestamp)
                .HasColumnName("Timestamp")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("(NOW() AT TIME ZONE 'UTC')");

            builder.HasIndex(e => e.Ticker, "idx_stock_prices_ticker");
            builder.HasIndex(e => e.Timestamp, "idx_stock_prices_timestamp");
        }

       /* public void (ModelBuilder builder)
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
                entity.HasIndex(e => e.Timestamp, "idx_stock_prices_timestamp");*/

    

            /*builder.Entity<StockPriceRecord>(entity =>
            {

            });*/

            /*var stockPriceRecord = builder.Entity<StockPriceRecord>();

            stockPriceRecord.HasKey(x => x.Id);*/
        //}
    }
}
