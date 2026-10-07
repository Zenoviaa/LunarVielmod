using Stellamod.Common.Shaders;
using Stellamod.Core;
using System;

namespace Stellamod.Content.Rendering;

public static class CommonTrails
{
    /// <summary>
    /// A trail used by weapons such as Singular Dive
    /// </summary>
    /// <param name="trailCache"></param>
    /// <param name="getTrailColor"></param>
    /// <param name="getTrailWidth"></param>
    /// <param name="colorPalette"></param>
    /// <param name="offset"></param>
    public static void DrawGlowTrail(
        Vector2[] trailCache, 
        Func<float, Color> getTrailColor, 
        Func<float, float> getTrailWidth, 
        TriColorPalette colorPalette, 
        Vector2? offset = null)
    {
        var pass = AssetReferences.Effects.Abyss.SingularTrail.CreatePrimitivesPass();
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.insideColor = colorPalette.primaryColor.ToVector3();
        pass.Parameters.bloomColor = colorPalette.secondaryColor.ToVector3();

        var hlsl = new HlslSampler();
        hlsl.Sampler = SamplerState.LinearWrap;
        hlsl.Texture = AssetReferences.Assets.LaserTextures.Beamlight.Asset.Value;
        pass.Parameters.laserSampler = hlsl;
        pass.Apply();
        TrailDrawer.Draw(trailCache, getTrailColor, getTrailWidth, pass.Shader, offset);
    }
}
