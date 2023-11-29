using Microsoft.AspNetCore.Components;
using ScrumPlanningPoker.Entity.RoomHub;
using ScrumPlanningPoker.Services.HubServices;

namespace ScrumPlanningPoker.Components.Pages;

public class RoomBase : ComponentBase, IAsyncDisposable
{
    #region Statements

    [Parameter] public string RoomName { get; set; } = "";
    protected bool RoomIsValid { get; private set; }

    protected string UserName { get; set; } = "";
    protected bool IsSpectator { get; set; }
    
    protected int[] CardValues { get; private init; } = { 1, 2, 3, 5, 8, 13, 20, 40, 100 };
    
    protected int UsersCount;
    protected List<User> _users = new();
    
    [Inject] private HubService _hubService { get; init; } = default!;

    #endregion

    #region Functions
    
    protected override void OnInitialized()
    {
        _hubService.OnUserUpdateRoom += HandleUserUpdateRoom;
    }
    
    protected override async Task OnInitializedAsync()
    {
        await _hubService.InitializeConnectionAsync();
        await JoinRoom();
    }

    private void HandleUserUpdateRoom(SessionRoom room)
    {
        if (RoomName != room.Name)
            return;
                
        UsersCount = room.Users.Count;
        _users = room.Users;

        InvokeAsync(StateHasChanged);
    }
    
    protected Task JoinRoom()
    {
        //UserName = "TestTEst";
        if (string.IsNullOrEmpty(UserName) || UserName.Length > 30)
        {
            RoomIsValid = false;
            return Task.CompletedTask;
        }
        
        RoomIsValid = true;
        return _hubService.JoinRoomAsync(GenerateGuid(), RoomName, UserName, IsSpectator);
    }
    
    private static string GenerateGuid()
    {
        return "test";
        //return Guid.NewGuid().ToString("N");
    }
    
    protected void ClickOnCard(int cardValue)
    {
        Console.WriteLine(cardValue);
    }
    
    public async ValueTask DisposeAsync()
    {
        _hubService.OnUserUpdateRoom -= HandleUserUpdateRoom;
        
        await LeaveRoom();
        
        GC.SuppressFinalize(this);
    }
    
    private Task LeaveRoom()
    {
        return _hubService.LeaveRoomAsync(RoomName, UserName);
    }

    #endregion
}