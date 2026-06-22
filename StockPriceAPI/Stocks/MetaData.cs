using System.Text.Json.Serialization;

namespace StockPriceAPI.Stocks;

public class MetaData
{
    [JsonPropertyName("2. Symbol")]
    public string Symbol { get; set; } = null!;

    [JsonPropertyName("3. Last Refreshed")]
    public string LastRefreshed { get; set; } = null!;
}