using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Core.Effects
{
    internal class MistShader : Shader
    {
        public MistShader()
        {

        }

        public Color StartColor { get; set; }
        public Color EndColor { get; set; }
        public override void ApplyToEffect()
        {
            base.ApplyToEffect();
            Effect.Parameters["startColor"].SetValue(StartColor.ToVector4());
            Effect.Parameters["endColor"].SetValue(EndColor.ToVector4());
        }
    }
}
