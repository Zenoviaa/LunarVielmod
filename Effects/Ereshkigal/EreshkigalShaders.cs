using Stellamod.Common.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellamod.Effects.Ereshkigal;


public class StarSuckShader : CrystalShader<StarSuckShader>
{
    public Vector2[] Particles
    {
        set
        {
            Effect.Parameters["particles"].SetValue(value);
        }
    }
    public Color BloomColor
    {
        set
        {
            Effect.Parameters["bloomColor"].SetValue(value.ToVector4());
        }
    }

    public Color FarColor
    {
        set
        {
            Effect.Parameters["farColor"].SetValue(value.ToVector4());
        }
    }
    public Color CloseColor
    {
        set
        {
            Effect.Parameters["closeColor"].SetValue(value.ToVector4());
        }
    }

    public Vector2 CenterNormalizedCoord
    {
        set
        {
            Effect.Parameters["centerNormalCoord"].SetValue(value);
        }
    }
    public float ParticleRadius
    {
        set
        {
            Effect.Parameters["particleRadius"].SetValue(value);
        }
    }
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }
    public float Swirliness
    {
        set
        {
            Effect.Parameters["swirliness"].SetValue(value);
        }
    }
}