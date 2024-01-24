using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Server = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;
using Map = Exiled.Events.Handlers.Map;
using Scp079 = Exiled.Events.Handlers.Scp079;

namespace BillsPlugin
{
    public class BillsPlugin : Plugin<Config>
    {
        public static BillsPlugin Instance { get; } = new BillsPlugin();

        public override PluginPriority Priority { get; } = PluginPriority.Medium;
        public override Version Version { get; } = new Version(0, 0, 9);

        public Dictionary<ReferenceHub, OpusComponent> Encoders = new Dictionary<ReferenceHub, OpusComponent>();

        private Handlers.Player _player;
        private Handlers.Server _server;
        private Handlers.Map _map;
        private Handlers.Scp079 _scp079;

        private BillsPlugin()
        {
        }

        public override void OnEnabled()
        {
            base.OnEnabled();

            RegisterEvents();

            Updater.CheckForUpdate();
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            UnregisterEvents();
        }

        public void RegisterEvents()
        {
            _player = new Handlers.Player();
            _server = new Handlers.Server();
            _map = new Handlers.Map();
            _scp079 = new Handlers.Scp079();

            Server.RoundStarted += _server.OnRoundStarted;
            Server.RestartingRound += _server.OnRestartingRound;
            Server.WaitingForPlayers += _server.OnWaitingForPlayers;

            Player.TriggeringTesla += _player.OnTriggeringTesla;
            Player.Spawned += _player.OnSpawned;
            Player.VoiceChatting += _player.OnVoiceChatting;
            Player.TogglingNoClip += _player.OnTogglingNoClip;

            Map.ExplodingGrenade += _map.OnExplodingGrenade;

            Scp079.InteractingTesla += _scp079.OnInteractingTesla;
        }

        public void UnregisterEvents()
        {
            Server.RoundStarted -= _server.OnRoundStarted;
            Server.RestartingRound -= _server.OnRestartingRound;
            Server.WaitingForPlayers -= _server.OnWaitingForPlayers;

            Player.TriggeringTesla -= _player.OnTriggeringTesla;
            Player.Spawned -= _player.OnSpawned;
            Player.VoiceChatting -= _player.OnVoiceChatting;
            Player.TogglingNoClip -= _player.OnTogglingNoClip;

            Map.ExplodingGrenade -= _map.OnExplodingGrenade;

            Scp079.InteractingTesla -= _scp079.OnInteractingTesla;

            _player = null;
            _server = null;
            _map = null;
            _scp079 = null;
        }
    }
}