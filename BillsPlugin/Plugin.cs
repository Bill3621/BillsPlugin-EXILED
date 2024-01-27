using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BillsPlugin.Core.Handlers;
using Exiled.API.Interfaces;
using JetBrains.Annotations;
using PlayerRoles;

namespace BillsPlugin;

public class Plugin : Plugin<Config>
{
    public static Plugin Instance { get; } = new Plugin();

    [CanBeNull] internal static Config GlobalConfig;

    private PlayerEventHandlers _playerEvents;
    private ServerEventHandlers _serverEvents;
    private MapEventHandlers _mapEvents;
    private Scp079EventHandlers _scp079Events;

    public override string Name => "BillsPlugin";
    public override string Author => "Bill (& ALEXWARELLC)";
    public override Version Version => new(0, 1, 1, 1);

    public List<OpusComponent> Encoders = [];

    private Plugin()
    {
    }

    public override void OnEnabled()
    {
        Initialize();
    }

    public override void OnDisabled()
    {
        Disconnect();
    }

    private void Initialize()
    {
        _playerEvents = new PlayerEventHandlers(GlobalConfig);
        _serverEvents = new ServerEventHandlers(GlobalConfig);
        _mapEvents = new MapEventHandlers(GlobalConfig);
        _scp079Events = new Scp079EventHandlers();

        Connect();

        Updater.CheckForUpdate();
    }

    private void Connect()
    {
        Server.RoundStarted += _serverEvents.OnRoundStarted;
        Server.RestartingRound += _serverEvents.OnRestartingRound;

        Player.TriggeringTesla += _playerEvents.OnTriggeringTesla;
        Player.Spawned += _playerEvents.OnSpawned;
        Player.VoiceChatting += _playerEvents.OnVoiceChatting;
        Player.TogglingNoClip += _playerEvents.OnTogglingNoClip;

        Map.ExplodingGrenade += _mapEvents.OnExplodingGrenade;

        Scp079.InteractingTesla += _scp079Events.OnInteractingTesla;
    }

    public void Disconnect()
    {
        Server.RoundStarted -= _serverEvents.OnRoundStarted;
        Server.RestartingRound -= _serverEvents.OnRestartingRound;

        Player.TriggeringTesla -= _playerEvents.OnTriggeringTesla;
        Player.Spawned -= _playerEvents.OnSpawned;
        Player.VoiceChatting -= _playerEvents.OnVoiceChatting;
        Player.TogglingNoClip -= _playerEvents.OnTogglingNoClip;

        Map.ExplodingGrenade -= _mapEvents.OnExplodingGrenade;

        Scp079.InteractingTesla -= _scp079Events.OnInteractingTesla;

        _playerEvents = null;
        _serverEvents = null;
        _mapEvents = null;
        _scp079Events = null;
    }

    public bool TryGetOpusComponent(ReferenceHub owner, ReferenceHub target, out OpusComponent result)
    {
        result = Encoders.FirstOrDefault(
            opusComponent => opusComponent.Owner == owner && opusComponent.Target == target);
        return result != null;
    }

    public void InvokeScanFacility()
    {
        _serverEvents.ScanFacility.Invoke();
    }
}

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = false;

    [Description("Sets if the plugin should check for updates.\nDefault: true")]
    public bool CheckForUpdates { get; set; } = true;

    [Description("Sets how many balls will spawn when using the balls command.\nDefault: 3")]
    public int BallAmount { get; set; } = 3;

    [Description("Sets time in minutes between facility scan. Set to -1 to disable.\nDefault: 8")]
    public int FacilityScanTime { get; set; } = 8;

    [Description("Sets the chance that a Facility Scan will fail in percent.\nDefault: 10")]
    public short FacilityScanFailChance { get; set; } = 10;

    [Description("Sets if the Facility Scan should fail after the Alpha Wahread blew up.\nDefault: false")]
    public bool FacilityScanFailAlphaWarhead { get; set; } = false;

    [Description(
        "Sets if instead of playing an announcement, a failed Facility Scan should be completely ignored (no announcements at all).\nDefault: false")]
    public bool FacilityScanFailNoAnnouncements { get; set; } = false;

    [Description("Sets if tesla gates should be disabled when hit by grenades.\nDefault true")]
    public bool TeslaGateDisabledByGrenade { get; set; } = true;

    [Description(
        "Sets the time in seconds the tesla gate should get disabled. Requires tesla_gate_disabled_by_grenade set to true.\nDefault: 30")]
    public int TeslaGateDisabledTime { get; set; } = 30;

    [Description(
        "Sets the roles which can will not trigger the tesla gates.\nValid roles: NtfSpecialist, NtfSergeant, NtfCaptain, NtfPrivate, FacilityGuard, Scientist, ClassD, ChaosConscript, ChaosRifleman, ChaosMaraudder, ChaosRepressor, Scp173, Scp106, Scp049, Scp079, Scp096, Scp0492, Scp939, Scp3114, Flamingo, AlphaFlamingo, ZombieFlamingo, Tutorial.\nDefault: NtfSpecialist, NtfSergeant, NtfCaptain, NtfPrivate, FacilityGuard")]
    public List<RoleTypeId> TeslaGateBypass { get; set; } =
    [
        RoleTypeId.NtfSpecialist,
        RoleTypeId.NtfSergeant,
        RoleTypeId.NtfCaptain,
        RoleTypeId.NtfPrivate,
        RoleTypeId.FacilityGuard
    ];

    [Description("Sets if god mode should get disabled on team change.\nDefault: true")]
    public bool DisableGodModeOnSpawn { get; set; } = true;

    [Description(
        "Sets which SCPs can use proximity chat.\nValid SCPs: Scp049, Scp0492, Scp096, Scp106, Scp173, Scp939.\nDefault: Scp049, Scp0492")]
    public List<RoleTypeId> ProximityChatAllowedRoles { get; set; } =
    [
        RoleTypeId.Scp049,
        RoleTypeId.Scp0492
    ];

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