namespace StockPriceAPI.Stocks;

public sealed record StockPriceRecord
{
    public int Id { get; set; }
    public string Ticker { get; set; } = null!;
    public decimal Price { get; set; }
    public DateTime Timestamp { get; set; }

    public StockPriceRecord() {}
}