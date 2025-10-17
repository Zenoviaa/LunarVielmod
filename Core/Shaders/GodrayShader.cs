using Microsoft.Xna.Framework;
using Terraria;

namespace Stellamod.Core.Shaders
{
    public class GodrayShader : BaseShader
    {
        private static GodrayShader _instance;
        public static GodrayShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Color GlowColor { get; set; }
        public override void SetDefaults()
        {
            base.SetDefaults();
            GlowColor = Color.White;
        }

        protected override void OnApply()
        {
            base.OnApply();
            Data.UseColor(GlowColor);
            Effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 1);
            Data.Apply();
        }
    }
}
