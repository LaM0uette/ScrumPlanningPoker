using System.Text.Json.Serialization;

namespace ScrumPlanningPoker.Entity.RoomHub;

public enum Role
{
    User,
    Spectator
}

public class User(string guid, string name, string roleName)
{
    #region Statements

    public string Guid { get; set; } = guid;
    public string Name { get; set; } = name;
    public string RoleName { get; set; } = roleName;
    public int CardValue { get; set; } = -1;

    [JsonIgnore] public Role Role { get; set; } = GetRole(roleName);

    #endregion

    #region Functions

    private static Role GetRole(string roleName)
    {
        return roleName switch
        {
            "user" => Role.User,
            "spectator" => Role.Spectator,
            _ => Role.User
        };
    }

    #endregion
}