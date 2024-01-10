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
            if (BillsPlugin.Instance.Config.TeslagateBypass.Contains(ev.Player.RoleManager.CurrentRole.RoleTypeId
                    .ToString()))
            {
                ev.IsTriggerable = false;
                ev.IsInIdleRange = false;
            }
        }

        //public void OnLeft(LeftEventArgs ev)
        //{
        //string message = TutorialPlugin.Instance.Config.LeftMessage.Replace("{player}", ev.Player.Nickname);
        //Map.Broadcast(6, message);
        //}

        //public void OnJoined(JoinedEventArgs ev)
        //{
        //string message = TutorialPlugin.Instance.Config.JoinedMessage.Replace("{player}", ev.Player.Nickname);
        //Map.Broadcast(6, message);
        //}

        //public void OnInteractingDoor(InteractingDoorEventArgs ev)
        //{
        //    if(ev.IsAllowed == false)
        //    {
        //        ev.Player.Broadcast(6, TutorialPlugin.Instance.Config.BoobyTrapMessage);
        //        ev.Player.Kill(DamageType.Poison);
        //    }
        //}
    }
}
