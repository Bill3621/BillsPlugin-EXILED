using System;
using Exiled.Events.EventArgs.Map;
using UnityEngine;

namespace BillsPlugin.Handlers
{
    internal class Map
    {
        public void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (!BillsPlugin.Instance.Config.TeslaGateDisabledByGrenade) return;

            var position = ev.Position;

            foreach (var teslaGate in TeslaGateController.Singleton.TeslaGates)
            {
                if (!teslaGate.IsInIdleRange(position)) continue;
                teslaGate.InactiveTime = Math.Max(0, BillsPlugin.Instance.Config.TeslaGateDisabledTime);
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
}