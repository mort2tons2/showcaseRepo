namespace StockPriceAPI.Stocks;

public sealed record StockPriceRecord
{
    public int Id { get; set; }
    public string Ticker { get; set; } = null!;
    public decimal Price { get; set; }
    public DateTime Timestamp { get; set; }

    /*public StockPriceRecord(string ticker, decimal price, DateTime timestamp)
    {
        Ticker = ticker;
        Price = price;
        Timestamp = timestamp;
    }*/

    public StockPriceRecord() {}

    //protected StockPriceRecord() {}
}