using Terraria.Graphics;
using Terraria.ModLoader;

namespace Stellamod.Core.Camera;

public class CameraZoomSystem : ModSystem
{
    public static float ZoomMultiplier = 1f;
    public static float TargetZoomMultiplier = 1f;
    public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
    {
        base.ModifyTransformMatrix(ref Transform);
        Transform.Zoom *= ZoomMultiplier;
    }

    public override void PreUpdateEntities()
    {
        base.PreUpdateEntities();
        TargetZoomMultiplier = 1f;
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        ZoomMultiplier = MathHelper.Lerp(ZoomMultiplier, TargetZoomMultiplier, 0.05f);
    }
}
