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
using Scp079 = Exiled.Events.Handlers.Scp079;
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
        private Handlers.Scp079 scp079;

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
            scp079 = new Handlers.Scp079();

            //Server.WaitingForPlayers += server.OnWaitingForPlayers;
            //Server.RoundStarted += server.OnRoundStarted;

            //Player.Left += player.OnLeft;
            //Player.Joined += player.OnJoined;
            //Player.InteractingDoor += player.OnInteractingDoor;
            Player.TriggeringTesla += player.OnTriggeringTesla;
            
            Map.ExplodingGrenade += map.OnExplodingGrenade;

            Scp079.InteractingTesla += scp079.OnInteractingTesla;
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

            Scp079.InteractingTesla -= scp079.OnInteractingTesla;

            player = null;
            server = null;
            map = null;
            scp079 = null;
        }

    }
}
