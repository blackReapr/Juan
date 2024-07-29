using Juan.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Juan.Hubs;

public class StatusHub : Hub
{
    private readonly UserManager<AppUser> _userManager;
    public StatusHub(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task Send(string userId, string message)
    {
        string? connectionId = _userManager.FindByIdAsync(userId)?.Result?.ClientId;
        await Clients.Client(connectionId).SendAsync("statusUpdate", message);
    }
    public override Task OnConnectedAsync()
    {
        if (Context.User.Identity.IsAuthenticated)
        {
            AppUser? user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;
            if (user != null)
            {
                user.ClientId = Context.ConnectionId;
                _userManager.UpdateAsync(user).Wait();
                Clients.All.SendAsync("userConnected", user.Id).Wait();
            }
        }
        return base.OnConnectedAsync();
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.User.Identity.IsAuthenticated)
        {
            AppUser? user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;
            if (user != null)
            {
                user.ClientId = null;
                _userManager.UpdateAsync(user).Wait();
                Clients.All.SendAsync("userDisconnected", user.Id).Wait();
            }
        }
        return base.OnDisconnectedAsync(exception);
    }
}
