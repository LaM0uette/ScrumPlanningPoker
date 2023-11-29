namespace ScrumPlanningPoker.Entity.RoomHub;

public class User(string guid, string name, string role)
{
    public string Guid { get; set; } = guid;
    public string Name { get; set; } = name;
    public string Role { get; set; } = role;
    public int Vote { get; set; } = -1;
}