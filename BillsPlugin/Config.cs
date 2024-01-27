using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using PlayerRoles;

namespace BillsPlugin
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Sets if the plugin should check for updates.\nDefault: true")]
        public bool CheckForUpdates { get; set; } = true;

        [Description("Sets how many balls will spawn when using the balls command.\nDefault: 3")]
        public int BallAmount { get; set; } = 3;

        [Description("Sets time in minutes between facility scan. Set to -1 to disable.\nDefault: 7.5")]
        public float FacilityScanTime { get; set; } = 7.5f;

        [Description("Sets the chance that a Facility Scan will fail in percent.\nDefault: 10")]
        public short FacilityScanFailChance { get; set; } = 10;

        [Description("Sets if the Facility Scan should fail after the Alpha Wahread blew up.\nDefault: false")]
        public bool FacilityScanFailAlphaWarhead { get; set; } = false;

        [Description("Sets if instead of playing an announcement, a failed Facility Scan should be completely ignored (no announcements at all).\nDefault: false")]
        public bool FacilityScanFailNoAnnouncements { get; set; } = false;

        [Description("Sets if tesla gates should be disabled when hit by grenades.\nDefault true")]
        public bool TeslaGateDisabledByGrenade { get; set; } = true;

        [Description(
            "Sets the time in seconds the tesla gate should get disabled. Requires tesla_gate_disabled_by_grenade set to true.\nDefault: 30")]
        public int TeslaGateDisabledTime { get; set; } = 30;

        [Description(
            "Sets the roles which can will not trigger the tesla gates.\nValid roles: NtfSpecialist, NtfSergeant, NtfCaptain, NtfPrivate, FacilityGuard, Scientist, ClassD, ChaosConscript, ChaosRifleman, ChaosMaraudder, ChaosRepressor, Scp173, Scp106, Scp049, Scp079, Scp096, Scp0492, Scp939, Scp3114, Flamingo, AlphaFlamingo, ZombieFlamingo, Tutorial.\nDefault: NtfSpecialist, NtfSergeant, NtfCaptain, NtfPrivate")]
        public List<RoleTypeId> TeslaGateBypass { get; set; } = new List<RoleTypeId>
        {
            RoleTypeId.NtfSpecialist,
            RoleTypeId.NtfSergeant,
            RoleTypeId.NtfCaptain,
            RoleTypeId.NtfPrivate
        };

        [Description("Sets if god mode should get disabled on team change.\nDefault: true")]
        public bool DisableGodModeOnTeamChange { get; set; } = true;

        [Description(
            "Sets which SCPs can use proximity chat.\nValid SCPs: Scp049, Scp0492, Scp096, Scp106, Scp173, Scp939.\nDefault: Scp049, Scp0492")]
        public List<RoleTypeId> ProximityChatAllowedRoles { get; set; } = new List<RoleTypeId>
        {
            RoleTypeId.Scp049,
            RoleTypeId.Scp0492
        };

        [Description("Sets the max distance of the proximity chat.\nDefault: 20")]
        public ushort ProximityChatDistance { get; set; } = 20;

        [Description(
            "Sets the message which gets displayed upon spawning as an SCP with proximity chat allowed.\nDefault: Proximity Chat can be toggled with Alt.")]
        public string ProximityChatBroadcastMessage { get; set; } =
            "<b>Proximity Chat can be toggled with Alt.";

        [Description(
            "Sets the message which gets displayed when enabling proximity chat.\nDefault: Proximity Chat on.")]
        public string ProximityChatEnabledMessage { get; set; } =
            "Proximity Chat on.";

        [Description(
            "Sets the message which gets displayed when disabling proximity chat.\nDefault: Proximity Chat off.")]
        public string ProximityChatDisabledMessage { get; set; } =
            "Proximity Chat off.";
    }
}