using Stellamod.Common.Shaders;

namespace Stellamod.Core.LunarLightingSystem
{
    public class SunLightShader : CrystalShader<SunLightShader>
    {
        public float ShadowAlpha
        {
            set
            {
                Effect.Parameters["shadowAlpha"].SetValue(value);
            }
        }

        public Vector2 StepSize
        {
            set
            {
                Effect.Parameters["stepSize"].SetValue(value);
            }
        }
    }
}
