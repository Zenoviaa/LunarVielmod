using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core.Effects.Trails;
using Stellamod.Helpers;

namespace Stellamod.Content.Trailers
{
    public class DesertWindyTrail : BaseTrailing
    {
        public override void ApplyShader()
        {
            base.ApplyShader();

            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, Color.LightGray, 0.5f);
            shader.NoiseColor = Color.LightGray;
            shader.OutlineColor = Color.Transparent;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;
            Shader = shader;
        }
        public override float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(0, 12, completionRatio);
        }

        public override Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.LightGray, EasingFunction.QuadraticBump(completionRatio)) * 0.5f;
        }
    }
    public class DesertBlazingTrail : BaseTrailing
    {
        public override void ApplyShader()
        {
            base.ApplyShader();
            //We're going to use the flaming trail shader here, it's cool
            FlamingTrailShader flamingTrailShader = new FlamingTrailShader();
            flamingTrailShader.SetDefaults();
            Shader = flamingTrailShader;
        }

        public override float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(0, 24, completionRatio);
        }

        public override Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Tan, Color.Transparent, completionRatio);
        }
    }
}
