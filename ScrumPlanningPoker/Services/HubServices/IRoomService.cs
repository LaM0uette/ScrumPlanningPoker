namespace ScrumPlanningPoker.Services.HubServices;

public interface IRoomService
{
    Task CreateRoomAsync(string roomName);
    Task JoinRoomAsync(string roomName, string guid, string userName, bool isSpectator);
    Task LeaveRoomAsync(string roomName, string userName);
}