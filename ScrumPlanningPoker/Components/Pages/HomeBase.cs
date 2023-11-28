using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ScrumPlanningPoker.Components.Pages;

public class HomeBase : ComponentBase, IAsyncDisposable
{
    #region Statements

    protected string? RoomLink;
    
    [Inject] private NavigationManager _navigationManager { get; init; } = default!;
    
    private HubConnection? _hubConnection;

    #endregion

    #region Functions
    
    protected override Task OnInitializedAsync()
    {
        if (_hubConnection != null) 
            return Task.CompletedTask;
        
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_navigationManager.ToAbsoluteUri("/session-room-hub"))
            .Build();

        return _hubConnection.StartAsync();
    }

    protected async void Create()
    {
        var roomName = GenerateUniqueRoomName();
        RoomLink = _navigationManager.BaseUri + $"room/{roomName}";

        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("CreateRoom", roomName);
        }
    }

    private static string GenerateUniqueRoomName()
    {
        return Guid.NewGuid().ToString("N");
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
}