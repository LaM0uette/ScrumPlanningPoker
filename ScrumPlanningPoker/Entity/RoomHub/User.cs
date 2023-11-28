namespace ScrumPlanningPoker.Entity.RoomHub;

public class User(string name, string role)
{
    public string Name { get; set; } = name;
    public string Role { get; set; } = role;
    public int Vote { get; set; } = -1;
}