namespace StockPriceAPI.Stocks
{
    public static class StockPriceEndpoints
    {
        public static void MapStockPriceEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("api/stocks");

            group.MapGet("/{ticker}", GetLatestStockPrice)
                .WithName(nameof(GetLatestStockPrice))
                .WithOpenApi();
        }

        private static async Task<IResult> GetLatestStockPrice(string ticker, StockService stockService, CancellationToken cancellationToken)
        {
            try
            {
                var result = await stockService.GetLatestStockPrice(ticker, cancellationToken);

                return Results.Ok(result);
            }
            catch (OperationCanceledException)
            {
                return Results.StatusCode(499);
            }
        }
    }
}
