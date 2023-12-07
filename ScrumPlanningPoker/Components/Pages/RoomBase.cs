using Microsoft.AspNetCore.Components;
using ScrumPlanningPoker.Entity.RoomHub;
using ScrumPlanningPoker.Services;
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
    
    private int? SelectedCardValue { get; set; }
    private bool CanChooseCard { get; set; } = true;
    
    protected User CurrentUser = null!;
    protected List<User> Users = [];
    
    [Inject] private CookieService _cookieService { get; init; } = default!;
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
        var cookieUserName = await _cookieService.GetCookie(CookieService.CookieUserName);
        if (cookieUserName != null)
        {
            UserName = cookieUserName;
        }

        await _hubService.InitializeConnectionAsync();
    }

    private void HandleUserUpdateRoom(SessionRoom room)
    {
        if (RoomName != room.Name)
            return;
                
        Users = room.Users;

        InvokeAsync(StateHasChanged);
    }
    
    private void HandleRevealCards(SessionRoom room, bool reveal)
    {
        if (RoomName != room.Name)
            return;
        
        CardsIsRevealed = reveal;
        CanChooseCard = !reveal;
        SelectedCardValue = null;
        
        InvokeAsync(StateHasChanged);
    }
    
    protected async Task JoinRoom()
    {
        if (string.IsNullOrEmpty(UserName) || UserName.Length > 18)
        {
            RoomIsValid = false;
            return;
        }
        
        await HandleCookie();
        
        RoomIsValid = true;
        await _hubService.JoinRoomAsync(RoomName, CurrentUser);
    }

    private async Task HandleCookie()
    {
        string userGuid;
        
        var cookieUserGuid = await _cookieService.GetCookie(CookieService.CookieUserGuid);
        if (cookieUserGuid != null)
        {
            userGuid = cookieUserGuid;
        }
        else
        {
            userGuid = GenerateGuid();
            await _cookieService.SetCookie(CookieService.CookieUserGuid, userGuid);
        }
        
        var cookieUserName = await _cookieService.GetCookie(CookieService.CookieUserName);
        if (cookieUserName == null)
        {
            await _cookieService.SetCookie(CookieService.CookieUserName, UserName, 14);
        }
        else
        {
            await _cookieService.UpdateCookie(CookieService.CookieUserName, UserName, 14);
        }
            
        CurrentUser = new User(userGuid, UserName, IsSpectator);
    }
    
    private static string GenerateGuid()
    {
        return Guid.NewGuid().ToString("N");
    }
    
    protected Task ClickOnCard(int cardValue)
    {
        if (!CanChooseCard) 
            return Task.CompletedTask;
        
        CurrentUser.CardValue = cardValue;
        
        ToggleCardSelection(cardValue);
        
        CanChooseCard = false;
        return _hubService.ClickOnCardAsync(RoomName, CurrentUser);
    }
    
    private void ToggleCardSelection(int cardValue)
    {
        if (SelectedCardValue == cardValue)
        {
            SelectedCardValue = null;
        }
        else
        {
            SelectedCardValue = cardValue;
        }

        StateHasChanged();
    }
    
    protected string GetCardCssClass(int cardValue)
    {
        if (SelectedCardValue != cardValue && SelectedCardValue is null)
        {
            return "card-unselected";
        }
        
        return SelectedCardValue == cardValue ? "card-selected" : "";
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
    
    protected class Statistics
    {
        public string Label { get; set; }
        public int Occurrence { get; set; }
        public string Color { get; set; }
    }

    protected List<Statistics> StatisticsDetails = new List<Statistics>
    {
        new Statistics { Label= "5", Occurrence = 1 ,Color="#498fff" },
        new Statistics { Label= "8", Occurrence = 3 ,Color="#ffa060" },
        new Statistics { Label= "13", Occurrence = 1 ,Color="#ff68b6" },
    };
}