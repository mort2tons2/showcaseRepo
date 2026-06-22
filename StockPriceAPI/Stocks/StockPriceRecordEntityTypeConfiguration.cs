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
    }
}
