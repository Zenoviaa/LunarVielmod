using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.SporedomBoss.Projectiles;

public class CorePellet : ModProjectile
{
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    private ref float Timer => ref Projectile.ai[1];
    private ref float Explode => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.timeLeft = 450;
        Projectile.light = 0.78f;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.velocity *= 0.96f;
        Projectile.rotation += Projectile.velocity.Length() * 0.05f;
        if(Explode >= 7)
        {
            Projectile.Kill();
        }
    }
    
    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
    
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            for(float f = 0; f < 14; f++)
            {
                var firer = ProjFirer.From<FallingPollen>(Projectile);
                firer.velocity.X = Main.rand.NextFloat(-16, 16);
                firer.velocity.Y -= 12;
                firer.New();
            }
        }
    }
}
