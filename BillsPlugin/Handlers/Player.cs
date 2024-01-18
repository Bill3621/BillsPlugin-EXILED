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
            if (!BillsPlugin.Instance.Config.ProximityChatAllowedRoles.Contains(ev.Player.Role)) return;

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
            if (!BillsPlugin.Instance.Config.ProximityChatAllowedRoles.Contains(ev.Player.Role)) return;

            SendProximityMessage(ev.VoiceMessage);
        }

        private static void SendProximityMessage(VoiceMessage msg)
        {
            foreach (var referenceHub in ReferenceHub.AllHubs)
            {
                if (referenceHub.roleManager.CurrentRole is SpectatorRole && !msg.Speaker.IsSpectatedBy(referenceHub))
                    continue;

                if (!(referenceHub.roleManager.CurrentRole is IVoiceRole))
                    continue;
                var voiceRole2 = (IVoiceRole)referenceHub.roleManager.CurrentRole;

                if (Vector3.Distance(msg.Speaker.transform.position, referenceHub.transform.position) >=
                    BillsPlugin.Instance.Config.ProximityChatDistance)
                    continue;

                if (voiceRole2.VoiceModule.ValidateReceive(msg.Speaker, VoiceChatChannel.Proximity) is VoiceChatChannel
                        .None)
                    continue;

                msg.Channel = VoiceChatChannel.Proximity;

                referenceHub.connectionToClient.Send(msg);
            }
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            if (BillsPlugin.Instance.Config.DisableGodModeOnTeamChange) ev.Player.IsGodModeEnabled = false;
            if (!BillsPlugin.Instance.Config.ProximityChatAllowedRoles.Contains(ev.Player.Role)) return;
            ev.Player.Broadcast(5, BillsPlugin.Instance.Config.ProximityChatBroadcastMessage);
        }
    }
}