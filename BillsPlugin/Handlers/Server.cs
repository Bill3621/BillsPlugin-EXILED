using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.Permissions.Extensions;

namespace BillsPlugin.Handlers
{
    class Server
    {
        /*public void OnWaitingForPlayers()
        {
            Log.Info("Waiting for players...");
        }*/

        public void OnRoundStarted()
        {
            //Map.Broadcast(6, TutorialPlugin.Instance.Config.RoundStartedMessage);
            Updater.CheckForUpdate();
            if (!Updater.UpdateAvailable)
            {
                return;
            }
            foreach (var player in Exiled.API.Features.Player.List)
            {
                if (player.CheckPermission(PlayerPermissions.ServerConfigs))
                {
                    player.Broadcast(6, "BillsPlugin: An update is available.", Broadcast.BroadcastFlags.AdminChat);
                }
            }
        }
    }
}
