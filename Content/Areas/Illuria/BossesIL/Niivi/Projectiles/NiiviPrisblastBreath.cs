using Stellamod.Common;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;

public class NiiviPrisblastBreath : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    NPC Parent => Main.npc[(int)Projectile.ai[1]];
    public override string Texture => ModType.NoTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.width = 2560;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.light = 0.6f;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return base.Colliding(projHitbox, targetHitbox);
    }
    public override void AI()
    {
        base.AI();

        if (!Parent.active)
            Projectile.Kill();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
