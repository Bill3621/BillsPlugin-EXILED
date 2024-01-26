using System;
using System.Diagnostics;
using System.Text;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using PlayerRoles;
using Respawning;

namespace BillsPlugin.Handlers
{
    internal class Server
    {
        private CoroutineHandle _handle;

        public static readonly Action ScanFacility = () =>
        {
            Log.Debug("Starting FacilityScan.");
            var fail = new Random().Next(1, 101) <= BillsPlugin.Instance.Config.FacilityScanFailChance ||
                       (Warhead.IsDetonated && BillsPlugin.Instance.Config.FacilityScanFailAlphaWarhead);

            Log.Debug($"Fail: {fail}");
            if (fail && BillsPlugin.Instance.Config.FacilityScanFailNoAnnouncements) return;

            //RespawnEffectsController.PlayCassieAnnouncement("Scanning Facility . .", false, false, true);
            Cassie.MessageTranslated("Scanning Facility . .", "Scanning Facility...", isNoisy: false);
            if (fail)
            {
                Cassie.MessageTranslated("Facility Scan failed", "Facility Scan <color=red>failed</color>.", isNoisy: false);
                //RespawnEffectsController.PlayCassieAnnouncement("Facility Scan failed", false, false, true);
                return;
            }

            var builder = new StringBuilder();
            builder.Append("Detected ");

            byte classD = 0;
            byte scientists = 0;
            byte security = 0;
            byte scp = 0;
            byte chaos = 0;

            foreach (var player in PluginAPI.Core.Player.GetPlayers())
                if (player.Team == Team.Scientists) scientists++;
                else if (player.Team == Team.ClassD) classD++;
                else if (player.Team.GetSide() == Side.Mtf) security++;
                else if (player.Team.GetSide() == Side.Scp) scp += 1;
                else if (player.Team.GetSide() == Side.ChaosInsurgency) chaos++;

            if (classD != 0) builder.Append(classD + " Class D . ");
            if (scientists != 0) builder.Append(scientists + $" Scientist{(scientists > 1 ? "s" : "")} . ");
            if (security != 0) builder.Append(security + $" Security Agent{(security > 1 ? "s" : "")} . ");
            if (chaos != 0) builder.Append(chaos + " Chaos Insurgency . ");
            if (scp != 0) builder.Append(scp + $" SCP{(scp > 1 ? "s" : "")} . ");

            var messageString = builder.ToString().Trim();
            if (messageString.Equals("Detected")) messageString = "Scan Failure .";
            Log.Debug($"Raw Message String: {messageString}");
            var subtitleString = messageString.Replace("Detected", "Detected:").Replace(" .", ", ").Trim();
            subtitleString = subtitleString.Remove(subtitleString.Length - 1);

            // Formatting
            subtitleString = subtitleString.Replace("Failure", "<color=red>Failure</color>");
            subtitleString = subtitleString.Replace("Class D", "<color=orange>Class-D</color>");
            subtitleString = subtitleString.Replace("Scientist", "<color=yellow>Scientist</color>");
            subtitleString = subtitleString.Replace("Scientists", "<color=yellow>Scientists</color>");
            subtitleString = subtitleString.Replace("Security Agent", "<color=blue>Security Agent</color>");
            subtitleString = subtitleString.Replace("Security Agents", "<color=blue>Security Agents</color>");
            subtitleString = subtitleString.Replace("Chaos Insurgency", "<color=green>Chaos Insurgency</color>");
            subtitleString = subtitleString.Replace("SCP", "<color=red>SCP</color>");
            subtitleString = subtitleString.Replace("SCPs", "<color=red>SCPs</color>");

            Cassie.MessageTranslated(messageString, subtitleString, isNoisy: false);
            //RespawnEffectsController.PlayCassieAnnouncement(builder.ToString(), false, false, true);
        };

        public void OnRestartingRound()
        {
            Timing.Instance.KillCoroutinesOnInstance(_handle);
        }

        public void OnRoundStarted()
        {
            Player.ProximityChatPlayers.Clear();

            if (BillsPlugin.Instance.Config.FacilityScanTime > 0)
                // Timeframe is a large value, so it's basically infinite long.
                _handle = Timing.CallPeriodically(15_000f * 500, BillsPlugin.Instance.Config.FacilityScanTime * 60f,
                    ScanFacility);

            Updater.CheckForUpdate();
            if (!Updater.UpdateAvailable) return;
            foreach (var player in Exiled.API.Features.Player.List)
                if (player.CheckPermission(PlayerPermissions.ServerConfigs))
                    player.Broadcast(6, "BillsPlugin: An update is available.", Broadcast.BroadcastFlags.AdminChat);
        }

        public void OnWaitingForPlayers()
        {
            BillsPlugin.Instance.Encoders.Clear();
        }
    }
}