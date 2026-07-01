using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Scalar.AspNetCore;
using StockPriceAPI.Stocks;
using StockPriceAPI.Update;
//using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();
builder.Services.AddMemoryCache();

if (builder.Environment.IsProduction())
{
    var keyVaultUri = builder.Configuration["AzureKeyVaultUrl"];
    //builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
}
else
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration["baseConnectionString"];
    var conStrBuilder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString)
    {
        Password = builder.Configuration["DbPassword"]
    };

    if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
        || conStrBuilder.Host == "localhost"
        || conStrBuilder.Host == "127.0.0.1")
    {
        conStrBuilder.Host = "host.docker.internal";
    }

    var connection = conStrBuilder.ConnectionString;

    options.UseNpgsql(connection);

    // "host=localhost;port=5432;database=postgres;userId=postgres;password=admin"
});

builder.Services.AddHttpClient<StocksClient>(httpClient =>
{
    var uri = builder.Configuration["Stocks:ApiUrl"];
    if (uri is not null)
    {
        httpClient.BaseAddress = new Uri(uri);
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

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["StorageConnection:blobServiceUri"]!).WithName("StorageConnection");
    clientBuilder.AddQueueServiceClient(builder.Configuration["StorageConnection:queueServiceUri"]!).WithName("StorageConnection");
    clientBuilder.AddTableServiceClient(builder.Configuration["StorageConnection:tableServiceUri"]!).WithName("StorageConnection");
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
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

/*app.MapGet("/api/stocks", async (string ticker, StockService stockService, CancellationToken cancellationToken) =>
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

    *//*return result is null
        ? Results.NotFound($"No stock data available for ticker: {ticker}")
        : Results.Ok(result);*//*
})
.WithName("GetLatestStockPrice")
.WithOpenApi();*/

app.UseCors("AllowAngularApp");
app.UseCors("SignalRPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHub<StocksFeedHub>("/stocks-feed").RequireCors("SignalRPolicy");

//app.MapControllers();

app.MapStockPriceEndpoints();

app.MapFallbackToFile("/index.html");

app.Run();