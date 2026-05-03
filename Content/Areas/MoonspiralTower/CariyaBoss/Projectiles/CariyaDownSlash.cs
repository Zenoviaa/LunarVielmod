using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MoonspiralTower.CariyaBoss.Projectiles;

public class CariyaDownSlash : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 7;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.timeLeft = 60;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
    }
    public override bool PreDraw(ref Color lightColor)
    {

        return false;
        //return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
