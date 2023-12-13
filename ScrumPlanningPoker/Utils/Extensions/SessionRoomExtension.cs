using ScrumPlanningPoker.Entity.Room;

namespace ScrumPlanningPoker.Utils.Extensions;

public static class SessionRoomExtension
{
    public static void SortUsers(this SessionRoom sessionRoom)
    {
        sessionRoom.Users = sessionRoom.Users.OrderBy(u => u.Name).ToList();
    }
}