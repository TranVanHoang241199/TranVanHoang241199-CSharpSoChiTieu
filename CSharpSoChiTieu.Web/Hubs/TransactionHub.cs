// CSharpSoChiTieu/Hubs/TransactionHub.cs
using Microsoft.AspNetCore.SignalR;

public class TransactionHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User.FindFirst("IdUser")?.Value;
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        await base.OnConnectedAsync();
    }
}