namespace ScrumPlanningPoker.Entity.RoomHub;

public class SessionRoom(Timer timer)
{
    public Timer Timer { get; set; } = timer;
    public List<string> Users { get; init; } = [];
}