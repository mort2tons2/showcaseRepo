using System.Text.Json.Serialization;

namespace StockPriceAPI.Stocks;

public class DailyTimeSeriesData
{
    [JsonPropertyName("1. open")]
    public string Open { get; set; } = null!;

    [JsonPropertyName("4. close")]
    public string Close { get; set; } = null!;
}