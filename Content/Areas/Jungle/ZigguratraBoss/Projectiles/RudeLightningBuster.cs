using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss.Projectiles;

public class RudeLightningBuster : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 256;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.light = 1f;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
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
