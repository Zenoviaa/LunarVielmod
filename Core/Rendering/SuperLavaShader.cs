using Stellamod.Common.Shaders;
using Terraria;

namespace Stellamod.Core.Rendering;

public class SuperLavaShader : CrystalShader<SuperLavaShader>
{
    public Texture2D RockTexture
    {
        set
        {
            Effect.Parameters["RockTexture"].SetValue(value);
        }
    }
    public Texture2D GlowMap
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[3] = value;
            Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.AnisotropicClamp;
        }
    }
    public Texture2D HeightMap
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[2] = value;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.AnisotropicClamp;
        }
    }
    public Texture2D WaterTexture
    {
        set
        {
            Effect.Parameters["WaterTexture"].SetValue(value);
        }
    }
    public float Time
    {
        set
        {
            Effect.Parameters["Time"].SetValue(value);
        }
    }
    public float Distortion
    {
        set
        {
            Effect.Parameters["Distortion"].SetValue(value);
        }
    }
    public Vector2 ScreenOffset
    {
        set
        {
            Effect.Parameters["ScreenOffset"].SetValue(value);
        }
    }
    public Vector2 Tiling
    {
        set
        {
            Effect.Parameters["Tiling"].SetValue(value);
        }
    }
    public Color InnerColor
    {
        set
        {
            Effect.Parameters["InnerColor"].SetValue(value.ToVector3());
        }
    }

    public Color BloomColor
    {
        set
        {
            Effect.Parameters["BloomColor"].SetValue(value.ToVector3());
        }
    }
    public Color StartGradient
    {
        set
        {
            Effect.Parameters["StartGradient"].SetValue(value.ToVector3());
        }
    }

    public Color EndGradient
    {
        set
        {
            Effect.Parameters["EndGradient"].SetValue(value.ToVector3());
        }
    }

    public float Quantize
    {
        set
        {
            Effect.Parameters["Quantize"].SetValue(value);
        }
    }

    public float NormalDistortionStrength
    {
        set
        {
            Effect.Parameters["NormalDistortionStrength"].SetValue(value);
        }
    }

    public Texture2D NormalNoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
        }
    }

    public Texture2D OutlineTexelSize
    {
        set
        {
            Effect.Parameters["outlineTexelSize"].SetValue(value.GetTexelSize() * 2);
        }
    }

    public Color OutlineColor
    {
        set
        {
            Effect.Parameters["outlineColor"].SetValue(value.ToVector4());
        }
    }
}
