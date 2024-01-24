using UnityEngine;
using VoiceChat.Codec;
using VoiceChat.Codec.Enums;

namespace BillsPlugin
{
    public class OpusComponent : MonoBehaviour
    {
        /// <summary>
        /// The ReferenceHub instance that this player sends as.
        /// </summary>
        public ReferenceHub Owner { get; set; }

        public OpusEncoder Encoder { get; } = new OpusEncoder(OpusApplicationType.Voip);
        public OpusDecoder Decoder { get; } = new OpusDecoder();

        /// <summary>
        /// Add or retrieve the OpusComponent instance based on a ReferenceHub instance.
        /// </summary>
        /// <param name="hub">The ReferenceHub instance that this OpusComponent belongs to</param>
        /// <returns><see cref="OpusComponent"/></returns>
        public static OpusComponent Get(ReferenceHub hub)
        {
            if (BillsPlugin.Instance.Encoders.TryGetValue(hub, out var player)) return player;

            player = hub.gameObject.AddComponent<OpusComponent>();
            player.Owner = hub;

            BillsPlugin.Instance.Encoders.Add(hub, player);
            return player;
        }

        public void ChangeVolume(float volumeFactor, float[] indata)
        {
            for (var i = 0; i < indata.Length; i++) indata[i] *= volumeFactor;
        }
    }
}