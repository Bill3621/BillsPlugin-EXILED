using System;
using System.Text;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Permissions.Extensions;
using MEC;
using PlayerRoles;

namespace BillsPlugin.Core.Handlers;
internal class ServerEventHandlers
{
    private static Config _config;

#nullable enable
    internal ServerEventHandlers(Config? global)
    {
        _config = global ?? new Config();
    }

    private CoroutineHandle _handle;

    public readonly Action ScanFacility = () =>
    {
        Log.Debug("Starting FacilityScan.");
        var fail = new Random().Next(1, 101) <= _config.FacilityScanFailChance ||
                   (Warhead.IsDetonated && _config.FacilityScanFailAlphaWarhead) ||
                   Players.GetPlayers().TrueForAll(player => !player.IsAlive);


        Log.Debug($"Fail: {fail}");
        if (fail && _config.FacilityScanFailNoAnnouncements) return;

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

        foreach (var player in Players.GetPlayers())
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
        Log.Debug($"Raw Message String: {messageString}");
        var subtitleString = messageString.Replace("Detected", "Detected:").Replace(" .", ", ").Trim();
        subtitleString = subtitleString.Remove(subtitleString.Length - 1);

        // Formatting
        subtitleString = subtitleString.Replace("Failure", "<color=red>Failure</color>");
        subtitleString = subtitleString.Replace("Class D", "<color=orange>Class-D</color>");
        subtitleString = subtitleString.Replace("Scientists", "<color=yellow>Scientists</color>");
        subtitleString = subtitleString.Replace("Scientist", "<color=yellow>Scientist</color>");
        subtitleString = subtitleString.Replace("Security Agents", "<color=blue>Security Agents</color>");
        subtitleString = subtitleString.Replace("Security Agent", "<color=blue>Security Agent</color>");
        subtitleString = subtitleString.Replace("Chaos Insurgency", "<color=green>Chaos Insurgency</color>");
        subtitleString = subtitleString.Replace("SCPs", "<color=red>SCPs</color>");
        subtitleString = subtitleString.Replace("SCP", "<color=red>SCP</color>");

        Cassie.MessageTranslated(messageString, subtitleString, isNoisy: false);
        //RespawnEffectsController.PlayCassieAnnouncement(builder.ToString(), false, false, true);
    };

    public void OnRestartingRound()
    {
        Timing.Instance.KillCoroutinesOnInstance(_handle);
    }

    public static void BroadcastStaff(string messageString)
    {
        foreach (var player in EPlayer.List)
            if (player.CheckPermission(PlayerPermissions.ServerConfigs))
                player.Broadcast(6, messageString, Broadcast.BroadcastFlags.AdminChat);
    }

    private int checkUpdateCount = 0;
    public void OnRoundStarted()
    {
        PlayerEventHandlers.ProximityChatPlayers.Clear();

        if (_config.FacilityScanTime > 0)
            // Timeframe is a large value, so it's basically infinite long.
            _handle = Timing.CallPeriodically(15_000f * 500, _config.FacilityScanTime * 60f,
                ScanFacility);

        checkUpdateCount++;
        if (checkUpdateCount < 5)
        {
            return;
        }
        checkUpdateCount = 0;

        Updater.CheckForUpdate();
        if (!Updater.UpdateAvailable) return;
        if (Updater.InstalledAutomatically)
        {
            if (_config.AutoRestart)
            {
                BroadcastStaff("BillsPlugin: An update has been installed. Applied next round.");
                return;
            }
            BroadcastStaff("BillsPlugin: An update has been installed. Applied on next restart.");
        }
        else if (_config.AutoUpdate)
        {
            BroadcastStaff("BillsPlugin: An update is available. Trying to install it.");
        }
        else
        {
            BroadcastStaff("BillsPlugin: An update is available.");
        }
    }
}
