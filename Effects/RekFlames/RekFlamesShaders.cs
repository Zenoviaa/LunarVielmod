using Stellamod.Common.Shaders;
using Terraria;

namespace Stellamod.Effects.RekFlames;

public class RekFlamethrowerBeamShader : CrystalShader<RekFlamethrowerBeamShader>
{
    public Texture2D DistortionNoise
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
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
            Effect.Parameters["strength"].SetValue(value);
        }
    }
}

public class RekFlamethrowerShader : CrystalShader<RekFlamethrowerShader>
{
    public Texture2D DistortionNoise
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
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
            Effect.Parameters["strength"].SetValue(value);
        }
    }
}

public class LavaSilShader : CrystalShader<LavaSilShader>
{
    public Texture2D MaskTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
        }
    }
}

public class RekFirebreathShader : CrystalShader<RekFirebreathShader>
{

    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public Color InnerColor
    {
        set
        {
            Effect.Parameters["innerColor"].SetValue(value.ToVector3());
        }
    }

    public Color BloomColor
    {
        set
        {
            Effect.Parameters["bloomColor"].SetValue(value.ToVector3());
        }
    }

    public Texture2D FlameTexture
    {
        set
        {
      
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
        }
    }

    public Texture2D MetaballTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[2] = value;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.LinearClamp;
        }
    }

}
public class RekFireballShader : CrystalShader<RekFireballShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public float Strength
    {
        set
        {
            Effect.Parameters["strength"].SetValue(value);
        }
    }

    public Color InnerColor
    {
        set
        {
            Effect.Parameters["innerColor"].SetValue(value.ToVector3());
        }
    }
    public Color BloomColor
    {
        set
        {
            Effect.Parameters["bloomColor"].SetValue(value.ToVector3());
        }
    }
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
        }
    }
    public Texture2D WNoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[2] = value;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
        }
    }
}


public class BigRekFireballShader : CrystalShader<BigRekFireballShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public float Strength
    {
        set
        {
            Effect.Parameters["strength"].SetValue(value);
        }
    }

    public Color InnerColor
    {
        set
        {
            Effect.Parameters["innerColor"].SetValue(value.ToVector3());
        }
    }
    public Color BloomColor
    {
        set
        {
            Effect.Parameters["bloomColor"].SetValue(value.ToVector3());
        }
    }
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
        }
    }
    public Texture2D MaskTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[2] = value;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
        }
    }
}
public class RekTorchShader : CrystalShader<RekTorchShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public float Strength
    {
        set
        {
            Effect.Parameters["strength"].SetValue(value);
        }
    }

    public Color InnerColor
    {
        set
        {
            Effect.Parameters["innerColor"].SetValue(value.ToVector3());
        }
    }
    public Color BloomColor
    {
        set
        {
            Effect.Parameters["bloomColor"].SetValue(value.ToVector3());
        }
    }
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
        }
    }
}
public class RekAuraShader : CrystalShader<RekAuraShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public float Strength
    {
        set
        {
            Effect.Parameters["strength"].SetValue(value);
        }
    }

    public Color InnerColor
    {
        set
        {
            Effect.Parameters["innerColor"].SetValue(value.ToVector3());
        }
    }
    public Color BloomColor
    {
        set
        {
            Effect.Parameters["bloomColor"].SetValue(value.ToVector3());
        }
    }
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
        }
    }
    public Texture2D DitherTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[2] = value;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
            Effect.Parameters["ditherTexelSize"].SetValue(value.GetTexelSize());
        }
    }
    public Vector2 SpriteSize
    {
        set
        {
            Effect.Parameters["spriteSize"].SetValue(value);
        }
    }

}
