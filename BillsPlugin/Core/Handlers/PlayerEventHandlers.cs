using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.Ragdolls;
using PlayerRoles.Spectating;
using PlayerRoles.Voice;
using UnityEngine;
using VoiceChat;
using VoiceChat.Codec.Enums;
using VoiceChat.Codec;
using VoiceChat.Networking;

namespace BillsPlugin.Core.Handlers;

internal class PlayerEventHandlers
{
    private readonly Config _config;

#nullable enable
    internal PlayerEventHandlers(Config? global)
    {
        _config = global ?? new Config();
    }

    public static readonly HashSet<EPlayer> ProximityChatPlayers = [];

    public EPlayer? GetRandomSpectator()
    {
        var random = new System.Random();
        var spectator = EPlayer.List
            .OrderBy(item => random.Next())
            .FirstOrDefault(player => player.Role == RoleTypeId.Spectator);

        return spectator;
    }

    public void OnLeaving(LeftEventArgs ev)
    {
        if (!_config.ReplaceScpOnLeave) return;
        var team = ev.Player.ReferenceHub.GetRoleId();
        if (team.GetSide() != Side.Scp) return;
        if (!_config.ReplaceZombies && team == RoleTypeId.Scp0492) return;

        var replacingPlayer = GetRandomSpectator();
        if (replacingPlayer == null) return;

        MEC.Timing.CallDelayed(0.25f, () =>
        {

            BasicRagdoll[] array = [.. (from r in UnityEngine.Object.FindObjectsOfType<BasicRagdoll>()
                                    orderby r.Info.CreationTime descending
                                    select r)];

            foreach (var ragdoll in array)
            {
                if (ragdoll.Info.OwnerHub != ev.Player.ReferenceHub)
                {
                    continue;
                }
                NetworkServer.Destroy(ragdoll.gameObject);
                break;
            }
        });

        replacingPlayer.ReferenceHub.roleManager.ServerSetRole(team, RoleChangeReason.Respawn, RoleSpawnFlags.None);
        replacingPlayer.ArtificialHealth = ev.Player.ArtificialHealth;
        replacingPlayer.Health = ev.Player.Health;
        replacingPlayer.Teleport(ev.Player.Position);
        replacingPlayer.Broadcast(6, "<b>You have replaced a player who disconnected.</b>");
        foreach (var p in EPlayer.List)
        {
            if (p == replacingPlayer) continue;
            p.Broadcast(6, "<b>An SCP has been replaced.</b>");
        }
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (!_config.CuffedNoGunDamage) return;
        if (!ev.Player.IsCuffed) return;
        if (!ev.DamageHandler.Type.IsWeapon()) return;

        ev.IsAllowed = false;
    }

    public void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
    {
        if (ev.Tesla.IsShocking) return;

        if (!_config.TeslaGateBypass.Contains(ev.Player.RoleManager.CurrentRole.RoleTypeId))
            return;

        ev.IsTriggerable = false;
        ev.IsInIdleRange = false;
    }

    public void OnTogglingNoClip(TogglingNoClipEventArgs ev)
    {
        if (ev.Player.IsNoclipPermitted) return;
        if (!_config.ProximityChatAllowedRoles.Contains(ev.Player.Role.Type)) return;

        ev.IsAllowed = false;

        ToggleProximityChat(ev);
    }

    private void ToggleProximityChat(TogglingNoClipEventArgs ev)
    {
        if (!ProximityChatPlayers.Add(ev.Player))
        {
            ProximityChatPlayers.Remove(ev.Player);
            ev.Player.ShowHint(_config.ProximityChatDisabledMessage, 5);
        }
        else
        {
            ev.Player.ShowHint(_config.ProximityChatEnabledMessage, 5);
        }
    }

    public void OnVoiceChatting(VoiceChattingEventArgs ev)
    {
        if (ev.VoiceMessage.Channel != VoiceChatChannel.ScpChat) return;
        if (!_config.ProximityChatAllowedRoles.Contains(ev.Player.Role.Type)) return;
        if (!ProximityChatPlayers.Contains(ev.Player)) return;

        SendProximityMessage(ev.VoiceMessage);
        ev.IsAllowed = false;
    }

    private void SendProximityMessage(VoiceMessage msg)
    {
        msg.Channel = VoiceChatChannel.Proximity;

        msg.SendToSpectatorsOf(msg.Speaker);

        foreach (var hub in FilterHubsByRole(msg))
        {
            if (!IsWithinProximity(msg, hub))
                continue;

            if (!CanReceiveMessage(msg, hub))
                continue;

            var clonedMessage = CloneMessageWithAdjustedVolume(msg, hub);

            hub.connectionToClient.Send(clonedMessage);
            clonedMessage.SendToSpectatorsOf(hub);
        }
    }

    private static IEnumerable<ReferenceHub> FilterHubsByRole(VoiceMessage msg)
    {
        return ReferenceHub.AllHubs.Where(hub =>
            !(hub.roleManager.CurrentRole is SpectatorRole && !msg.Speaker.IsSpectatedBy(hub)) &&
            hub.roleManager.CurrentRole is IVoiceRole);
    }

    private bool IsWithinProximity(VoiceMessage msg, ReferenceHub hub)
    {
        var distance = Vector3.Distance(msg.Speaker.transform.position, hub.transform.position);
        return distance < _config.ProximityChatDistance;
    }

    private static bool CanReceiveMessage(VoiceMessage msg, ReferenceHub hub)
    {
        if (hub.roleManager.CurrentRole is not IVoiceRole voiceRole)
            return false;

        return voiceRole.VoiceModule.ValidateReceive(msg.Speaker, VoiceChatChannel.Proximity) !=
               VoiceChatChannel.None;
    }

    public OpusEncoder Encoder = new(OpusApplicationType.Voip);
    public OpusDecoder Decoder = new();

    private VoiceMessage CloneMessageWithAdjustedVolume(VoiceMessage msg, ReferenceHub hub)
    {
        var clonedMsg = new VoiceMessage
        {
            Data = (byte[])msg.Data.Clone(),
            DataLength = msg.DataLength,
            Channel = msg.Channel,
            Speaker = msg.Speaker,
            SpeakerNull = msg.SpeakerNull
        };

        if (hub.GetRoleId() == RoleTypeId.Spectator || hub.GetRoleId() == RoleTypeId.Overwatch) return clonedMsg;

        var message = new float[48000];
        Decoder.Decode(clonedMsg.Data, clonedMsg.DataLength, message);

        var volumeFactor = 1f - Vector3.Distance(msg.Speaker.transform.position, hub.transform.position) /
            _config.ProximityChatDistance;
        for (var i = 0; i < message.Length; i++) message[i] *= volumeFactor;

        clonedMsg.DataLength = Encoder.Encode(message, clonedMsg.Data);

        return clonedMsg;
    }

    public void OnSpawned(SpawnedEventArgs ev)
    {
        if (ev.Player == null) return;

        var broadcast = _config.RoleSpawnBroadcasts?.FirstOrDefault(broad => broad.Role == ev.Player.Role.Type);
        if (broadcast != null)
        {
            Timing.CallDelayed(broadcast.BroadcastDelay, () =>
            {
                if (broadcast.IsHintInstead)
                {
                    ev.Player.ShowHint(broadcast.BroadcastMessage, broadcast.TimeShown);
                }
                else
                {
                    ev.Player.Broadcast(broadcast.TimeShown, broadcast.BroadcastMessage);
                }
            });
        }

        if (ev.Player.Role != null && ev.Player.Role != RoleTypeId.None)
        {
            if (_config.DisableGodModeOnSpawn) ev.Player.IsGodModeEnabled = false;
            if (!_config.ProximityChatAllowedRoles.Contains(ev.Player.Role.Type)) return;
        }
        ev.Player.Broadcast(5, _config.ProximityChatBroadcastMessage);
    }
}