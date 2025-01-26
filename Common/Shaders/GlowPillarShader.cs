using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Stellamod.Common.Shaders
{
    internal class GlowPillarShader : BaseShader
    {
        private static GlowPillarShader _instance;
        public static GlowPillarShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public float Speed { get; set; }
        public float BasePower { get; set; }
        public float Size { get; set; }
        public float Pixelation { get; set; }
        public float OuterPower { get; set; }
        public float Height { get; set; }
        public Color InnerColor { get; set; }
        public Color GlowColor { get; set; }
        public Color OuterGlowColor { get; set; }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Speed = 5;
            BasePower = 3.0f;
            Size = 0.5f;

            InnerColor = Color.White;
            GlowColor = Color.Yellow;
            OuterGlowColor = Color.Red;
            Pixelation = 1f;
            OuterPower = 4;
            Height = 1f;
        }

        public override void Apply()
        {
            Effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
            Effect.Parameters["height"].SetValue(Height);
            Effect.Parameters["size"].SetValue(Size);
            Effect.Parameters["basePower"].SetValue(BasePower);
            Effect.Parameters["innerColor"].SetValue(InnerColor.ToVector3());
            Effect.Parameters["glowColor"].SetValue(GlowColor.ToVector3());
            Effect.Parameters["outerGlowColor"].SetValue(OuterGlowColor.ToVector3());
            Effect.Parameters["pixelation"].SetValue(Pixelation);
            Effect.Parameters["outerPower"].SetValue(OuterPower);
        }
    }
}
