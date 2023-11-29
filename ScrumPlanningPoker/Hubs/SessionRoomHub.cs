﻿using Microsoft.AspNetCore.SignalR;
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

    public Task JoinRoom(string roomName, User user)
    {
        if (!Rooms.TryGetValue(roomName, out var sessionRoom))
            return Task.CompletedTask;

        sessionRoom.Users.Add(user);
        Rooms[roomName] = sessionRoom;
        
        return Clients.All.SendAsync("ReceiveUserUpdateRoom", sessionRoom);
    }
    
    public Task LeaveRoom(string roomName, User user)
    {
        if (!Rooms.TryGetValue(roomName, out var sessionRoom))
            return Task.CompletedTask;
        
        var userToRemove = sessionRoom.Users.FirstOrDefault(u => u.Guid == user.Guid && u.Name == user.Name);
        if (userToRemove == null)
            return Task.CompletedTask;

        sessionRoom.Users.Remove(userToRemove);
        Rooms[roomName] = sessionRoom;
        
        return Clients.All.SendAsync("ReceiveUserUpdateRoom", sessionRoom);
    }

    #endregion

    #region UserInteractions

    public Task ClickOnCard(string roomName, User user)
    {
        if (!Rooms.TryGetValue(roomName, out var sessionRoom))
            return Task.CompletedTask;
        
        var userToUpdate = sessionRoom.Users.FirstOrDefault(u => u.Guid == user.Guid && u.Name == user.Name);
        if (userToUpdate == null)
            return Task.CompletedTask;

        userToUpdate.CardValue = user.CardValue;
        Rooms[roomName] = sessionRoom;
        
        return Clients.All.SendAsync("ReceiveUserUpdateRoom", sessionRoom);
    }
    
    public Task RevealCards(string roomName, bool reveal)
    {
        if (!Rooms.TryGetValue(roomName, out _))
            return Task.CompletedTask;

        return Clients.All.SendAsync("ReceiveRevealCards", reveal);
    }

    #endregion
}