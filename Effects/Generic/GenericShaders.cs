using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Terraria;

namespace Stellamod.Effects.Generic;

public class DitheredColorPaletteShader : CrystalShader<DitheredColorPaletteShader>
{
    public Vector2 ScreenSize
    {
        set
        {
            Effect.Parameters["screenSize"].SetValue(value);
        }
    }
    public Texture3D ColorAtlasTexture
    {
        set
        {
            Effect.Parameters["ColorSpectrumTexture"].SetValue(value);
        }
    }
    public Texture2D DitheredTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            Effect.Parameters["ditherTexelSize"].SetValue(value.GetTexelSize());
        }
    }
    public float DitherAlpha
    {
        set
        {
            Effect.Parameters["ditherAlpha"].SetValue(value);
        }
    }

    public static DitheredColorPaletteShader PrepareForDrawing(Texture3D colorAtlas, Vector2 targetSize)
    {
        DitheredColorPaletteShader paletteShader = ShaderContent.GetInstance<DitheredColorPaletteShader>();
        paletteShader.DitherAlpha = 0.5f;
        paletteShader.ScreenSize = targetSize;
        paletteShader.ColorAtlasTexture = colorAtlas;
        paletteShader.DitheredTexture = AssetManager.Dithering.Dither8x8Double.Asset.Value;
        return paletteShader;
    }
}
public class MetaballShader : CrystalShader<MetaballShader>
{
    public Vector3[] Particles
    {
        set
        {
            Effect.Parameters["particles"].SetValue(value);
            Effect.Parameters["texelSize"].SetValue(Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight));
        }
    }
}
public class BishinineTentacleShader : CrystalShader<BishinineTentacleShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public float Frequency
    {
        set
        {
            Effect.Parameters["frequency"].SetValue(value);
        }
    }

    public float Amplitude
    {
        set
        {
            Effect.Parameters["amplitude"].SetValue(value);
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
public class NoisyBoomShader : CrystalShader<NoisyBoomShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }
    public Color NoiseColor
    {
        set
        {
            Effect.Parameters["bloomColor"].SetValue(value.ToVector3());
        }
    }
}
public class AuraShader : CrystalShader<AuraShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }
}

public class BasicGlowTrailShader : CrystalShader<BasicGlowTrailShader>
{
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
            Effect.Parameters["insideColor"].SetValue(value.ToVector4());
        }
    }
    public Color BloomColor
    {
        set
        {
            Effect.Parameters["bloomColor"].SetValue(value.ToVector4());
        }
    }
    public Color GlowColor
    {
        set
        {
            Effect.Parameters["glowColor"].SetValue(value.ToVector4());
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        TransformMatrix = TrailDrawer.WorldViewPoint2;

    }
}

public class GunHeatShader : CrystalShader<GunHeatShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public Color HottestColor
    {
        set
        {
            Effect.Parameters["hottestColor"].SetValue(value.ToVector3());
        }
    }

    public Color ColdestColor
    {
        set
        {
            Effect.Parameters["coldestColor"].SetValue(value.ToVector3());
        }
    }
}
public class GlowyTrailShader : CrystalShader<GlowyTrailShader>
{
    public Vector2[] Particles
    {
        set
        {
            Effect.Parameters["particles"].SetValue(value);
        }
    }

    public Color InsideColor
    {
        set
        {
            Effect.Parameters["insideColor"].SetValue(value.ToVector4());
        }
    }

    public Color BloomColor
    {
        set
        {
            Effect.Parameters["bloomColor"].SetValue(value.ToVector4());
        }
    }

    public float ParticleRadius
    {
        set
        {
            Effect.Parameters["particleRadius"].SetValue(value);
        }
    }
}