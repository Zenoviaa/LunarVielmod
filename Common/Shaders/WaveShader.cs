using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellamod.Common.Shaders;

public class WaveShader : CrystalShader<WaveShader>
{
    private EffectParameter _frequencyParam;
    private EffectParameter _amplitudeParam;
    private EffectParameter _xStrengthParam;
    private EffectParameter _timeParam;
    private EffectParameter _noiseTextureParam;

    public Texture2D NoiseTexture
    {
        set
        {
            _noiseTextureParam = Effect.Parameters["noiseTexture"];
            _noiseTextureParam.SetValue(value);
        }
    }
    public float Time
    {
        set
        {
            _timeParam = Effect.Parameters["time"];
            _timeParam.SetValue(value);
        }
    }

    public float Frequency
    {
        set
        {
            _frequencyParam = Effect.Parameters["frequency"];
            _frequencyParam.SetValue(value);
        }
    }

    public float Amplitude
    {
        set
        {
            _amplitudeParam = Effect.Parameters["amplitude"];
            _amplitudeParam.SetValue(value);
        }
    }
    public float XStrength
    {
        set
        {
            _xStrengthParam = Effect.Parameters["xStrength"];
            _xStrengthParam.SetValue(value);
        }
    }
}
