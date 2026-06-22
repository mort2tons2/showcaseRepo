namespace StockPriceAPI.Stocks;

public class StockPriceResponse
{
    public int Id { get; set; }
    public string Ticker { get; set; } = null!;
    public decimal Price { get; set; }
    public DateTime Timestamp { get; set; }

    public StockPriceResponse(int id, string ticker, decimal price, DateTime timestamp)
    {
        Id = id;
        Ticker = ticker;
        Price = price;
        Timestamp = timestamp;
    }
}