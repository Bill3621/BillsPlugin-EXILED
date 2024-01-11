using Exiled.Events.EventArgs.Scp079;

namespace BillsPlugin.Handlers
{
    internal class Scp079
    {
        public void OnInteractingTesla(InteractingTeslaEventArgs ev)
        {
            var obj = ev.Tesla.Transform.Find("InactiveGate");
            if (obj == null) return;

            if (ev.Tesla.InactiveTime > 0)
                ev.IsAllowed = false;
            else
                UnityEngine.Object.Destroy(obj.gameObject);
        }
    }
}