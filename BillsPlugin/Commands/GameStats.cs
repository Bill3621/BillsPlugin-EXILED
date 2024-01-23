using System;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using PlayerRoles;
using PluginAPI.Core;

namespace BillsPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class GameStats : ICommand
    {
        public string Command { get; } = "gamestats";

        public string[] Aliases { get; } = { };

        public string Description { get; } =
            "Returns stats of the current game";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GameplayData, out response)) return false;

            byte CIA = 0;
            byte CDA = 0;
            byte MAGA = 0;
            byte SA = 0;
            byte ZA = 0;

            foreach (var player in Player.GetPlayers())
            {
                if (player.Team == Team.Scientists) continue;
                if (player.Team == Team.ClassD)
                {
                    CDA += 1;
                    continue;
                }

                if (player.Role == RoleTypeId.Scp0492)
                {
                    ZA += 1;
                    continue;
                }

                if (player.Team.GetSide() == Side.ChaosInsurgency)
                    CIA += 1;
                else if (player.Team.GetSide() == Side.Mtf)
                    MAGA += 1;
                else if (player.Team.GetSide() == Side.Scp) SA += 1;
            }

            response = "\n"
                       + $"There {(Player.Count < 2 ? "is" : "are")} {Player.Count} player{(Player.Count < 2 ? "" : "s")} in the server currently.\n"
                       + $"<color=green>Round started:</color> {Statistics.CurrentRound.StartTimestamp}\n"
                       + $"<color=green>Map seed:</color> <color=red>{Map.Seed}</color>\n\n"
                       + $"<color=green>TotalScpKills:</color> {Statistics.CurrentRound.TotalScpKills}\n"
                       + $"<color=green>ZombiesChanged:</color> {Statistics.CurrentRound.ZombiesChanged}\n"
                       + $"<color=green>ScpsAlive:</color> {SA}\n"
                       + $"<color=green>ChaosInsurgencyAlive:</color> {CIA}\n"
                       + $"<color=green>ClassDAlive:</color> {CDA}\n"
                       + $"<color=green>MtfAndGuardsAlive:</color> {MAGA}\n"
                       + $"<color=green>ZombiesAlive:</color> {ZA}\n"
                       + $"<color=green>ClassDEscaped:</color> {Statistics.CurrentRound.ClassDEscaped}\n"
                       + $"<color=green>ScientistsEscaped:</color> {Statistics.CurrentRound.ScientistsEscaped}\n"
                       + $"<color=green>TotalKilledPlayers:</color> {Statistics.CurrentRound.TotalKilledPlayers}";

            return true;
        }
    }
}