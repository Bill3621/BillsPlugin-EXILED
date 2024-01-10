

using System;
using Exiled.Events.EventArgs.Map;
using PluginAPI.Core;
using UnityEngine;

namespace BillsPlugin.Handlers
{
    class Map
    {

        public void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {

            if (!BillsPlugin.Instance.Config.TeslaGateDisabledByGrenade)
            {
                return;
            }

            Vector3 position = ev.Position;

            foreach (var teslaGate in TeslaGateController.Singleton.TeslaGates)
            {
                if (!teslaGate.IsInIdleRange(position))
                {
                    continue;
                }

                Log.Info($"Map {teslaGate.transform.childCount}");
                teslaGate.InactiveTime = Math.Max(0, BillsPlugin.Instance.Config.TeslaGateDisabledTime);
                teslaGate.isIdling = false;
                teslaGate.ServerSideIdle(false);

                var obj = new GameObject();
                obj.transform.parent = teslaGate.transform;
                obj.transform.name = "InactiveGate";
            }
        }

    }
}