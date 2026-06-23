using Stellamod.Common.Shaders;
using Terraria;

namespace Stellamod.Effects.Ice;


public class FrostedShader : CrystalShader<FrostedShader>
{
    public Vector2 SpriteSize
    {
        set
        {
            Effect.Parameters["spriteSize"].SetValue(value);
        }
    }

    public Texture2D SnowTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            Effect.Parameters["frostedTexelSize"].SetValue(value.GetTexelSize());
        }
    }

}
public class BlizzardTrailShader : CrystalShader<BlizzardTrailShader>
{
    public Texture2D WindTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[0] = value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Effect.Parameters["windImageSize"].SetValue(value.Size());
        }
    }

    public Texture2D SnowTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            Effect.Parameters["snowTexelSize"].SetValue(value.GetTexelSize());
        }
    }

    public Matrix TransformMatrix
    {
        set
        {
            Effect.Parameters["transformMatrix"].SetValue(value);
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

    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }
}
