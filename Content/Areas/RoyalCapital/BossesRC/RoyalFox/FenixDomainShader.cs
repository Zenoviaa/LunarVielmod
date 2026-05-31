using Stellamod.Common.Shaders;
using Terraria;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;

public class FenixDomainShader : CrystalShader<FenixDomainShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public Texture2D GradientMap
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
        }
    }
}
