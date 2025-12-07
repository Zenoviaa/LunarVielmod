using Microsoft.Xna.Framework;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Shaders.MagicTrails;

namespace Stellamod.Content.Trailers
{
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
            return MathHelper.SmoothStep(0, 12, completionRatio);
        }

        public override Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Tan, Color.Transparent, completionRatio);
        }
    }
}
