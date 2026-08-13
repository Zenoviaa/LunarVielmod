using Stellamod.Common.Shaders;

namespace Stellamod.Common.Particles;

public class BitDustShader : CrystalShader<BitDustShader>
{
    private EffectParameter _spriteTextureParam;
    private EffectParameter _projectionParam;
    public Matrix Projection
    {
        set
        {
            _projectionParam ??= Effect.Parameters["projection"];
            _projectionParam.SetValue(value);
        }
    }
    public Texture2D SpriteTexture
    {
        set
        {
            _spriteTextureParam ??= Effect.Parameters["spriteTexture"];
            _spriteTextureParam.SetValue(value);
        }
    }
}
public class InstancedParticleShader : CrystalShader<InstancedParticleShader>
{
    private EffectParameter _spriteTextureParam;
    private EffectParameter _projectionParam;
    public Matrix Projection
    {
        set
        {
            _projectionParam ??= Effect.Parameters["projection"];
            _projectionParam.SetValue(value);
        }
    }
    public Texture2D SpriteTexture
    {
        set
        {
            _spriteTextureParam ??= Effect.Parameters["spriteTexture"];
            _spriteTextureParam.SetValue(value);
        }
    }
}
