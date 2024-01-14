using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace BillsPlugin
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Sets if the plugin should check for updates.")]
        public bool CheckForUpdates { get; set; } = true;

        [Description("Sets how many balls will spawn when using the balls command.")]
        public int BallAmount { get; set; } = 3;

        [Description("Sets if tesla gates should be disabled when hit by grenades.")]
        public bool TeslaGateDisabledByGrenade { get; set; } = true;

        [Description(
            "Sets the time in seconds the tesla gate should get disabled. Requires tesla_gate_disabled_by_grenade set to true.")]
        public int TeslaGateDisabledTime { get; set; } = 30;


        [Description(
            "Sets the roles which can will not trigger the tesla gates. Valid roles: NtfSpecialist, NtfSergeant, NtfCaptain, NtfPrivate, FacilityGuard, Scientist, ClassD, ChaosConscript, ChaosRifleman, ChaosMaraudder, ChaosRepressor, Scp173, Scp106, Scp049, Scp079, Scp096, Scp0492, Scp939, Scp3114, Flamingo, AlphaFlamingo, ZombieFlamingo, Tutorial")]
        public List<string> TeslaGateBypass { get; set; } = new List<string>
            { "NtfSpecialist", "NtfSergeant", "NtfCaptain", "NtfPrivate" };

        [Description("Sets if god mode should get disabled on team change.")]
        public bool DisableGodModeOnTeamChange { get; set; } = true;
    }
}