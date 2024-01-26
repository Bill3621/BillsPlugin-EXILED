using System;
using CommandSystem;
using Server = BillsPlugin.Handlers.Server;

namespace BillsPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class ForceScan : ICommand
    {
        public string Command { get; } = "forcescan";

        public string[] Aliases { get; } = { "fc" };

        public string Description { get; } = "Forces the ScanFacility event to run.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GameplayData, out response)) return false;

            Server.ScanFacility.Invoke();

            response = "Ran Event.";
            return true;
        }
    }
}