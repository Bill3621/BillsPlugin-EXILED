using System;
using CommandSystem;

namespace BillsPlugin.Core.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal class ServerStats : ICommand
{
    public string Command { get; } = "serverstats";

    public string[] Aliases { get; } = [];

    public string Description { get; } =
        "Returns stats of the server";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        response = "\n"
                   + $"<color=green>Server IP:</color> {Servers.ServerIpAddress}:{Servers.Port}\n"
                   + $"<color=green>Server Max Players:</color> {Servers.MaxPlayers} players\n"
                   + $"<color=green>Number of Players:</color> {Players.Count} player{(Players.Count < 2 ? "" : "s")}";
        return true;
    }
}