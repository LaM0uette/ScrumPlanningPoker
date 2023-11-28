using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ScrumPlanningPoker.Services.HubServices;

public class HubService(NavigationManager navigationManager) : IHubService, IRoomService, IAsyncDisposable
{
    #region Statements
    
    public event Action<string, List<string>>? OnUserUpdateRoom;
    
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

        _hubConnection.On<string, List<string>>("ReceiveUserUpdateRoom", (roomName, users) =>
        {
            OnUserUpdateRoom?.Invoke(roomName, users);
        });
        
        return _hubConnection.StartAsync();
    }

    public Task CreateRoomAsync(string roomName)
    {
        if (_hubConnection?.State != HubConnectionState.Connected) 
            return Task.CompletedTask;
        
        return _hubConnection.SendAsync("CreateRoom", roomName);
    }

    public Task JoinRoomAsync(string roomName, string userName)
    {
        if (_hubConnection?.State != HubConnectionState.Connected) 
            return Task.CompletedTask;
        
        return _hubConnection.SendAsync("JoinRoom", roomName, userName);
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

    #region Functions

    public string GetBaseUri()
    {
        return navigationManager.BaseUri;
    }

    #endregion
}