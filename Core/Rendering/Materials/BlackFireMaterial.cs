using Stellamod.Common.Shaders;

namespace Stellamod.Core.Rendering.Materials;

public class BlackFireMaterial : Material<VertexPositionColorTexture, BlackFireShader, BlackFireMaterial>
{
    public override void SetShaderParameters(BlackFireShader shader)
    {
        //Just using the default black fire parameters here
        shader.SetDefaults();
    }
}
