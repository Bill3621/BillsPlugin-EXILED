using System;
using CommandSystem;

namespace BillsPlugin.Core.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal class ForceScan : ICommand
{
    public string Command { get; } = "forcescan";

    public string[] Aliases { get; } = [];

    public string Description { get; } = "Forces the ScanFacility event to run.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.GameplayData, out response)) return false;

        Plugin.Instance.InvokeScanFacility();

        response = "Ran Event.";
        return true;
    }
}