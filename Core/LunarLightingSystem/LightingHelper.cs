using Terraria;
using Terraria.Graphics.Light;

namespace Stellamod.Core.LunarLightingSystem;

public static class LightingHelper
{
    public static bool CanRenderPostProcessingEffects
    {
        get
        {
            return Lighting.UsingNewLighting;
        }
    }

    /// <summary>
    /// Interpolates between 0-1 near the end of a day/night cycle, to make the transition a bit sooner
    /// </summary>
    public static float DayLightEase
    {
        get
        {
            float easingTime = 2400;
            float dayLength = (float)Main.dayLength;
            if (!Main.dayTime)
            {
                dayLength = (float)Main.nightLength;
            }

            float inTime = (float)Main.time;
            float inEasing = EasingFunction.InOutSine(inTime / easingTime);
            float outTime = (float)Main.time;
            float outDown = outTime - (dayLength - easingTime);
            float outEasing = EasingFunction.InOutSine(outDown / easingTime);
            float a = inEasing * MathHelper.Lerp(1f, 0f, outEasing);
            return a;
        }
    }
}
