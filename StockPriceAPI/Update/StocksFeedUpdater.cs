using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using StockPriceAPI.Stocks;

namespace StockPriceAPI.Update;

public class StocksFeedUpdater(
    IServiceScopeFactory serviceScopeFactory,
    ActiveTickerManager activeTickerManager,
    IHubContext<StocksFeedHub, IStockUpdateClient> hubContext,
    IOptions<StockUpdateOptions> options,
    ILogger<StocksFeedUpdater> logger)
    : BackgroundService
{
    private readonly Random _random = new();
    private readonly StockUpdateOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdateStockPrices(stoppingToken);

            await Task.Delay(_options.UpdateInterval, stoppingToken);
        }
    }

    private async Task UpdateStockPrices(CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        StockService stockService = scope.ServiceProvider.GetRequiredService<StockService>();

        foreach (string ticker in activeTickerManager.GetAllTickers())
        {
            StockPriceRecord? currentPrice = await stockService.GetLatestStockPrice(ticker, cancellationToken);
            if (currentPrice == null)
            {
                continue;
            }

            decimal newPrice = CalculateNewPrice(currentPrice);

            var update = new StockPriceUpdate(ticker, newPrice);

            await hubContext.Clients.Group(ticker).ReceiveStockPriceUpdate(update);

            logger.LogInformation("Updated {Ticker} price to {Price}", ticker, newPrice);
        }
    }

    private decimal CalculateNewPrice(StockPriceRecord currentPrice)
    {
        double change = _options.MaxPercentageChange;
        decimal priceFactor = (decimal)(_random.NextDouble() * change * 2 - change);
        decimal priceChange = currentPrice.Price * priceFactor;
        decimal newPrice = Math.Max(0, currentPrice.Price + priceChange);
        newPrice = Math.Round(newPrice, 2);
        return newPrice;
    }
}