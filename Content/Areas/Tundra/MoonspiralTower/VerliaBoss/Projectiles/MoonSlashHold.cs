using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles;

public class MoonSlashHold : ModProjectile
{
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    private ref float Timer => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 10;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = true;
        Projectile.width = 300;
        Projectile.height = 300;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 40;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Moaning"), Projectile.position);
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordHoldVerlia"), Projectile.position);
        }
        Projectile.Center = Parent.Center;
        if (++Projectile.frameCounter >= 1)
        {
            Projectile.frameCounter = 0;
            if (++Projectile.frame >= 10)
            {
                Projectile.frame = 0;
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.color.A = 0;
        Main.spriteBatch.Draw(sbDrawer);
        Main.spriteBatch.Draw(sbDrawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
