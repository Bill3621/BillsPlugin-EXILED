using Exiled.Events.EventArgs.Player;

namespace BillsPlugin.Handlers
{
    internal class Player
    {
        public void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (ev.Tesla.IsShocking) return;

            if (BillsPlugin.Instance.Config.TeslaGateBypass.Contains(ev.Player.RoleManager.CurrentRole.RoleTypeId
                    .ToString()))
            {
                ev.IsTriggerable = false;
                ev.IsInIdleRange = false;
            }
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            if (BillsPlugin.Instance.Config.DisableGodModeOnTeamChange) ev.Player.IsGodModeEnabled = false;
        }
    }
}