using Microsoft.EntityFrameworkCore;
using StockPriceAPI.Update;

namespace StockPriceAPI.Stocks;

public class StockService(
    AppDbContext dbContext,
    StocksClient stocksClient,
    ActiveTickerManager activeTickerManager,
    ILogger<StockService> logger)
{
    public async Task<StockPriceRecord?> GetLatestStockPrice(string ticker, CancellationToken cancellationToken)
    {
        try
        {
            var dbPrice = await GetLatestStockPriceFromDatabase(ticker, cancellationToken);
            if (dbPrice is not null)
            {
                activeTickerManager.AddTicker(ticker);
                return dbPrice;
            }

            var apiPrice = await stocksClient.GetDataForTicker(ticker, cancellationToken);

            if (apiPrice == null)
            {
                logger.LogWarning("No data returned from external API for ticker: {Ticker}", ticker);
                return null;
            }

            apiPrice.Timestamp = DateTime.SpecifyKind(apiPrice.Timestamp, DateTimeKind.Unspecified);

            await SavePriceToDatabase(apiPrice, cancellationToken);

            activeTickerManager.AddTicker(ticker);

            return apiPrice;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching stock price for ticker: {Ticker}", ticker);
            throw;
        }
    }

    private async Task<StockPriceRecord?> GetLatestStockPriceFromDatabase(string ticker, CancellationToken cancellationToken)
    {
        var result = await dbContext.StockPriceRecords
            .Where(x => x.Ticker == ticker)
            .OrderByDescending(x => x.Timestamp)
            .SingleOrDefaultAsync(cancellationToken);

        return result is not null ?
            new StockPriceRecord
            {
                Ticker = result.Ticker,
                Price = result.Price,
                Timestamp = result.Timestamp
            } : null;
    }

    private async Task SavePriceToDatabase(StockPriceRecord price, CancellationToken cancellationToken)
    {
        await dbContext.StockPriceRecords.AddAsync(price, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}