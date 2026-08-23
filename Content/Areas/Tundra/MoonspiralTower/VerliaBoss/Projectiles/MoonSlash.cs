using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles;

public class MoonSlash : ModProjectile
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
        Projectile.width = 256;
        Projectile.height = 256;
        Projectile.hostile = true;
        Projectile.timeLeft = 24;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {


            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Hyuh"), Projectile.position);
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordSlice"), Projectile.position);
        }
        Projectile.spriteDirection = Projectile.velocity.X < 0 ? 1 : -1;
        if (++Projectile.frameCounter >= 2)
        {
            Projectile.frameCounter = 0;
            if (++Projectile.frame >= 7)
            {
                //Projectile.frame = 0;
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
        //  return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
