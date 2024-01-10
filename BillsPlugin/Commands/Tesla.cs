using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Footprinting;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using PlayerRoles;
using Respawning;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using Utils;

namespace BillsPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Tesla : ICommand, IUsageProvider
    {
        public string Command { get; } = "tesla";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "A command that triggers the nearest tesla gate of a given player (if in room / idle range)";

        public string[] Usage { get; } = new string[1] { "%player%" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if(!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
            {
                return false;
            }
            if (arguments.Count == 0)
            {
                response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
                return false;
            }

            var list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out _);

            if (arguments.At(0).Equals("*"))
            {
                list.AddRange(ReferenceHub.AllHubs);
            }

            if(list.Count == 0)
            {
                response = "No player selected.";
                return false;
            }

            foreach(var referenceHub in list)
            {
                if(!referenceHub.IsAlive())
                {
                    continue;
                }
                // Check for closest tesla
                foreach (var teslaGate in TeslaGateController.Singleton.TeslaGates)
                {
                    if (!teslaGate.IsInIdleRange(referenceHub))
                    {
                        continue;
                    }

                    teslaGate.RpcInstantBurst();
                }
            }

            response = "Triggered valid tesla gates.";
            return true;
        }

        private static void SetupScp018(ThrownProjectile projectile)
        {
            projectile.GetComponent<Rigidbody>().velocity = UnityEngine.Random.onUnitSphere * 15f;
        }

        private void SpawnProjectile(ItemType id, ReferenceHub hub, Action<ThrownProjectile> setupMethod)
        {
            if(InventoryItemLoader.TryGetItem<ThrowableItem>(id, out var result))
            {
                ThrownProjectile thrownProjectile = UnityEngine.Object.Instantiate(result.Projectile, hub.transform.position, Quaternion.identity);
                PickupSyncInfo pickupSyncInfo = new PickupSyncInfo(id, result.Weight, ItemSerialGenerator.GenerateNext());
                pickupSyncInfo.Locked = true;
                PickupSyncInfo networkInfo = pickupSyncInfo;
                thrownProjectile.NetworkInfo = networkInfo;
                thrownProjectile.PreviousOwner = new Footprint(hub);
                setupMethod(thrownProjectile);
                NetworkServer.Spawn(thrownProjectile.gameObject);
            } 
            else
            {
                Log.Info("Uh no for some reason");
            }
        }
    }
}
