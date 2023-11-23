using Microsoft.AspNetCore.SignalR;

namespace ScrumPlanningPoker.Hubs;

public class ClientsHub : Hub
{
    public static int ClientCount { get; set; }
    
    public async Task Notify()
    {
        ClientCount++;
        await Clients.All.SendAsync("ReceiveNotify", ClientCount);
    }
}