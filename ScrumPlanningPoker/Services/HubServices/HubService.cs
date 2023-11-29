using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using ScrumPlanningPoker.Entity.RoomHub;

namespace ScrumPlanningPoker.Services.HubServices;

public class HubService(NavigationManager navigationManager) : IHubService, IRoomService, IAsyncDisposable
{
    #region Statements
    
    public event Action<SessionRoom>? OnUserUpdateRoom;
    
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
        
        return _hubConnection.StartAsync();
    }

    public Task CreateRoomAsync(string roomName)
    {
        if (_hubConnection?.State != HubConnectionState.Connected) 
            return Task.CompletedTask;
        
        return _hubConnection.SendAsync("CreateRoom", roomName);
    }

    public Task JoinRoomAsync(string roomName, string guid, string userName, bool isSpectator)
    {
        if (_hubConnection?.State != HubConnectionState.Connected) 
            return Task.CompletedTask;
        
        return _hubConnection.SendAsync("JoinRoom", guid, roomName, userName, isSpectator);
    }

    public Task LeaveRoomAsync(string roomName, string userName)
    {
        if (_hubConnection?.State != HubConnectionState.Connected) 
            return Task.CompletedTask;
        
        return _hubConnection.SendAsync("LeaveRoom", roomName, userName);
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

    public Task ClickOnCardAsync(string roomName, string guid, string userName, int cardValue)
    {
        if (_hubConnection?.State != HubConnectionState.Connected) 
            return Task.CompletedTask;
        
        return _hubConnection.SendAsync("ClickOnCard", roomName, guid, userName, cardValue);
    }

    #endregion

    #region Functions

    public string GetBaseUri()
    {
        return navigationManager.BaseUri;
    }

    #endregion
}