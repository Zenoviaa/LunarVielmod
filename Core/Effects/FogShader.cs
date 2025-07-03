using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Terraria;

namespace Stellamod.Core.Effects
{
    internal class FogShader : Shader
    {
        public FogShader()
        {
            FogTexture = AssetRegistry.Textures.Noise.Clouds3;
            Speed = 0.5f;
            EdgePower = 1.5f;
            ProgressPower = 2.5f;
        }
        public Asset<Texture2D> FogTexture { get; set; }
        public float Speed { get; set; }
        public float EdgePower { get; set; }
        public float ProgressPower { get; set; }
        public Vector2 Offset { get; set; }
        public override void ApplyToEffect()
        {
            base.ApplyToEffect();
            Effect.Parameters["fogTexture"].SetValue(FogTexture.Value);
            Effect.Parameters["offset"].SetValue(Offset);
            Effect.Parameters["edgePower"].SetValue(EdgePower);
            Effect.Parameters["progressPower"].SetValue(ProgressPower);
            Effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
        }
    }
}
