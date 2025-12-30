using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Terraria;

namespace Stellamod.Common.Shaders.Dyes
{
    public class DyeStaticShader : BaseShader
    {
        private static DyeStaticShader _instance;
        public static DyeStaticShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }
        public float Speed { get; set; }
        public bool IsDirty { get; set; }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Speed = 5;
        }

        public override void Apply()
        {
            Effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
            Effect.Parameters["dirty"].SetValue(IsDirty);
        }
    }
}
