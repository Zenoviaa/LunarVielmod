using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Stellamod.Common.Shaders
{
    internal class MagicMothlightDistortionShaderArmorShaderData : ArmorShaderData
    {
        public MagicMothlightDistortionShaderArmorShaderData(Ref<Effect> shader, string passName)
        : base(shader, passName)
        {
        }
        public MagicMothlightDistortionShaderArmorShaderData(Asset<Effect> shader, string passName)
            : base(shader, passName)
        {
        }

        public override void Apply()
        {
            MagicMothlightDistortionShader shader = MagicMothlightDistortionShader.Instance;
            Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * shader.Speed);
            Shader.Parameters["distortion"].SetValue(shader.Distortion);
            Shader.Parameters["distortingNoiseTexture"].SetValue(shader.DistortingNoiseTexture.Value);
            base.Apply();
        }
    }

    internal class MagicMothlightDistortionShader : BaseShader
    {
        private static MagicMothlightDistortionShader _instance;
        public static MagicMothlightDistortionShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Asset<Texture2D> DistortingNoiseTexture { get; set; }
        public float Distortion { get; set; }
        public float Speed { get; set; }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Speed = 15;
            DistortingNoiseTexture = TrailRegistry.Clouds3;
            Distortion = 0.2f;
        }

        public override void Apply()
        {
            base.Apply();

            Effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * Speed);

            Effect.Parameters["distortion"].SetValue(Distortion);
            Effect.Parameters["distortingNoiseTexture"].SetValue(DistortingNoiseTexture.Value);
        }

    }
}
