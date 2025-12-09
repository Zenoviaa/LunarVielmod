using Microsoft.Xna.Framework;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;

namespace Stellamod.Content.Trailers
{

    public class IyxFlamingTrail : BaseTrailing
    {
        public override void ApplyShader()
        {
            base.ApplyShader();

            BlackFireShader blackFireShader = new BlackFireShader();
            blackFireShader.SetDefaults();
            Shader = blackFireShader;
        }

        public override float WidthFunction(float completionRatio)
        {
            return EasingFunction.QuadraticBump(completionRatio) * 64;
        }

        public override Color ColorFunction(float completionRatio)
        {
            Color lerp1 = Color.Lerp(Color.OrangeRed, Color.RosyBrown, completionRatio);
            return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(completionRatio)) * 0.5f;
        }
    }
}
