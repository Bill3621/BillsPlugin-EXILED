using UnityEngine;
using VoiceChat.Codec;
using VoiceChat.Codec.Enums;

namespace BillsPlugin.Core.Classes;

public class OpusComponent : MonoBehaviour
{
    public ReferenceHub Owner { get; set; }
    public ReferenceHub Target { get; set; }

    public OpusEncoder Encoder { get; } = new(OpusApplicationType.Voip);
    public OpusDecoder Decoder { get; } = new();

    public static OpusComponent Get(ReferenceHub hub, ReferenceHub target)
    {
        if (Plugin.Instance.TryGetOpusComponent(hub, target, out var player)) return player;

        player = hub.gameObject.AddComponent<OpusComponent>();
        player.Owner = hub;
        player.Target = target;

        Plugin.Instance.Encoders.Add(player);
        return player;
    }

    public void ChangeVolume(float volumeFactor, float[] indata)
    {
        for (var i = 0; i < indata.Length; i++) indata[i] *= volumeFactor;
    }
}