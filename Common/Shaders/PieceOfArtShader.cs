using ReLogic.Content;
using Terraria;

namespace Stellamod.Common.Shaders;

public class PieceOfArtShader : CrystalShader<PieceOfArtShader>
{
    private EffectParameter _levelsParam;

    public float Levels
    {
        set
        {
            _levelsParam ??= Effect.Parameters["levels"];
            _levelsParam.SetValue(value);
        }
    }
    public Asset<Texture2D> BlobTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[1] = value.Value;
        }
    }
    public Texture2D Blob
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[1] = value;
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();

    }
}
