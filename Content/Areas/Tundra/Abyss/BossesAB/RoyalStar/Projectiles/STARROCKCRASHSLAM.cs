using Stellamod.Common.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;

public class STARROCKCRASHSLAM : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 34;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            for (float f = 0; f < 32; f++)
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = Projectile.Center,
                    velocity = Main.rand.NextVector2Circular(32, 32),
                    innerColor = Color.LightGoldenrodYellow.ToVector4(),
                    outerColor = Color.DarkGoldenrod.ToVector4(),
                });
            }

        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
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
