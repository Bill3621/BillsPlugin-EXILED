using Exiled.Permissions.Extensions;

namespace BillsPlugin.Handlers
{
    internal class Server
    {
        public void OnRoundStarted()
        {
            Player.ProximityChatPlayers.Clear();

            Updater.CheckForUpdate();
            if (!Updater.UpdateAvailable) return;
            foreach (var player in Exiled.API.Features.Player.List)
                if (player.CheckPermission(PlayerPermissions.ServerConfigs))
                    player.Broadcast(6, "BillsPlugin: An update is available.", Broadcast.BroadcastFlags.AdminChat);
        }
    }
}