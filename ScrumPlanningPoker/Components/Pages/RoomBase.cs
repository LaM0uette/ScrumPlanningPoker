using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ScrumPlanningPoker.Components.Pages;

public class RoomBase : ComponentBase, IAsyncDisposable
{
    #region Statements

    [Parameter] public string RoomName { get; set; } = "";
    protected string? UserNameInput { get; set; }
    protected string? UserName { get; set; }
    protected int UsersCount;
    protected List<string> _users = new();
    
    [Inject] private NavigationManager _navigationManager { get; init; } = default!;
    
    private HubConnection? _hubConnection;

    #endregion

    #region Functions
    
    protected override async Task OnInitializedAsync()
    {
        if (_hubConnection == null)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/session-room-hub"))
                .Build();

            _hubConnection.On<string, List<string>>("ReceiveUserUpdateRoom", (roomName, users) =>
            {
                if (RoomName != roomName)
                    return;
                
                UsersCount = users.Count;
                _users = users;
                
                InvokeAsync(StateHasChanged);
            });

            await _hubConnection.StartAsync();
        }
    }
    
    protected async Task JoinRoom()
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            UserName = UserNameInput;
            await _hubConnection.SendAsync("UserJoinRoom", RoomName, UserName);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await LeaveRoom();
            
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
        }
        
        GC.SuppressFinalize(this);
    }
    
    private async Task LeaveRoom()
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("UserLeaveRoom", RoomName, UserName);
        }
    }

    #endregion
}