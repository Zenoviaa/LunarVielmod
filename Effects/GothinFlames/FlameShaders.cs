using Stellamod.Common.Shaders;
using Terraria;

namespace Stellamod.Effects.GothinFlames;

public class FlameWingShader : CrystalShader<FlameWingShader>
{
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
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

    public float Distortion
    {
        set
        {
           Effect.Parameters["distortion"].SetValue(value);
        }
    }
}
public class FireVortexSmokeShader : CrystalShader<FireVortexSmokeShader>
{
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;
        }
    }
    public Color GradientTopColor
    {
        set
        {
            Effect.Parameters["gradientTopColor"].SetValue(value.ToVector4());
        }
    }

    public Color GradientBottomColor
    {
        set
        {
            Effect.Parameters["gradientBottomColor"].SetValue(value.ToVector4());
        }
    }
    public Vector2 Resolution
    {
        set
        {
            Effect.Parameters["resolution"].SetValue(value);
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
public class FireVortexShader : CrystalShader<FireVortexShader>
{
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;
        }
    }
    public Color GradientTopColor
    {
        set
        {
            Effect.Parameters["gradientTopColor"].SetValue(value.ToVector4());
        }
    }

    public Color GradientBottomColor
    {
        set
        {
            Effect.Parameters["gradientBottomColor"].SetValue(value.ToVector4());
        }
    }
    public Vector2 Resolution
    {
        set
        {
            Effect.Parameters["resolution"].SetValue(value);
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

public class WildfireShader : CrystalShader<FlameWindsShader>
{
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;
        }
    }
    public Color GradientTopColor
    {
        set
        {
            Effect.Parameters["gradientTopColor"].SetValue(value.ToVector4());
        }
    }

    public Color GradientBottomColor
    {
        set
        {
            Effect.Parameters["gradientBottomColor"].SetValue(value.ToVector4());
        }
    }
    public Vector2 Resolution
    {
        set
        {
           // Effect.Parameters["resolution"].SetValue(value);
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

public class FlameWindsShader : CrystalShader<FlameWindsShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }
}

public class FlameHurricaneShader : CrystalShader<FlameHurricaneShader>
{
    public float Radius
    {
        set
        {
            Effect.Parameters["radius"].SetValue(value);
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

}

public class FlameSwirlShader : CrystalShader<FlameSwirlShader>
{
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
    public float AngleCenter
    {
        set
        {
            Effect.Parameters["angleCenter"].SetValue(value);
        }
    }

    public float AngleRadius
    {
        set
        {
            Effect.Parameters["angleRadius"].SetValue(value);
        }
    }
}
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

    public float DistortionStrength
    {
        set
        {
            Effect.Parameters["distortionStrength"].SetValue(value);
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


