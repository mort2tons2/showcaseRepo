using System.Globalization;

namespace StockPriceAPI.Stocks;

public class StocksClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StocksClient> _logger;
    private readonly IConfiguration _config;

    public StocksClient(HttpClient httpClient, ILogger<StocksClient> logger, IConfiguration config)
    {
        _httpClient = httpClient;
        _logger = logger;
        _config = config;
    }

    public async Task<StockPriceRecord?> GetDataForTicker(string ticker, CancellationToken cancellationToken)
    {
        var alphaVantageKey = _config["Stocks:AlphaVantageApiKey"];
        var queryUrl = _config["queryUrl"] + _config["Stocks:AlphaVantageApiKey"];
        // 8PZDV7O4DLKKAFX0
        try
        {
            var response = await _httpClient.GetAsync(queryUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch data for {Ticker}. Status Code: {StatusCode}", ticker, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadFromJsonAsync<StockPriceData>(cancellationToken);
            if (content?.TimeSeries == null || content.TimeSeries.Count == 0)
                return null;


            // 1. Get the most recent date entry in the dictionary
            var latestEntry = content.TimeSeries
                .OrderByDescending(x => x.Key)
                .First();

            var latestDateStr = latestEntry.Key;
            var dataPoints = latestEntry.Value;

            // 2. Safely parse values into your original flat response structure
            var closePrice = decimal.Parse(dataPoints.Close, CultureInfo.InvariantCulture);
            var timestamp = DateTime.Parse(latestDateStr, CultureInfo.InvariantCulture);

            var stockPriceRecord = new StockPriceRecord
            {
                Ticker = content.MetaData.Symbol,
                Price = closePrice,
                Timestamp = DateTime.SpecifyKind(timestamp, DateTimeKind.Utc)
            };

            return stockPriceRecord;
        }
        catch (HttpRequestException ex)
        {
            // Handle network errors (e.g., DNS issues, timeout)
            _logger.LogError(ex, "Network error while fetching data for {Ticker}", ticker);
            return null;
        }
    }
}