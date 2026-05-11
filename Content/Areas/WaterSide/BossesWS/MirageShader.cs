using Stellamod.Common.Shaders;

namespace Stellamod.Content.Areas.WaterSide.BossesWS;

public class MirageShader : CrystalShader<MirageShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public Texture2D NoiseTexture
    {
        set
        {
            Effect.Parameters["noiseTexture"].SetValue(value);
            //   Effect.Parameters["noiseSize"].SetValue(value.Size());
        }
    }

    public float Alpha
    {
        set
        {
            Effect.Parameters["alpha"].SetValue(value);
        }
    }
}
