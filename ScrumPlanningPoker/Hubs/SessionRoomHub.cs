using Microsoft.AspNetCore.SignalR;

namespace ScrumPlanningPoker.Hubs;

public struct Room
{
    public Timer Timer { get; set; }
    public List<string> Users { get; set; } // creéer un struct puis une enum pour le role
}

public class SessionRoomHub : Hub
{
    #region Statements

    private static Dictionary<string, Room> Rooms = new();

    #endregion

    #region RoomCreation

    public void CreateRoom(string name)
    {
        var room = new Room
        {
            Timer = new Timer(DeleteRoom, name, TimeSpan.FromHours(24), Timeout.InfiniteTimeSpan),
            Users = []
        };
        
        Rooms.Add(name, room);
    }

    private static void DeleteRoom(object? state)
    {
        if (state == null) return;
        
        var roomName = (string) state;
        Rooms.Remove(roomName);
    }

    #endregion

    #region UserConnection

    public async Task UserJoinRoom(string roomName, string userName)
    {
        if (!Rooms.TryGetValue(roomName, out var value))
            return;

        value.Users.Add(userName);
        Rooms[roomName] = value;
        
        await Clients.All.SendAsync("ReceiveUserUpdateRoom", roomName ,Rooms[roomName].Users.Count);
    }
    
    public async Task UserLeaveRoom(string roomName, string userName)
    {
        if (!Rooms.TryGetValue(roomName, out var value))
            return;

        value.Users.Remove(userName);
        Rooms[roomName] = value;
        
        await Clients.All.SendAsync("ReceiveUserUpdateRoom", roomName, Rooms[roomName].Users.Count);
    }

    #endregion
    
    
    
    
    
    
    
    
    
    private static int UsersCount { get; set; }
    
    public async Task OnConnection()
    {
        UsersCount++;
        await Clients.All.SendAsync("ReceiveNotify", UsersCount);
    }
    
    public async Task OnDisconnection()
    {
        UsersCount--;
        await Clients.All.SendAsync("ReceiveNotify", UsersCount);
    }
}