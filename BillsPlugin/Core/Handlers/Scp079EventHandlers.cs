using Exiled.Events.EventArgs.Scp079;
using UnityEngine;

namespace BillsPlugin.Core.Handlers;

internal class Scp079EventHandlers
{
    public void OnInteractingTesla(InteractingTeslaEventArgs ev)
    {
        var obj = ev.Tesla.Transform.Find("InactiveGate");
        if (obj == null) return;

        if (ev.Tesla.InactiveTime > 0)
            ev.IsAllowed = false;
        else
            Object.Destroy(obj.gameObject);
    }
}