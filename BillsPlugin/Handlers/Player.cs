using System.Collections.Generic;
using System.Linq;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Spectating;
using PlayerRoles.Voice;
using UnityEngine;
using VoiceChat;
using VoiceChat.Networking;

namespace BillsPlugin.Handlers
{
    internal class Player
    {
        public static readonly HashSet<Exiled.API.Features.Player> ProximityChatPlayers =
            new HashSet<Exiled.API.Features.Player>();

        public void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (ev.Tesla.IsShocking) return;

            if (!BillsPlugin.Instance.Config.TeslaGateBypass.Contains(ev.Player.RoleManager.CurrentRole.RoleTypeId))
                return;

            ev.IsTriggerable = false;
            ev.IsInIdleRange = false;
        }

        public void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            if (!IsNoClipPermitted(ev)) return;

            if (!IsProximityChatAllowed(ev)) return;

            ev.IsAllowed = false;

            ToggleProximityChat(ev);
        }

        private bool IsNoClipPermitted(TogglingNoClipEventArgs ev)
        {
            return ev.Player.IsNoclipPermitted;
        }

        private bool IsProximityChatAllowed(TogglingNoClipEventArgs ev)
        {
            return BillsPlugin.Instance.Config.ProximityChatAllowedRoles.Contains(ev.Player.Role.Type);
        }

        private void ToggleProximityChat(TogglingNoClipEventArgs ev)
        {
            if (!ProximityChatPlayers.Add(ev.Player))
            {
                ProximityChatPlayers.Remove(ev.Player);
                ev.Player.ShowHint(BillsPlugin.Instance.Config.ProximityChatDisabledMessage, 5);
            }
            else
            {
                ev.Player.ShowHint(BillsPlugin.Instance.Config.ProximityChatEnabledMessage, 5);
            }
        }

        public void OnVoiceChatting(VoiceChattingEventArgs ev)
        {
            if (ev.VoiceMessage.Channel != VoiceChatChannel.ScpChat) return;
            if (!BillsPlugin.Instance.Config.ProximityChatAllowedRoles.Contains(ev.Player.Role.Type)) return;
            if (!ProximityChatPlayers.Contains(ev.Player)) return;

            SendProximityMessage(ev.VoiceMessage);
            ev.IsAllowed = false;
        }

        private static void SendProximityMessage(VoiceMessage msg)
        {
            UpdateVoiceMessageChannel(msg);

            foreach (var hub in FilterHubsByRole(msg))
            {
                if (!IsWithinProximity(msg, hub))
                    continue;

                if (!CanReceiveMessage(msg, hub))
                    continue;

                var clonedMessage = CloneMessageWithAdjustedVolume(msg, hub);

                SendClonedMessage(clonedMessage, hub);
            }
        }

        private static void UpdateVoiceMessageChannel(VoiceMessage msg)
        {
            msg.Channel = VoiceChatChannel.Proximity;
        }

        private static IEnumerable<ReferenceHub> FilterHubsByRole(VoiceMessage msg)
        {
            return ReferenceHub.AllHubs.Where(hub =>
                !(hub.roleManager.CurrentRole is SpectatorRole && !msg.Speaker.IsSpectatedBy(hub)) &&
                hub.roleManager.CurrentRole is IVoiceRole);
        }

        private static bool IsWithinProximity(VoiceMessage msg, ReferenceHub hub)
        {
            var distance = Vector3.Distance(msg.Speaker.transform.position, hub.transform.position);
            return distance < BillsPlugin.Instance.Config.ProximityChatDistance;
        }

        private static bool CanReceiveMessage(VoiceMessage msg, ReferenceHub hub)
        {
            if (!(hub.roleManager.CurrentRole is IVoiceRole voiceRole))
                return false;

            return voiceRole.VoiceModule.ValidateReceive(msg.Speaker, VoiceChatChannel.Proximity) !=
                   VoiceChatChannel.None;
        }

        private static VoiceMessage CloneMessageWithAdjustedVolume(VoiceMessage msg, ReferenceHub hub)
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
                BillsPlugin.Instance.Config.ProximityChatDistance, message);

            clonedMsg.DataLength = opusComponent.Encoder.Encode(message, clonedMsg.Data, 480);

            return clonedMsg;
        }

        private static void SendClonedMessage(VoiceMessage msg, ReferenceHub hub)
        {
            hub.connectionToClient.Send<VoiceMessage>(msg, 0);
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            // I don't know why but the player was null once??
            if (ev.Player == null) return;
            if (BillsPlugin.Instance.Config.DisableGodModeOnTeamChange) ev.Player.IsGodModeEnabled = false;
            if (!BillsPlugin.Instance.Config.ProximityChatAllowedRoles.Contains(ev.Player.Role.Type)) return;
            ev.Player.Broadcast(5, BillsPlugin.Instance.Config.ProximityChatBroadcastMessage);
        }
    }
}