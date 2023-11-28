using Microsoft.AspNetCore.Components;
using ScrumPlanningPoker.Services.HubServices;

namespace ScrumPlanningPoker.Components.Pages;

public class RoomBase : ComponentBase, IAsyncDisposable
{
    #region Statements

    [Parameter] public string RoomName { get; set; } = "";
    protected bool RoomIsValid { get; private set; }

    protected string UserName { get; set; } = "";
    protected int UsersCount;
    protected List<string> _users = new();
    
    [Inject] private HubService _hubService { get; init; } = default!;

    #endregion

    #region Functions
    
    protected override void OnInitialized()
    {
        _hubService.OnUserUpdateRoom += HandleUserUpdateRoom;
    }
    
    protected override Task OnInitializedAsync()
    {
        return _hubService.InitializeConnectionAsync();
    }
    
    private void HandleUserUpdateRoom(string roomName, List<string> users)
    {
        if (RoomName != roomName)
            return;
                
        UsersCount = users.Count;
        _users = users;

        InvokeAsync(StateHasChanged);
    }
    
    protected Task JoinRoom()
    {
        if (string.IsNullOrEmpty(UserName) || UserName.Length > 30)
        {
            RoomIsValid = false;
            return Task.CompletedTask;
        }
        
        RoomIsValid = true;
        return _hubService.JoinRoomAsync(RoomName, UserName);
    }
    
    private Task LeaveRoom()
    {
        return _hubService.LeaveRoomAsync(RoomName, UserName);
    }
    
    public async ValueTask DisposeAsync()
    {
        _hubService.OnUserUpdateRoom -= HandleUserUpdateRoom;
        
        await LeaveRoom();
        
        GC.SuppressFinalize(this);
    }

    #endregion
}