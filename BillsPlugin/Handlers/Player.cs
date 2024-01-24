using System.Collections.Generic;
using Exiled.Events.EventArgs.Player;
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

            if (BillsPlugin.Instance.Config.TeslaGateBypass.Contains(ev.Player.RoleManager.CurrentRole.RoleTypeId))
            {
                ev.IsTriggerable = false;
                ev.IsInIdleRange = false;
            }
        }

        public void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            if (FpcNoclip.IsPermitted(ev.Player.ReferenceHub)) return;
            if (!BillsPlugin.Instance.Config.ProximityChatAllowedRoles.Contains(ev.Player.Role.Type)) return;

            if (!ProximityChatPlayers.Add(ev.Player))
            {
                ProximityChatPlayers.Remove(ev.Player);
                ev.Player.ShowHint(BillsPlugin.Instance.Config.ProximityChatDisabledMessage, 5);
                ev.IsAllowed = false;
                return;
            }

            ev.Player.ShowHint(BillsPlugin.Instance.Config.ProximityChatEnabledMessage, 5);
            ev.IsAllowed = false;
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
            msg.Channel = VoiceChatChannel.Proximity;
            var plr = Exiled.API.Features.Player.Get(msg.Speaker);
            foreach (var referenceHub in ReferenceHub.AllHubs)
            {
                if (referenceHub.roleManager.CurrentRole is SpectatorRole && !msg.Speaker.IsSpectatedBy(referenceHub))
                    continue;

                if (!(referenceHub.roleManager.CurrentRole is IVoiceRole))
                    continue;
                var voiceRole2 = (IVoiceRole)referenceHub.roleManager.CurrentRole;

                var distance = Vector3.Distance(msg.Speaker.transform.position, referenceHub.transform.position);
                if (distance >=
                    BillsPlugin.Instance.Config.ProximityChatDistance)
                    continue;

                if (voiceRole2.VoiceModule.ValidateReceive(msg.Speaker, VoiceChatChannel.Proximity) is VoiceChatChannel
                        .None)
                    continue;

                var clone = new VoiceMessage
                {
                    Data = (byte[])msg.Data.Clone(),
                    DataLength = msg.DataLength,
                    Channel = msg.Channel,
                    Speaker = msg.Speaker,
                    SpeakerNull = msg.SpeakerNull
                };

                var message = new float[48000];
                var comp = OpusComponent.Get(plr.ReferenceHub);
                comp.Decoder.Decode(clone.Data, clone.DataLength, message);

                comp.ChangeVolume(1f - distance / BillsPlugin.Instance.Config.ProximityChatDistance, message);

                clone.DataLength = comp.Encoder.Encode(message, clone.Data, 480);

                referenceHub.connectionToClient.Send<VoiceMessage>(clone, 0);
            }
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