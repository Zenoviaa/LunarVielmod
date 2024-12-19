using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Trails;
using Terraria;

namespace Stellamod.Common.Shaders
{
    internal class FoggyShader : BaseShader
    {
        private static FoggyShader _instance;
        public static FoggyShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Asset<Texture2D> FogTexture { get; set; }

        public Vector2 SourceSize { get; set; }
        public float EdgePower { get; set; }
        public float ProgressPower { get; set; }
        public float Speed { get; set; }

        public override void SetDefaults()
        {
            base.SetDefaults();

            FogTexture = TrailRegistry.CloudsSmall;
            SourceSize = FogTexture.Size();
            EdgePower = 0.66f;
            ProgressPower = 2f;
            Speed = 0.15f;
        }

        public override void Apply()
        {
            Effect.Parameters["sourceSize"].SetValue(SourceSize);
            Effect.Parameters["cloudTexSize"].SetValue(FogTexture.Value.Size());
            Effect.Parameters["cloudTexture"].SetValue(FogTexture.Value);
            Effect.Parameters["edgePower"].SetValue(EdgePower);
            Effect.Parameters["progressPower"].SetValue(ProgressPower);
            Effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
        }
    }
}
