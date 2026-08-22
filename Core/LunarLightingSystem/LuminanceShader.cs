using Stellamod.Common.Shaders;

namespace Stellamod.Core.LunarLightingSystem
{
    public class LuminanceShader : CrystalShader<LuminanceShader>
    {
        private EffectParameter _thresholdParam;
        public float Threshold
        {
            set
            {
                _thresholdParam ??= Effect.Parameters["threshold"];
                _thresholdParam.SetValue(value);
            }
        }

    }
}
