using Microsoft.AspNetCore.SignalR;
using ScrumPlanningPoker.Entity.RoomHub;

namespace ScrumPlanningPoker.Hubs;

public class SessionRoomHub : Hub
{
    #region Statements

    private static readonly Dictionary<string, SessionRoom> Rooms = new();

    #endregion

    #region RoomCreation

    public void CreateRoom(string name)
    {
        var timer = new Timer(DeleteRoom, name, TimeSpan.FromHours(24), Timeout.InfiniteTimeSpan);
        var sessionRoom = new SessionRoom(timer);
        
        Rooms.Add(name, sessionRoom);
    }

    private static void DeleteRoom(object? state)
    {
        if (state == null) return;
        
        var roomName = (string) state;
        Rooms.Remove(roomName);
    }

    #endregion

    #region UserConnection

    public Task JoinRoom(string roomName, string userName)
    {
        if (!Rooms.TryGetValue(roomName, out var value))
            return Task.CompletedTask;

        value.Users.Add(userName);
        Rooms[roomName] = value;
        
        return Clients.All.SendAsync("ReceiveUserUpdateRoom", roomName ,Rooms[roomName].Users);
    }
    
    public Task LeaveRoom(string roomName, string userName)
    {
        if (!Rooms.TryGetValue(roomName, out var value))
            return Task.CompletedTask;

        value.Users.Remove(userName);
        Rooms[roomName] = value;
        
        return Clients.All.SendAsync("ReceiveUserUpdateRoom", roomName, Rooms[roomName].Users);
    }

    #endregion
}