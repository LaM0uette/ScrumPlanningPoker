namespace ScrumPlanningPoker.Entity.Room;

public class User(string guid, string name, bool isSpectator)
{
    #region Statements

    public string Guid { get; } = guid;
    public string Name { get; } = name;
    public bool IsSpectator { get; } = isSpectator;
    public int? CardValue { get; set; }

    #endregion
}