using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Trails;
using Terraria;

namespace Stellamod.Common.Shaders.MagicTrails
{
    internal class MagicMoonShader : BaseShader
    {
        private static MagicMoonShader _instance;
        public static MagicMoonShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }
        public Asset<Texture2D> PrimaryTexture { get; set; }
        public Asset<Texture2D> NoiseTexture { get; set; }
        public float Speed { get; set; }
        public float Repeats { get; set; }
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrimaryTexture = TrailRegistry.StarTrail;
            NoiseTexture = TrailRegistry.StarTrail;
            Speed = 5;
            Repeats = 1;
        }

        public override void Apply()
        {
            Effect.Parameters["transformMatrix"].SetValue(TrailDrawer.WorldViewPoint2);
            Effect.Parameters["primaryTexture"].SetValue(PrimaryTexture.Value);
            Effect.Parameters["noiseTexture"].SetValue(NoiseTexture.Value);
            Effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
            Effect.Parameters["repeats"].SetValue(Repeats);
        }
    }
}
