using Stellamod.Assets;
using Stellamod.Common.Shaders;

namespace Stellamod.Core.Rendering.Materials;

public class RedBeamMaterial : Material<VertexPositionColorTexture, FixedRichLaserShader, RedBeamMaterial>
{
    public override void SetShaderParameters(FixedRichLaserShader shader)
    {
        shader.SetDefaults();
        shader.LaserColor = Color.White;
        shader.LaserTexture = TrailRegistry.StarTrail;
        shader.InnerColor = Color.Red * 0.5f;
        shader.OuterColor = Color.DarkRed;
        //    throw new System.NotImplementedException();
    }
}