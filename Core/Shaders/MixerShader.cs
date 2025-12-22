using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace Stellamod.Core.Shaders
{
    public class MixerShader : BaseShader
    {
        private EffectParameter _noiseTextureParam;
        private EffectParameter _mixTextureParam;
        private EffectParameter _timeParam;
        private EffectParameter _strengthParam;
        private static MixerShader _instance;
        public static MixerShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Asset<Texture2D> MixTexture
        {
            set
            {
                _mixTextureParam ??= Effect.Parameters["mixTexture"];
                _mixTextureParam.SetValue(value.Value);
            }
        }

        public Asset<Texture2D> NoiseTexture
        {
            set
            {
                _noiseTextureParam ??= Effect.Parameters["noiseTexture"];
                _noiseTextureParam.SetValue(value.Value);
            }
        }

        public float Time
        {
            set
            {
                _timeParam ??= Effect.Parameters["time"];
                _timeParam.SetValue(value);
            }
        }
        public float Strength
        {
            set
            {
                _strengthParam ??= Effect.Parameters["strength"];
                _strengthParam.SetValue(value);
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Time = Main.GlobalTimeWrappedHourly * 4;
        }
    }
}
