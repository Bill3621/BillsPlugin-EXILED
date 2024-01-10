using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillsPlugin
{
    public sealed class Config : IConfig
    {

        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        
        [Description("Sets how many balls will spawn when using the balls command.")]
        public int BallAmount { get; set; } = 1;

        [Description("Sets if tesla gates should be disabled when hit by grenades.")]
        public bool TeslaGateDisabledByGrenade { get; set; } = true;

        [Description("Sets the time in seconds the tesla gate should get disabled. Requires tesla_gate_disabled_by_grenade set to true.")]
        public int TeslaGateDisabledTime { get; set; } = 5;


        [Description("Sets the roles which can will not trigger the tesla gates. Valid roles: NtfSpecialist, NtfSergeant, NtfCaptain, NtfPrivate, FacilityGuard, Scientist, ClassD, ChaosConscript, ChaosRifleman, ChaosMaraudder, ChaosRepressor, Scp173, Scp106, Scp049, Scp079, Scp096, Scp0492, Scp939, Scp3114, Flamingo, AlphaFlamingo, ZombieFlamingo, Tutorial")]
        public List<string> TeslaGateBypass { get; set; } = new List<string> { "NtfSpecialist", "NtfSergeant", "NtfCaptain", "NtfPrivate"};

        //[Description("Sets the message for when someone joins the server. {player} will be replaced with the players name.")]
        //public string JoinedMessage { get; set; } = "{player} has joined the server.";

        //[Description("Sets the message for when someone leaves the server. {player} will be replaced with the players name.")]
        //public string LeftMessage { get; set; } = "{player} has left the server.";

        //[Description("Sets the message for when the round starts.")]
        //public string RoundStartedMessage { get; set; } = "uwu :3";


        //[Description("Sets the message for when someone triggers a trap.")]
        //public string BoobyTrapMessage { get; set; } = "You have activated my trap card!";
    }
}
