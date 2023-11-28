using Microsoft.AspNetCore.SignalR;
using ScrumPlanningPoker.Entity.RoomHub;

namespace ScrumPlanningPoker.Hubs;

public class SessionRoomHub : Hub
{
    #region Statements

    private static readonly Dictionary<string, SessionRoom> Rooms = new();

    #endregion

    #region RoomCreation

    public void CreateRoom(string roomName)
    {
        var sessionRoom = new SessionRoom(roomName);
        
        Rooms.Add(roomName, sessionRoom);
    }

    #endregion

    #region UserConnection

    public Task JoinRoom(string roomName, string userName)
    {
        if (!Rooms.TryGetValue(roomName, out var sessionRoom))
            return Task.CompletedTask;
        
        var user = new User(userName, "user");

        sessionRoom.Users.Add(user);
        Rooms[roomName] = sessionRoom;
        
        return Clients.All.SendAsync("ReceiveUserUpdateRoom", sessionRoom);
    }
    
    public Task LeaveRoom(string roomName, string userName)
    {
        if (!Rooms.TryGetValue(roomName, out var sessionRoom))
            return Task.CompletedTask;
        
        var user = sessionRoom.Users.FirstOrDefault(user => user.Name == userName);
        if (user == null)
            return Task.CompletedTask;

        sessionRoom.Users.Remove(user);
        Rooms[roomName] = sessionRoom;
        
        return Clients.All.SendAsync("ReceiveUserUpdateRoom", sessionRoom);
    }

    #endregion
}