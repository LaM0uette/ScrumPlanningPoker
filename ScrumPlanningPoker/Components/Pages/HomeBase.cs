using Microsoft.AspNetCore.Components;
using ScrumPlanningPoker.Services.HubServices;

namespace ScrumPlanningPoker.Components.Pages;

public class HomeBase : ComponentBase
{
    #region Statements

    protected string? RoomLink;
    
    [Inject] private HubService _hubService { get; init; } = default!;

    #endregion

    #region Functions
    
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

    private static string GenerateUniqueRoomName()
    {
        return Guid.NewGuid().ToString("N");
    }
    
    #endregion
}