using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Pulse.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public async Task JoinEventChat(string eventId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, eventId);
        await Clients.Group(eventId).SendAsync("UserJoined", Context.UserIdentifier);
    }

    public async Task LeaveEventChat(string eventId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventId);
        await Clients.Group(eventId).SendAsync("UserLeft", Context.UserIdentifier);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
