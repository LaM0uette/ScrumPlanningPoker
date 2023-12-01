namespace ScrumPlanningPoker.Entity.RoomHub;

public class User(string guid, string name, bool isSpectator)
{
    #region Statements

    public string Guid { get; set; } = guid;
    public string Name { get; set; } = name;
    public bool IsSpectator { get; set; } = isSpectator;
    public int? CardValue { get; set; }

    #endregion
}