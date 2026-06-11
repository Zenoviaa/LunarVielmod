using Stellamod.Common.Shaders;
using Terraria;

namespace Stellamod.Effects.GothinFlames;

public class BlowTorchShader : CrystalShader<BlowTorchShader>
{
    public Texture2D FlameNoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
        }
    }
    public Color InsideColor
    {
        set
        {
            Effect.Parameters["flameStartColor"].SetValue(value.ToVector3());
        }
    }
    public Color BloomColor
    {
        set
        {
            Effect.Parameters["flameBloomColor"].SetValue(value.ToVector3());
        }
    }
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }
}

public class RedSunShader : CrystalShader<RedSunShader>
{
    public Texture2D FlameNoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
        }
    }
    public Color InsideColor
    {
        set
        {
            Effect.Parameters["flameInsideColor"].SetValue(value.ToVector3());
        }
    }
    public Color BloomColor
    {
        set
        {
            Effect.Parameters["flameBloomColor"].SetValue(value.ToVector3());
        }
    }
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

}
public class FlameBowShader : CrystalShader<FlameBowShader>
{
    public Texture2D FlameNoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
        }
    }
    public Color InsideColor
    {
        set
        {
            Effect.Parameters["flameInsideColor"].SetValue(value.ToVector3());
        }
    }
    public Color BloomColor
    {
        set
        {
            Effect.Parameters["flameBloomColor"].SetValue(value.ToVector3());
        }
    }
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public float DissipateThreshold
    {
        set
        {
            Effect.Parameters["dissipateThreshold"].SetValue(value);
        }
    }
}

public class GothinFlameTrailShader : CrystalShader<GothinFlameTrailShader>
{
    public Texture2D LaserTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[0] = value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        }
    }
    public Matrix TransformMatrix
    {
        set
        {
            Effect.Parameters["transformMatrix"].SetValue(value);
        }
    }
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }
    public Color InsideColor
    {
        set
        {
            Effect.Parameters["insideColor"].SetValue(value.ToVector3());
        }
    }
    public Color BloomColor
    {
        set
        {
            Effect.Parameters["bloomColor"].SetValue(value.ToVector3());
        }
    }
}

public class FlameyBoomShader : CrystalShader<FlameyBoomShader>
{
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
        }
    }
    public Color InsideColor
    {
        set
        {
            Effect.Parameters["flameInsideColor"].SetValue(value.ToVector3());
        }
    }
    public Color BloomColor
    {
        set
        {
            Effect.Parameters["flameBloomColor"].SetValue(value.ToVector3());
        }
    }
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }
    public float Threshold
    {
        set
        {
            Effect.Parameters["threshold"].SetValue(value);
        }
    }
}


