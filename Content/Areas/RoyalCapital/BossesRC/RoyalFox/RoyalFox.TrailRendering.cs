using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;

public partial class RoyalFox
{
    private Color DashTrailColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(completionRatio)) * _dashTrailAlpha;
    }

    private float DashTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(80, 80, completionRatio);
    }

    private void RenderPixelatedDashTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader laserShader = ShaderContent.GetInstance<BasicLaserShader>();
        laserShader.LaserTexture = AssetManager.LaserTextures.SplittingTrail;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 16));
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, DashTrailColorFunction, DashTrailWidthFunction, laserShader, NPC.Size * 0.5f);
    }
}
