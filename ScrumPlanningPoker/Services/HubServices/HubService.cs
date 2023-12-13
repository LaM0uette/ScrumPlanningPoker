using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using ScrumPlanningPoker.Entity.Room;

namespace ScrumPlanningPoker.Services.HubServices;

public class HubService(NavigationManager navigationManager) : IHubService, IRoomService, IAsyncDisposable
{
    #region Statements
    
    public event Action<SessionRoom>? OnUserUpdateRoom;
    public event Action<SessionRoom, bool>? OnRevealCards;
    
    private HubConnection? _hubConnection;

    #endregion

    #region InterfaceFunctions

    public Task InitializeConnectionAsync()
    {
        if (_hubConnection != null) 
            return Task.CompletedTask;
        
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri("/session-room-hub"))
            .Build();

        _hubConnection.On<SessionRoom>("ReceiveUserUpdateRoom", (room) =>
        {
            OnUserUpdateRoom?.Invoke(room);
        });
        
        _hubConnection.On<SessionRoom, bool>("ReceiveRevealCards", (room, reveal) =>
        {
            OnRevealCards?.Invoke(room, reveal);
        });
        
        return _hubConnection.StartAsync();
    }

    public Task CheckRoomExistsAsync(string roomName)
    {
        if (_hubConnection?.State != HubConnectionState.Connected) 
            return Task.CompletedTask;
        
        return _hubConnection.SendAsync("CheckRoomExists", roomName);
    }
    
    public Task CreateRoomAsync(string roomName)
    {
        if (_hubConnection?.State != HubConnectionState.Connected) 
            return Task.CompletedTask;
        
        return _hubConnection.SendAsync("CreateRoom", roomName);
    }

    public Task JoinRoomAsync(string roomName, User user)
    {
        if (_hubConnection?.State != HubConnectionState.Connected) 
            return Task.CompletedTask;
        
        return _hubConnection.SendAsync("JoinRoom", roomName, user);
    }

    public Task LeaveRoomAsync(string roomName, User user)
    {
        if (_hubConnection?.State != HubConnectionState.Connected) 
            return Task.CompletedTask;
        
        return _hubConnection.SendAsync("LeaveRoom", roomName, user);
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }

    #endregion

    #region UserInteractions

    public Task ClickOnCardAsync(string roomName, User user)
    {
        if (_hubConnection?.State != HubConnectionState.Connected) 
            return Task.CompletedTask;
        
        return _hubConnection.SendAsync("ClickOnCard", roomName, user);
    }
    
    public Task RevealCardsAsync(string roomName, bool reveal)
    {
        if (_hubConnection?.State != HubConnectionState.Connected) 
            return Task.CompletedTask;
        
        return _hubConnection.SendAsync("RevealCards", roomName, reveal);
    }

    #endregion

    #region Functions

    public string GetBaseUri()
    {
        return navigationManager.BaseUri;
    }

    #endregion
}