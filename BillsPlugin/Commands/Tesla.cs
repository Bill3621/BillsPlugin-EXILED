using System;
using CommandSystem;
using PlayerRoles;
using Utils;

namespace BillsPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class Tesla : ICommand, IUsageProvider
    {
        public string Command { get; } = "tesla";

        public string[] Aliases { get; } = { };

        public string Description { get; } =
            "A command that triggers the nearest tesla gate of a given player (if in room / idle range)";

        public string[] Usage { get; } = new string[] { "%player%" };

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

            foreach (var referenceHub in list)
            {
                if (!referenceHub.IsAlive()) continue;
                // Check for closest tesla
                foreach (var teslaGate in TeslaGateController.Singleton.TeslaGates)
                {
                    if (!teslaGate.IsInIdleRange(referenceHub)) continue;

                    teslaGate.RpcInstantBurst();
                }
            }

            response = "Triggered valid tesla gates.";
            return true;
        }
    }
}