using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Stellamod.Common.Shaders
{
    internal class MagicMothlightMaskingArmorShaderData : ArmorShaderData
    {
        public MagicMothlightMaskingArmorShaderData(Ref<Effect> shader, string passName)
        : base(shader, passName)
        {
        }
        public MagicMothlightMaskingArmorShaderData(Asset<Effect> shader, string passName)
            : base(shader, passName)
        {
        }

        public override void Apply()
        {
            MagicMothlightMaskingShader shader = MagicMothlightMaskingShader.Instance;
            Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * shader.Speed);
            Shader.Parameters["maskingColor"].SetValue(shader.MaskingColor.ToVector3());
            Shader.Parameters["maskingTexture"].SetValue(shader.MaskingTexture.Value);
            Shader.Parameters["maskingTextureSize"].SetValue(shader.MaskingTexture.Value.Size());
            Shader.Parameters["distortion"].SetValue(shader.Distortion);
            Shader.Parameters["distortingNoiseTexture"].SetValue(shader.DistortingNoiseTexture.Value);
            base.Apply();
        }
    }

    internal class MagicMothlightMaskingShader : BaseShader
    {
        private static MagicMothlightMaskingShader _instance;
        public static MagicMothlightMaskingShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Asset<Texture2D> MaskingTexture { get; set; }
        public Color MaskingColor { get; set; }
        public float Speed { get; set; }

        public Asset<Texture2D> DistortingNoiseTexture { get; set; }
        public float Distortion { get; set; }
        public override void SetDefaults()
        {
            base.SetDefaults();
            MaskingTexture = TrailRegistry.NoiseTextureSpaceStars;
            MaskingColor = Color.White;
            Speed = 2;
            DistortingNoiseTexture = TrailRegistry.Clouds3;
            Distortion = 0f;
        }

        public override void Apply()
        {
            base.Apply();

            Effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
            Effect.Parameters["maskingColor"].SetValue(MaskingColor.ToVector3());
            Effect.Parameters["maskingTexture"].SetValue(MaskingTexture.Value);
            Effect.Parameters["maskingTextureSize"].SetValue(MaskingTexture.Value.Size());


            Effect.Parameters["distortion"].SetValue(Distortion);
            Effect.Parameters["distortingNoiseTexture"].SetValue(DistortingNoiseTexture.Value);
        }

    }
}
