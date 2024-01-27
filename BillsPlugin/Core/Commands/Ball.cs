using System;
using System.Linq;
using CommandSystem;
using Footprinting;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using PlayerRoles;
using Respawning;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace BillsPlugin.Core.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal class Ball : ICommand, IUsageProvider
{
    public string Command { get; } = "balls";

    public string[] Aliases { get; } = [];

    public string Description { get; } = "A command that spawns balls";

    public string[] Usage { get; } = ["%player%"];

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response)) return false;
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
            for (var i = 0; i < Math.Max(1, Plugin.Instance.Config.BallAmount); i++)
                SpawnProjectile(ItemType.SCP018, referenceHub, SetupScp018);

        RespawnEffectsController.PlayCassieAnnouncement("XMAS_BOUNCYBALLS", false, false);

        response = "Spawned balls";
        return true;
    }

    private static void SetupScp018(ThrownProjectile projectile)
    {
        projectile.GetComponent<Rigidbody>().velocity = Random.onUnitSphere * 15f;
    }

    private static void SpawnProjectile(ItemType id, ReferenceHub hub, Action<ThrownProjectile> setupMethod)
    {
        if (InventoryItemLoader.TryGetItem<ThrowableItem>(id, out var result))
        {
            var thrownProjectile =
                Object.Instantiate(result.Projectile, hub.transform.position, Quaternion.identity);
            var pickupSyncInfo = new PickupSyncInfo(id, result.Weight, ItemSerialGenerator.GenerateNext())
            {
                Locked = true
            };
            var networkInfo = pickupSyncInfo;
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