using PlayerRoles;

namespace BillsPlugin.Core.Classes;

public class RoleSpawnBroadcast(RoleTypeId role, string broadcastMessage, ushort broadcastDelay, ushort timeShown, bool isHintInstead)
{
    public RoleTypeId Role { get; set; } = role;
    public string BroadcastMessage { get; set; } = broadcastMessage;
    public ushort BroadcastDelay { get; set; } = broadcastDelay;
    public ushort TimeShown { get; set; } = timeShown;
    public bool IsHintInstead { get; set; } = isHintInstead;

    public RoleSpawnBroadcast()
        : this(RoleTypeId.None, "", 0, 5, false)
    {
    }
}