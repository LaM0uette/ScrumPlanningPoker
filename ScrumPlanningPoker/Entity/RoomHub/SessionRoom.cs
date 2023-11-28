namespace ScrumPlanningPoker.Entity.RoomHub;

public class SessionRoom(string name)
{
    public string Name { get; set; } = name;
    public List<User> Users { get; set; } = [];
}