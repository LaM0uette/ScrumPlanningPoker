using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ScrumPlanningPoker.Services;
using ScrumPlanningPoker.Services.HubServices;

namespace ScrumPlanningPoker.Components.Pages;

public class HomeBase : ComponentBase, IDisposable
{
    #region Statements

    protected string? RoomLink;
    
    [Inject] public IJSRuntime JSRuntime { get; init; } = default!;
    [Inject] public NavigationManager NavigationManager { get; init; } = default!;
    [Inject] public ThemeStateService ThemeState { get; init; } = default!;
    [Inject] private HubService _hubService { get; init; } = default!;

    #endregion

    #region Functions

    protected override void OnInitialized()
    {
        ThemeState.OnChange += StateHasChanged;
    }
    public void Dispose()
    {
        ThemeState.OnChange -= StateHasChanged;
        GC.SuppressFinalize(this);
    }

    protected override Task OnInitializedAsync()
    {
        return _hubService.InitializeConnectionAsync();
    }

    protected async void Create()
    {
        var roomName = GenerateUniqueRoomName();
        RoomLink = _hubService.GetBaseUri() + $"room/{roomName}";

        await _hubService.CreateRoomAsync(roomName);
    }

    protected async Task GoToRoomLink()
    {
        if (string.IsNullOrEmpty(RoomLink))
            return;
        
        await CopyRoomLink();
        NavigationManager.NavigateTo(RoomLink);
    }

    protected async Task CopyRoomLink()
    {
        await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", RoomLink);
    }

    private static string GenerateUniqueRoomName()
    {
        return Guid.NewGuid().ToString("N");
    }
    
    #endregion
}