using Terraria.ModLoader;

namespace Stellamod.Core.Backgrounds;

public class CustomBGGlobalLightPlayer : ModPlayer
{
    public static float LightStrength;
    public override void ResetEffects()
    {
        base.ResetEffects();
        LightStrength = 0;
    }
    public override void PostUpdate()
    {
        base.PostUpdate();
        if (CustomBGManager.drawingCustomBG)
        {
            LightStrength = 0.005f;
        }

    }
}
