using Microsoft.AspNetCore.SignalR;

namespace StockPriceAPI.Update;

public class StocksFeedHub : Hub<IStockUpdateClient>
{
    public async Task JoinStockGroup(string ticker)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, ticker);
    }

    public async Task LeaveStockGroup(string ticker)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, ticker);
    }
}