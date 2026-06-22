/*var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
*/


using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StockPriceAPI.Update;
using StockPriceAPI.Stocks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddMemoryCache();

/*builder.Services.AddSingleton(_ =>
{
    string connectionString = builder.Configuration.GetConnectionString("Database");

    var npgsqlDataSource = NpgsqlDataSource.Create(connectionString);

    return npgsqlDataSource;
});*/

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddHostedService<DatabaseInitializer>();

builder.Services.AddHttpClient<StocksClient>(httpClient =>
{
    var uri = builder.Configuration["Stocks:ApiUrl"];
    // hvordan sjekke for possible null reference?
    if (uri is not null)
    {
        httpClient.BaseAddress = new Uri(builder.Configuration["Stocks:ApiUrl"]);
    }
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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
        policy.WithOrigins("https://localhost:60448") // Replace with your exact frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Crucial for SignalR negotiation
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

/*using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}*/

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate(); // Creates database and applies migrations
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
    app.MapOpenApi();

    app.MapScalarApiReference();

    app.UseCors(policy => policy
        .WithOrigins(builder.Configuration["Cors:AllowedOrigin"])
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
}

// If using the modern .NET 9 Scalar UI alternative
//app.MapGet("/", () => Results.Redirect("/scalar/v1"));

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

//app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapHub<StocksFeedHub>("/stocks-feed").RequireCors("SignalRPolicy");

//app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();