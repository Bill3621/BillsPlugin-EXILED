using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Exiled.Events.EventArgs.Scp079;
using PluginAPI.Core;
using UnityEngine;

namespace BillsPlugin.Handlers
{
    class Scp079
    {
        public void OnInteractingTesla(InteractingTeslaEventArgs ev)
        {
            var obj = ev.Tesla.Transform.Find("InactiveGate");
            if (obj == null)
            {
                return;
            }

            if (ev.Tesla.InactiveTime > 0)
            {
                ev.IsAllowed = false;
            } 
            else
            {
                UnityEngine.Object.Destroy(obj.gameObject);
            }
        }
    }
}
