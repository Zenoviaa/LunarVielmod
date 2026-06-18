using Stellamod.Common.Shaders;

namespace Stellamod.Effects.Generic;

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