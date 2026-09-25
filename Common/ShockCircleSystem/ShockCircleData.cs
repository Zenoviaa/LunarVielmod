using ReLogic.Content;
using System;

namespace Stellamod.Common.ShockCircleSystem;

public struct ShockCircleData
{
    public Asset<Texture2D> textureAsset;
    public Func<float, float> easing;
    public Func<float, Color> colorOverTime;
    public Vector2 position;
    public float startScale;
    public float endScale;
    public float time;

    public float GetRadius(float uneasedProgress)
    {
        return MathHelper.Lerp(startScale, endScale, easing(uneasedProgress));
    }

    public Color GetColor(float uneasedProgress)
    {
        return colorOverTime(easing(uneasedProgress));
    }
}
