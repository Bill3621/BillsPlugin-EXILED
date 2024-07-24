using System;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using PlayerRoles;
using PluginAPI.Core;

namespace BillsPlugin.Core.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal class GameStats : ICommand
{
    public string Command { get; } = "gamestats";

    public string[] Aliases { get; } = [];

    public string Description { get; } =
        "Returns stats of the current game";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.GameplayData, out response)) return false;

        byte chaosInsurgencyAlive = 0;
        byte classDAlive = 0;
        byte mtfAndGuardsAlive = 0;
        byte scpsAlive = 0;
        byte zombiesAlive = 0;

        foreach (var player in Players.GetPlayers())
        {
            if (player.Team == Team.Scientists) continue;
            if (player.Team == Team.ClassD)
            {
                classDAlive += 1;
                continue;
            }

            if (player.Role == RoleTypeId.Scp0492)
            {
                zombiesAlive += 1;
                continue;
            }

            if (player.Team.GetSide() == Side.ChaosInsurgency)
                chaosInsurgencyAlive += 1;
            else if (player.Team.GetSide() == Side.Mtf)
                mtfAndGuardsAlive += 1;
            else if (player.Team.GetSide() == Side.Scp) scpsAlive += 1;
        }

        response = "\n"
                   + $"There {(Players.Count < 2 ? "is" : "are")} {Players.Count} player{(Players.Count < 2 ? "" : "s")} in the server currently.\n"
                   + $"<color=green>Round started:</color> {Statistics.CurrentRound.StartTimestamp}\n"
                   + $"<color=green>Map seed:</color> <color=red>{Maps.Seed}</color>\n\n"
                   + $"<color=green>TotalScpKills:</color> {Statistics.CurrentRound.TotalScpKills}\n"
                   + $"<color=green>ZombiesChanged:</color> {Statistics.CurrentRound.ZombiesChanged}\n"
                   + $"<color=green>ScpsAlive:</color> {scpsAlive}\n"
                   + $"<color=green>ChaosInsurgencyAlive:</color> {chaosInsurgencyAlive}\n"
                   + $"<color=green>ClassDAlive:</color> {classDAlive}\n"
                   + $"<color=green>MtfAndGuardsAlive:</color> {mtfAndGuardsAlive}\n"
                   + $"<color=green>ZombiesAlive:</color> {zombiesAlive}\n"
                   + $"<color=green>ClassDEscaped:</color> {Statistics.CurrentRound.ClassDEscaped}\n"
                   + $"<color=green>ScientistsEscaped:</color> {Statistics.CurrentRound.ScientistsEscaped}\n"
                   + $"<color=green>TotalKilledPlayers:</color> {Statistics.CurrentRound.TotalKilledPlayers}";

        return true;
    }
}