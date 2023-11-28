namespace ScrumPlanningPoker.Services.HubServices;

public interface IRoomService
{
    Task CreateRoomAsync(string roomName);
    Task JoinRoomAsync(string roomName, string userName);
    Task LeaveRoomAsync(string roomName, string userName);
}