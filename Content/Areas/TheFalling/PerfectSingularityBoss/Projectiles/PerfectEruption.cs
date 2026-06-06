using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.TheFalling.PerfectSingularityBoss.Projectiles;

public class PerfectEruption : ModProjectile,
    IDrawToRenderTarget
{
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public void DrawToRenderTargets()
    {

    }
}
