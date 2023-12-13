using Microsoft.AspNetCore.Components;
using MudBlazor;
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
    
    protected User CurrentUser = null!;
    protected List<User> Users = [];
    
    protected readonly ChartOptions PieChartOptions = new();
    
    [Inject] public ThemeStateService ThemeState { get; init; } = default!;
    [Inject] private CookieService _cookieService { get; init; } = default!;
    [Inject] private HubService _hubService { get; init; } = default!;

    #endregion

    #region Functions
    
    protected override void OnInitialized()
    {
        _hubService.OnUserUpdateRoom += HandleUserUpdateRoom;
        _hubService.OnRevealCards += HandleRevealCards;
        ThemeState.OnChange += StateHasChanged;
        
        PieChartOptions.ChartPalette = new []{ "#80b577", "#00a9e6", "#3070ba","#494331","#8464a7", "#5e358c", "#446f69", "#28a745", "#dc3545", "#09080b", "#826e45", "#d99a67", "#ffc107" };
    }
    
    protected override async Task OnInitializedAsync()
    {
        var cookieUserName = await _cookieService.GetCookie(CookieService.CookieUserName);
        if (cookieUserName != null)
        {
            UserName = cookieUserName;
        }

        await _hubService.InitializeConnectionAsync();
        await CheckRoomExists();
    }
    
    private Task CheckRoomExists()
    {
        return _hubService.CheckRoomExistsAsync(RoomName);
    }

    private void HandleUserUpdateRoom(SessionRoom room)
    {
        if (RoomName != room.Name)
            return;
        
        CardsIsRevealed = room.CardsIsRevealed;
                
        Users = room.Users;

        InvokeAsync(StateHasChanged);
    }
    
    private void HandleRevealCards(SessionRoom room, bool reveal)
    {
        if (RoomName != room.Name)
            return;
        
        CardsIsRevealed = reveal;
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
        if (cardValue == CurrentUser.CardValue)
            return Task.CompletedTask;
        
        CurrentUser.CardValue = cardValue;
        ToggleCardSelection(cardValue);
        
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
        ThemeState.OnChange -= StateHasChanged;
        
        await LeaveRoom();
        
        GC.SuppressFinalize(this);
    }
    
    private Task LeaveRoom()
    {
        return _hubService.LeaveRoomAsync(RoomName, CurrentUser);
    }

    #endregion
}