using System;
using System.Linq;
using CommandSystem;
using PlayerRoles;
using Utils;

namespace BillsPlugin.Core.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal class Tesla : ICommand, IUsageProvider
{
    public string Command { get; } = "tesla";

    public string[] Aliases { get; } = [];

    public string Description { get; } =
        "A command that triggers the nearest tesla gate of a given player (if in room / idle range)";

    public string[] Usage { get; } = ["%player%"];

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.GameplayData, out response)) return false;
        if (arguments.Count == 0)
        {
            response = "To execute this command provide at least 1 argument!\nUsage: " + Command + " " +
                       this.DisplayCommandUsage();
            return false;
        }

        var list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out _);

        if (arguments.At(0).Equals("*")) list.AddRange(ReferenceHub.AllHubs);

        if (list.Count == 0)
        {
            response = "No player selected.";
            return false;
        }

        foreach (var referenceHub in list.Where(referenceHub => referenceHub.IsAlive()))
            foreach (var teslaGate in TeslaGateController.Singleton.TeslaGates.Where(teslaGate =>
                         teslaGate.IsInIdleRange(referenceHub)))
                teslaGate.RpcInstantBurst();

        response = "Triggered valid tesla gates.";
        return true;
    }
}