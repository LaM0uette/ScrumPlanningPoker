using Microsoft.AspNetCore.SignalR;

namespace ScrumPlanningPoker.Hubs;

public struct Room
{
    public Timer Timer { get; set; }
    public List<string> Users { get; init; }
}

public class SessionRoomHub : Hub
{
    #region Statements

    private static readonly Dictionary<string, Room> Rooms = new();

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

    public async Task JoinRoom(string roomName, string userName)
    {
        if (!Rooms.TryGetValue(roomName, out var value))
            return;

        value.Users.Add(userName);
        Rooms[roomName] = value;
        
        await Clients.All.SendAsync("ReceiveUserUpdateRoom", roomName ,Rooms[roomName].Users);
    }
    
    public async Task LeaveRoom(string roomName, string userName)
    {
        if (!Rooms.TryGetValue(roomName, out var value))
            return;

        value.Users.Remove(userName);
        Rooms[roomName] = value;
        
        await Clients.All.SendAsync("ReceiveUserUpdateRoom", roomName, Rooms[roomName].Users);
    }

    #endregion
}