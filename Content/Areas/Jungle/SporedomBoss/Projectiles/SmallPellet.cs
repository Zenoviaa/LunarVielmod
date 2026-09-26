using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.SporedomBoss.Projectiles;

public class SmallPellet : ModProjectile
{
    private Vector2 FindTargetToMoveTo
    {
        get
        {
            var type = ModContent.ProjectileType<CorePellet>();
            foreach(var proj in Main.ActiveProjectiles)
            {
                if (proj.type != type)
                    continue;
                if (proj.ai[0] == Parent.whoAmI)
                {
                    return proj.Center;
                }
            }
            return Projectile.Center;
        }
    }
    private ref float InitialSpeed => ref Projectile.ai[2];
    private ref float Timer => ref Projectile.ai[1];
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 240;
    }

    public override void AI()
    {
        base.AI();
        if(Timer == 1)
        {
            InitialSpeed = Projectile.velocity.Length();
        }
        var targetToMoveTo = FindTargetToMoveTo;
        var targetDirection = targetToMoveTo - Projectile.Center;
        targetDirection = targetDirection.SafeNormalize(Vector2.Zero);
        var targetVelocity = targetDirection * InitialSpeed;
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.2f);
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation(), 0.03f);
        var distanceSquaredToTarget = Vector2.DistanceSquared(Projectile.Center, targetToMoveTo);
        var isCloseEnough = distanceSquaredToTarget < 24 * 24;
        if (isCloseEnough)
        {
            Eat();
        }
    }

    private void Eat()
    {
        var type = ModContent.ProjectileType<CorePellet>();
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.type != type)
                continue;
            if (proj.ai[0] == Parent.whoAmI)
            {
                proj.ai[2]++;
                proj.netUpdate = true;
                break;
            }
        }
        Projectile.Kill();
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
