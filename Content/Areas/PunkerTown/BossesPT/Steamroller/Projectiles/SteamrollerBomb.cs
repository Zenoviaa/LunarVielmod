using ReLogic.Content;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Steamroller.Projectiles;

public class SteamrollerBomb : ModProjectile
{
    private Asset<Texture2D> _outlineTextureAsset;
    private Asset<Texture2D> _glowTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.timeLeft = 90;
        Projectile.light = 1.5f;

    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {

            FXUtil.GlowCircleBoom(Projectile.Center, Color.Yellow, Color.Orange, Color.Red);
            for (float f = 0; f < 4; f++)
            {
                Vector2 fireVelocity = Projectile.velocity;
                fireVelocity = fireVelocity.RotatedByRandom(MathHelper.ToRadians(65));
                fireVelocity *= Main.rand.NextFloat(0.4f, 0.7f);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), Scale: 0.5f, Velocity: fireVelocity, newColor: Color.Yellow);
            }
        }
        if (Timer % 12 == 0)
        {
            Vector2 dustVelocity = -Projectile.velocity;
            var dp = DustParticle.Spawn(Projectile.Center, dustVelocity);
            dp.innerColor = Color.Yellow;
            dp.outerColor = Color.DarkRed;
            dp.Scale *= 0.75f;
        }

        Projectile.velocity.Y += 0.5f;
        Projectile.rotation += Projectile.velocity.Length() * 0.02f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        _glowTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Glow");
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(sbDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(_glowTextureAsset, Projectile.Center);
        glowDrawer.color = Color.Lerp(Color.Black, Color.Red, ExtraMath.Osc(0f, 1f, speed: 12, offset: Projectile.whoAmI));
        glowDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowDrawer);

        SpritebatchDrawer outlienDrawer = SpritebatchDrawer.FromTextureAsset(_outlineTextureAsset, Projectile.Center);
        outlienDrawer.color = Color.Red;
        outlienDrawer.color.A = 0;
        Main.spriteBatch.Draw(outlienDrawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<SteamrollerBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
