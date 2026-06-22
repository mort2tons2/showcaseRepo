using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StockPriceAPI.Update;
using StockPriceAPI.Stocks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<StocksClient>(httpClient =>
{
    var uri = builder.Configuration["Stocks:ApiUrl"];
    if (uri is not null)
    {
        httpClient.BaseAddress = new Uri(builder.Configuration["Stocks:ApiUrl"]);
    }
});

builder.Services.AddOpenApi();

builder.Services.AddScoped<StockService>();
builder.Services.AddSingleton<ActiveTickerManager>();
builder.Services.AddHostedService<StocksFeedUpdater>();

builder.Services.Configure<StockUpdateOptions>(builder.Configuration.GetSection("StockUpdateOptions"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173/")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });

    options.AddPolicy("SignalRPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:60448")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();

    app.UseCors(policy => policy
        .WithOrigins(builder.Configuration["Cors:AllowedOrigin"])
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
}

app.MapGet("/api/stocks", async (string ticker, StockService stockService, CancellationToken cancellationToken) =>
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

    //StockPriceRecord? result = await stockService.GetLatestStockPrice(ticker, cancellationToken);

    /*return result is null
        ? Results.NotFound($"No stock data available for ticker: {ticker}")
        : Results.Ok(result);*/
})
.WithName("GetLatestStockPrice")
.WithOpenApi();

app.UseCors("AllowAngularApp");
app.UseCors("SignalRPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHub<StocksFeedHub>("/stocks-feed").RequireCors("SignalRPolicy");

//app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();