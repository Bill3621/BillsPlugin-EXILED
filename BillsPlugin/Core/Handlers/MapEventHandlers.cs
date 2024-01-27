using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Map;
using UnityEngine;

namespace BillsPlugin.Core.Handlers;

internal class MapEventHandlers
{
    private readonly Config _config;

#nullable enable
    internal MapEventHandlers(Config? global)
    {
        _config = global ?? new Config();
    }

    public void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
    {
        if (!_config.TeslaGateDisabledByGrenade) return;
        if (ev.Projectile.ProjectileType != ProjectileType.FragGrenade &&
            ev.Projectile.ProjectileType != ProjectileType.Scp018) return;

        var position = ev.Position;

        var idleTeslaGates = TeslaGateController.Singleton.TeslaGates
            .Where(teslaGate => teslaGate.IsInIdleRange(position));

        foreach (var teslaGate in idleTeslaGates)
        {
            teslaGate.InactiveTime = Math.Max(0, _config.TeslaGateDisabledTime);
            teslaGate.isIdling = false;
            teslaGate.ServerSideIdle(false);
            _ = new GameObject
            {
                transform =
                {
                    parent = teslaGate.transform,
                    name = "InactiveGate"
                }
            };
        }
    }
}