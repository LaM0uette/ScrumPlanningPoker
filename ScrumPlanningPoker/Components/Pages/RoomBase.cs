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
    protected bool CardsIsRevealed { get; set; }
    
    protected int[] CardValues { get; private init; } = { 1, 2, 3, 5, 8, 13, 20, 40, 100 };
    protected bool CanChooseCard { get; set; } = true;
    
    protected int UsersCount;
    protected User CurrentUser;
    protected List<User> Users = new();
    
    [Inject] private HubService _hubService { get; init; } = default!;

    #endregion

    #region Functions
    
    protected override void OnInitialized()
    {
        _hubService.OnUserUpdateRoom += HandleUserUpdateRoom;
        _hubService.OnRevealCards += HandleRevealCards;
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
        Users = room.Users;

        InvokeAsync(StateHasChanged);
    }
    
    private void HandleRevealCards(bool reveal)
    {
        CardsIsRevealed = reveal;
        CanChooseCard = !reveal;
        
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
        
        CurrentUser = new User(GenerateGuid(), UserName, IsSpectator);
        
        RoomIsValid = true;
        return _hubService.JoinRoomAsync(RoomName, CurrentUser);
    }
    
    private static string GenerateGuid()
    {
        return "test";
        //return Guid.NewGuid().ToString("N");
    }
    
    protected Task ClickOnCard(int cardValue)
    {
        if (!CanChooseCard) 
            return Task.CompletedTask;
        
        CurrentUser.CardValue = cardValue;
        
        CanChooseCard = false;
        return _hubService.ClickOnCardAsync(RoomName, CurrentUser);
    }
    
    protected Task RevealCards()
    {
        return _hubService.RevealCardsAsync(RoomName, true);
    }
    
    protected Task NewGame()
    {
        return _hubService.RevealCardsAsync(RoomName, false);
    }
    
    public async ValueTask DisposeAsync()
    {
        _hubService.OnUserUpdateRoom -= HandleUserUpdateRoom;
        _hubService.OnRevealCards -= HandleRevealCards;
        
        await LeaveRoom();
        
        GC.SuppressFinalize(this);
    }
    
    private Task LeaveRoom()
    {
        return _hubService.LeaveRoomAsync(RoomName, CurrentUser);
    }

    #endregion
}