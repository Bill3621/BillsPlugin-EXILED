using System;
using CommandSystem;
using PluginAPI.Core;

namespace BillsPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class ServerStats : ICommand
    {
        public string Command { get; } = "serverstats";

        public string[] Aliases { get; } = { };

        public string Description { get; } =
            "Returns stats of the server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "\n"
                       + $"<color=green>Server IP:</color> {Server.ServerIpAddress}:{Server.Port}\n"
                       + $"<color=green>Server Max Players:</color> {Server.MaxPlayers} players\n"
                       + $"<color=green>Number of Players:</color> {Player.Count} player{(Player.Count < 2 ? "" : "s")}";
            return true;
        }
    }
}