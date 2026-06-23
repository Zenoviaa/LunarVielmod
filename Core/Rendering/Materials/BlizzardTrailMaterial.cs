using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Effects.Ice;
using Terraria;

namespace Stellamod.Core.Rendering.Materials;

public class BlizzardTrailMaterial : Material<VertexPositionColorTexture, BlizzardTrailShader, BlizzardTrailMaterial>
{
    public override void SetShaderParameters(BlizzardTrailShader shader)
    {
        shader.WindTexture = TrailRegistry.WhispyTrail.Value;
        shader.SnowTexture = AssetManager.Noise.SnowStormNoise.Asset.Value;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.InsideColor = Color.White;
        shader.BloomColor = Color.Blue;
        shader.TransformMatrix = TrailDrawer.WorldViewPoint2;
    }
}