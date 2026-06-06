using Stellamod.Core.Pixelation;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.TheFalling.PerfectSingularityBoss.Projectiles;

public class PerfectChain : ModProjectile,
    IDrawToRenderTarget
{
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
