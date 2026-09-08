using Stellamod.Assets;
using Stellamod.Common.Shaders;
using System;
using Terraria;

namespace Stellamod.Core.Utilities;

public class HairRenderer
{
    public HairRenderer(Vector2 originPoint, int pointLength, int numPoints)
    {
        Chain = new Chain(originPoint, pointLength, numPoints);
        ghostAlpha = 1f;
        getHairColor = GetHairColor;
        getHairWidth = GetHairWidth;
        subdivisionCount = 5;
    }
    public readonly Chain Chain;
    public Func<float, Color> getHairColor;
    public Func<float, float> getHairWidth;
    public float ghostAlpha;
    public int subdivisionCount;
    public void SimulateHair(Vector2 originPoint)
    {
        Chain.points[0] = originPoint;
        Chain.points[0].Y -= 4 + ExtraMath.Osc(0f, 16, speed: 2);
        Chain.pinned[0] = true;

        for (int i = 0; i < 6; i++)
        {
            Chain.points[i].Y += ExtraMath.Osc(-8f, 8f, speed: 0.5f, offset: i);
        }
        for (int i = 0; i < Chain.points.Length; i++)
        {
            Chain.points[i].Y += MathHelper.Lerp(0.2f, 1f, i / (float)Chain.points.Length);
        }
        for (int i = 0; i < subdivisionCount; i++)
        {
            Chain.ResolveBackToRoot();
        }
    }

    public float GetHairWidth(float ratio)
    {
        return MathHelper.SmoothStep(80, 48, ratio) * EasingFunction.QuadraticBump(ratio);
    }

    public Color GetHairColor(float ratio)
    {
        Color spectralColor = Color.White * ghostAlpha * EasingFunction.OutExpo(ratio + 0.5f) * MathHelper.SmoothStep(1f, 0f, ratio) * EasingFunction.QuadraticBump(ratio) * 1.5f;
        Color scorllingColor = DrawUtilities.InterpolateColorArray(ratio, Color.White, Color.SkyBlue, Color.White, Color.LightSkyBlue);
        spectralColor = spectralColor.MultiplyRGBA(scorllingColor);
        return spectralColor;
    }

    public void Render(GraphicsDevice gDevice)
    {
        HairShader shader = ShaderContent.GetInstance<HairShader>();
        shader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
        shader.Time = Main.GlobalTimeWrappedHourly * 0.2f;
        shader.WaveFrequency = 8;
        shader.XOffset = 12;
        TrailDrawer.Draw(Main.spriteBatch, Chain.points, GetHairColor, GetHairWidth, shader);
    }
    public void Render(BaseShader shader)
    {
        TrailDrawer.Draw(Main.spriteBatch, Chain.points, GetHairColor, GetHairWidth, shader);
    }
}
