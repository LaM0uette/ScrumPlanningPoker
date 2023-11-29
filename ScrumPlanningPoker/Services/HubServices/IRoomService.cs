namespace ScrumPlanningPoker.Services.HubServices;

public interface IRoomService
{
    Task CreateRoomAsync(string roomName);
    Task JoinRoomAsync(string roomName, string userName, bool isSpectator);
    Task LeaveRoomAsync(string roomName, string userName);
}