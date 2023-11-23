using Microsoft.AspNetCore.SignalR;

namespace ScrumPlanningPoker.Hubs;

public class TestHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        Console.WriteLine($"SendMessage: {user} - {message}");
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}