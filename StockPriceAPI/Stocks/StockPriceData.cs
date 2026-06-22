using System.Text.Json.Serialization;

namespace StockPriceAPI.Stocks;

public class StockPriceData
{
    [JsonPropertyName("Meta Data")]
    public MetaData MetaData { get; set; } = null!;

    [JsonPropertyName("Time Series (Daily)")]
    public Dictionary<string, DailyTimeSeriesData> TimeSeries { get; set; } = null!;
}