using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginAPI.Core;

namespace BillsPlugin.Handlers
{
    class Player
    {

        public void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {

            if (ev.Tesla.IsShocking)
            {
                return;
            }

            if (BillsPlugin.Instance.Config.TeslaGateBypass.Contains(ev.Player.RoleManager.CurrentRole.RoleTypeId
                    .ToString()))
            {
                ev.IsTriggerable = false;
                ev.IsInIdleRange = false;
            }
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            if (BillsPlugin.Instance.Config.DisableGodModeOnTeamChange)
            {
                ev.Player.IsGodModeEnabled = false;
            }
        }
    }
}
