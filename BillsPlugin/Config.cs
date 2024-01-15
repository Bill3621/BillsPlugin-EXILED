using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;

namespace BillsPlugin
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Sets if the plugin should check for updates. Default: true")]
        public bool CheckForUpdates { get; set; } = true;

        [Description("Sets how many balls will spawn when using the balls command. Default: 3")]
        public int BallAmount { get; set; } = 3;

        [Description("Sets if tesla gates should be disabled when hit by grenades. Default true")]
        public bool TeslaGateDisabledByGrenade { get; set; } = true;

        [Description(
            "Sets the time in seconds the tesla gate should get disabled. Requires tesla_gate_disabled_by_grenade set to true. Default: 30")]
        public int TeslaGateDisabledTime { get; set; } = 30;


        [Description(
            "Sets the roles which can will not trigger the tesla gates. Valid roles: NtfSpecialist, NtfSergeant, NtfCaptain, NtfPrivate, FacilityGuard, Scientist, ClassD, ChaosConscript, ChaosRifleman, ChaosMaraudder, ChaosRepressor, Scp173, Scp106, Scp049, Scp079, Scp096, Scp0492, Scp939, Scp3114, Flamingo, AlphaFlamingo, ZombieFlamingo, Tutorial. Default: NtfSpecialist, NtfSergeant, NtfCaptain, NtfPrivate")]
        public List<string> TeslaGateBypass { get; set; } = new List<string>
            { "NtfSpecialist", "NtfSergeant", "NtfCaptain", "NtfPrivate" };

        [Description("Sets if god mode should get disabled on team change. Default: true")]
        public bool DisableGodModeOnTeamChange { get; set; } = true;
    }
}