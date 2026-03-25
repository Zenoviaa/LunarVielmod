using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Special.DeadRomancesExcalibur;

public class Smite : ModProjectile
{
    private Vector2 _smitePosition;
    private ref float Timer => ref Projectile.ai[0];
    private int Target => (int)Projectile.ai[1];
    
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();

    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 90;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();

        NPC target = Main.npc[Target];
        if (target.active)
        {
            _smitePosition = target.Center;
        }
            
        Timer++;
        if(Timer % 6 == 0)
        {
            if (this.OwnedByLocalClient())
            {
                Vector2 spawnPos = _smitePosition + new Vector2(0, -Main.rand.Next(250, 500));
                spawnPos.X += Main.rand.NextFloat(-500, 500);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), spawnPos, Vector2.Zero, 
                    ModContent.ProjectileType<DeadRomanceHeavenlySmiteBlade>(), Projectile.damage, Projectile.knockBack, 
                    Projectile.owner, ai0: Target);
            }
        }
    }

}
