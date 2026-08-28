using Stellamod.Assets;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime;

public class Kabloowie : ModProjectile
{
    private int _frame;
    private float _animationTimer;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Style => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 512;
        Projectile.friendly = false;
        Projectile.hostile = false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SoundStyle kabloowieSound = AssetRegistry.Sounds.SteamPunking.ReadyAttack;
            SoundEngine.PlaySound(kabloowieSound, Projectile.position);
        }
        _animationTimer++;
        if (_animationTimer >= 1)
        {
            _frame++;
            _animationTimer = 0;
        }

        if (_frame >= 126)
        {
            Projectile.Kill();
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Rectangle frame = TextureAssets.Projectile[Type].Value.GetFrame(_frame, 8, 16);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.sourceRect = frame;
        drawer.drawOrigin = frame.Size() * 0.5f;
        drawer.scale *= 1.5f;
        if (Style == 1)
            drawer.scale *= 0.5f;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
}
