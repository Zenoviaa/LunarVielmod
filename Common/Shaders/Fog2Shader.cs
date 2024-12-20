using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Trails;
using Terraria;

namespace Stellamod.Common.Shaders
{
    internal class Fog2Shader : BaseShader
    {
        private static Fog2Shader _instance;
        public static Fog2Shader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Asset<Texture2D> FogTexture { get; set; }
        public float Speed { get; set; }
        public float EdgePower { get; set; }
        public float ProgressPower { get; set; }

        public override void SetDefaults()
        {
            base.SetDefaults();
            FogTexture = TrailRegistry.Clouds3;
            Speed = 0.5f;
            EdgePower = 1.5f;
            ProgressPower = 2.5f;
        }

        protected override void OnApply()
        {
            base.OnApply();
            Effect.Parameters["primaryTexture"].SetValue(FogTexture.Value);
            Effect.Parameters["edgePower"].SetValue(EdgePower);
            Effect.Parameters["progressPower"].SetValue(ProgressPower);
            Effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
        }
    }
}
