using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Stellamod.Common.Shaders.MagicTrails
{
    internal class SimpleGradientTrailShader : BaseShader
    {
        private static SimpleGradientTrailShader _instance;
        public static SimpleGradientTrailShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }
        public Asset<Texture2D> SlashTexture { get; set; }
        public Asset<Texture2D> GradientTexture { get; set; }
        public Color PrimaryColor { get; set; }
        public Color SecondaryColor { get; set; }
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrimaryColor = Color.White;
            SecondaryColor = Color.White;
        }

        public override void Apply()
        {
            Effect.Parameters["transformMatrix"].SetValue(TrailDrawer.WorldViewPoint2);
            Effect.Parameters["primaryColor"].SetValue(PrimaryColor.ToVector4());
            Effect.Parameters["secondaryColor"].SetValue(SecondaryColor.ToVector4());
            Effect.Parameters["trailTexture"].SetValue(SlashTexture.Value);
            Effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);
        }
    }
}
