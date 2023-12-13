using Microsoft.AspNetCore.SignalR;
using ScrumPlanningPoker.Entity.Room;
using ScrumPlanningPoker.Utils.Extensions;

namespace ScrumPlanningPoker.Hubs;

public class SessionRoomHub : Hub
{
    #region Statements

    private static readonly Dictionary<string, SessionRoom> Rooms = new();

    #endregion

    #region RoomCreation
    
    public void CheckRoomExists(string roomName)
    {
        if (!Rooms.ContainsKey(roomName))
        {
            CreateRoom(roomName);
        }
    }

    public void CreateRoom(string roomName)
    {
        var sessionRoom = new SessionRoom(roomName);
        Rooms.Add(roomName, sessionRoom);
    }

    #endregion

    #region UserConnection

    public Task JoinRoom(string roomName, User user)
    {
        var sessionRoom = GetSessionRoom(roomName);
        if (sessionRoom is null)
        {
            return Task.CompletedTask;
        }

        var userInSessionRoom = GetUserInSessionRoom(user, sessionRoom);
        if (userInSessionRoom is not null)
        {
            LeaveRoom(roomName, user);
        }

        sessionRoom.Users.Add(user);
        sessionRoom.SortUsers();
        
        Rooms[roomName] = sessionRoom;
        return Clients.All.SendAsync("ReceiveUserUpdateRoom", sessionRoom);
    }

    public Task LeaveRoom(string roomName, User user)
    {
        var sessionRoom = GetSessionRoom(roomName);
        if (sessionRoom is null)
        {
            return Task.CompletedTask;
        }
        
        var userToRemove = GetUserInSessionRoom(user, sessionRoom);
        if (userToRemove == null)
        {
            return Task.CompletedTask;
        }

        sessionRoom.Users.Remove(userToRemove);
        sessionRoom.SortUsers();
        
        Rooms[roomName] = sessionRoom;
        return Clients.All.SendAsync("ReceiveUserUpdateRoom", sessionRoom);
    }

    #endregion

    #region UserInteractions

    public Task ClickOnCard(string roomName, User user)
    {
        var sessionRoom = GetSessionRoom(roomName);
        if (sessionRoom is null)
        {
            return Task.CompletedTask;
        }
        
        var userToUpdate = GetUserInSessionRoom(user, sessionRoom);
        if (userToUpdate == null)
        {
            return Task.CompletedTask;
        }

        userToUpdate.CardValue = user.CardValue;
        sessionRoom.SortUsers();
        
        Rooms[roomName] = sessionRoom;
        return Clients.All.SendAsync("ReceiveUserUpdateRoom", sessionRoom);
    }
    
    public async Task RevealCards(string roomName, bool reveal)
    {
        var sessionRoom = GetSessionRoom(roomName);
        if (sessionRoom is null)
        {
            return;
        }

        sessionRoom.CardsIsRevealed = reveal;
        
        if (!reveal)
        {
            sessionRoom.Users.ForEach(u => u.CardValue = null);
        }
        
        sessionRoom.SortUsers();
        Rooms[roomName] = sessionRoom;
        
        await Clients.All.SendAsync("ReceiveRevealCards", sessionRoom, reveal);
        await Clients.All.SendAsync("ReceiveUserUpdateRoom", sessionRoom);
    }

    #endregion

    #region Functions
    
    private static SessionRoom? GetSessionRoom(string roomName)
    {
        _ = !Rooms.TryGetValue(roomName, out var sessionRoom);
        return sessionRoom;
    }
    
    private static User? GetUserInSessionRoom(User user, SessionRoom sessionRoom)
    {
        foreach (var userInSessionRoom in sessionRoom.Users)
        {
            if (userInSessionRoom.Guid == user.Guid && userInSessionRoom.Name == user.Name) 
                return userInSessionRoom;
        }

        return null;
    }

    #endregion
}