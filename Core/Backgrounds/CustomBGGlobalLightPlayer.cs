using Terraria;
using Terraria.ID;
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
        if (Main.netMode == NetmodeID.Server)
            return;

        if (CustomBGManager.drawingCustomBG)
        {
            LightStrength = 0.005f;
        }

    }
}
