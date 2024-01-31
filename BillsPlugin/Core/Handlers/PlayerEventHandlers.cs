using System.Collections.Generic;
using System.Linq;
using BillsPlugin.Core.Classes;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using PlayerRoles.Spectating;
using PlayerRoles.Voice;
using UnityEngine;
using VoiceChat;
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

    public static readonly HashSet<Exiled.API.Features.Player> ProximityChatPlayers = [];

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

        foreach (var hub in FilterHubsByRole(msg))
        {
            if (!IsWithinProximity(msg, hub))
                continue;

            if (!CanReceiveMessage(msg, hub))
                continue;

            var clonedMessage = CloneMessageWithAdjustedVolume(msg, hub);

            //hub.connectionToClient.Send<VoiceMessage>(clonedMessage, 0);
            hub.connectionToClient.Send(clonedMessage);
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

        var player = Exiled.API.Features.Player.Get(msg.Speaker);
        var opusComponent = OpusComponent.Get(player.ReferenceHub, hub);

        var message = new float[48000];
        opusComponent.Decoder.Decode(clonedMsg.Data, clonedMsg.DataLength, message);

        opusComponent.ChangeVolume(
            1f - Vector3.Distance(msg.Speaker.transform.position, hub.transform.position) /
            _config.ProximityChatDistance, message);

        //clonedMsg.DataLength = opusComponent.Encoder.Encode(message, clonedMsg.Data, 480);
        clonedMsg.DataLength = opusComponent.Encoder.Encode(message, clonedMsg.Data);

        return clonedMsg;
    }

    public void OnSpawned(SpawnedEventArgs ev)
    {
        // I don't know why but the player was null once??
        if (ev.Player == null) return;

        var broadcast = _config.RoleSpawnBroadcasts.FirstOrDefault(broad => broad.Role == ev.Player.Role.Type);
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

        if (_config.DisableGodModeOnSpawn) ev.Player.IsGodModeEnabled = false;
        if (!_config.ProximityChatAllowedRoles.Contains(ev.Player.Role.Type)) return;
        ev.Player.Broadcast(5, _config.ProximityChatBroadcastMessage);
    }
}