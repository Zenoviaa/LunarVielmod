using Stellamod.Core;
using Terraria;
using Terraria.ID;

namespace Stellamod.Common.ShockCircleSystem;

public static class ShockCircles
{
    public static void CreateQuickWhiteFlash(Vector2 position)
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        ShockCircleUpdater.Create(new()
        {
            position = position,
            time = 120,
            startScale = 0.1f,
            endScale = 1,
            colorOverTime = (float f) => Color.Lerp(Color.White, Color.SkyBlue, f) * MathHelper.Lerp(1f, 0f, f),
            easing = EasingFunction.OutExpo,
            textureAsset = AssetReferences.Assets.NoiseTextures.BeamTrail.Asset
        });
    }
}
