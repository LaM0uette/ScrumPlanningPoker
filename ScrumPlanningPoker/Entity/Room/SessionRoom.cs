namespace ScrumPlanningPoker.Entity.Room;

public class SessionRoom(string name)
{
    public string Name { get; } = name;
    public bool CardsIsRevealed { get; set; }
    public List<User> Users { get; set; } = new();
}