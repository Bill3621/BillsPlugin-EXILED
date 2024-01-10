using Exiled.API.Enums;
using System;
using Exiled.API.Features;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Server = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;
using Map = Exiled.Events.Handlers.Map;
using Log = PluginAPI.Core.Log;

namespace BillsPlugin
{
    public class BillsPlugin : Plugin<Config>
    {

        private static readonly BillsPlugin Singleton = new BillsPlugin();
        public static BillsPlugin Instance => Singleton;

        public override PluginPriority Priority { get; } = PluginPriority.Medium;

        private Handlers.Player player;
        private Handlers.Server server;
        private Handlers.Map map;

        private BillsPlugin() { }

        public override void OnEnabled()
        {
            RegisterEvents();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
        }

        public void RegisterEvents()
        {
            player = new Handlers.Player();
            server = new Handlers.Server();
            map = new Handlers.Map();

            //Server.WaitingForPlayers += server.OnWaitingForPlayers;
            //Server.RoundStarted += server.OnRoundStarted;

            //Player.Left += player.OnLeft;
            //Player.Joined += player.OnJoined;
            //Player.InteractingDoor += player.OnInteractingDoor;
            Player.TriggeringTesla += player.OnTriggeringTesla;
            
            Map.ExplodingGrenade += map.OnExplodingGrenade;
        }

        public void UnregisterEvents()
        {
            //Server.WaitingForPlayers -= server.OnWaitingForPlayers;
            // Server.RoundStarted -= server.OnRoundStarted;

            //Player.Left -= player.OnLeft;
            //Player.Joined -= player.OnJoined;
            //Player.InteractingDoor -= player.OnInteractingDoor;
            Player.TriggeringTesla -= player.OnTriggeringTesla;

            Map.ExplodingGrenade -= map.OnExplodingGrenade;

            player = null;
            server = null;
            map = null;
        }

    }
}
