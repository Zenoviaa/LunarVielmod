using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Terraria;

namespace Stellamod.Core.Effects
{
    internal class FoggyShader : Shader
    {
        public FoggyShader()
        {
            FogTexture = AssetRegistry.Textures.Noise.CloudsSmall;
            SourceSize = FogTexture.Size();
            EdgePower = 0.66f;
            ProgressPower = 2f;
            Speed = 0.15f;
        }
        public Asset<Texture2D> FogTexture { get; set; }

        public Vector2 SourceSize { get; set; }
        public float EdgePower { get; set; }
        public float ProgressPower { get; set; }
        public float Speed { get; set; }

        public override void ApplyToEffect()
        {
            base.ApplyToEffect();
            Effect.Parameters["sourceSize"].SetValue(SourceSize);
            Effect.Parameters["cloudTexSize"].SetValue(FogTexture.Value.Size());
            Effect.Parameters["cloudTexture"].SetValue(FogTexture.Value);
            Effect.Parameters["edgePower"].SetValue(EdgePower);
            Effect.Parameters["progressPower"].SetValue(ProgressPower);
            Effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
        }
    }
}
