using Stellamod.Common.Shaders;

namespace Stellamod.Core.LunarLightingSystem
{
    public class SSAOShader : CrystalShader<SSAOShader>
    {
        public Vector2 StepSize
        {
            set
            {
                Effect.Parameters["stepSize"].SetValue(value);
            }
        }

        public Vector2[] Offsets
        {
            set
            {
                Effect.Parameters["offsets"].SetValue(value);
            }
        }
    }
}
