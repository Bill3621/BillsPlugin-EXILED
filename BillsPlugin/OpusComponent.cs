using UnityEngine;
using VoiceChat.Codec;
using VoiceChat.Codec.Enums;

namespace BillsPlugin
{
    public class OpusComponent : MonoBehaviour
    {
        public ReferenceHub Owner { get; set; }
        public ReferenceHub Target { get; set; }

        public OpusEncoder Encoder { get; } = new OpusEncoder(OpusApplicationType.Voip);
        public OpusDecoder Decoder { get; } = new OpusDecoder();

        public static OpusComponent Get(ReferenceHub hub, ReferenceHub target)
        {
            if (BillsPlugin.Instance.TryGetOpusComponent(hub, target, out var player)) return player;

            player = hub.gameObject.AddComponent<OpusComponent>();
            player.Owner = hub;
            player.Target = target;

            BillsPlugin.Instance.Encoders.Add(player);
            return player;
        }

        public void ChangeVolume(float volumeFactor, float[] indata)
        {
            for (var i = 0; i < indata.Length; i++) indata[i] *= volumeFactor;
        }
    }
}