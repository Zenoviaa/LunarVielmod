using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles;

public class MoonSnipe : ModProjectile
{
    private float _inScale;
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.timeLeft = 120;
        Projectile.penetrate = -1;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            var softSummon = new SoundStyle("Stellamod/Assets/Sounds/SoftSummon");
            softSummon.PitchVariance = 0.5f;
            SoundEngine.PlaySound(softSummon, Projectile.position);
        }
        if (Timer < 30)
        {
            if (Projectile.velocity.Length() > 0.2f)
                Projectile.velocity *= 0.2f;
        }
        else if (Timer == 31)
        {
            var softSummon = new SoundStyle("Stellamod/Assets/Sounds/StarFlower1");
            softSummon.PitchVariance = 0.5f;
            SoundEngine.PlaySound(softSummon, Projectile.position);
            Projectile.velocity *= 10;
        }
        else if (Projectile.velocity.Length() < 25)
        {
            Projectile.velocity *= 1.2f;
            if (Projectile.velocity.Length() > 25)
            {
                LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 0.5f);
            }
        }
        else
        {
            Projectile.extraUpdates = 2;
        }
        _inScale = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / 60f));
    }

    private void DrawTrails(GraphicsDevice gDevice)
    {
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserColor = Color.White;
        laserShader.LaserTexture = TrailRegistry.StarTrail;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, laserShader, Projectile.Size * 0.5f);

        BloomTrailShader b = BloomTrailShader.Instance;
        b.InnerColor = Color.Blue;
        b.OuterColor = Color.DarkBlue;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth2, b, Projectile.Size * 0.5f);
    }
    private float GetTrailWidth2(float ratio)
    {
        return MathHelper.SmoothStep(32, 0, ratio) * _inScale * 2;
    }
    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(32, 0, ratio) * _inScale;
    }
    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.Blue, ratio);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawTrails);
        float globalScale = 0.4f * _inScale;
        SpritebatchDrawer flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        flareDrawer.color = Color.Blue;
        flareDrawer.color.A = 0;
        flareDrawer.scale *= globalScale;
        flareDrawer.rotation = Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(flareDrawer);

        flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare2, Projectile.Center);
        flareDrawer.color = Color.LightSkyBlue;
        flareDrawer.color.A = 0;
        flareDrawer.scale *= globalScale * 0.8f;
        flareDrawer.rotation = -Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(flareDrawer);

        flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare3, Projectile.Center);
        flareDrawer.color = Color.White;
        flareDrawer.color.A = 0;
        flareDrawer.scale *= globalScale;
        flareDrawer.rotation = Main.GlobalTimeWrappedHourly * 4;
        Main.spriteBatch.Draw(flareDrawer);
        return false;
        //   return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
        fx.Scale *= 1.5f;
        float numDust = 5;
        for (float n = 0; n < numDust; n++)
        {
            Vector2 vel = -Projectile.velocity;
            vel = vel.RotatedByRandom(MathHelper.ToRadians(60));
            vel = vel.SafeNormalize(Vector2.Zero);
            vel *= Main.rand.NextFloat(6, 75);
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.Blue;
            var dp = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
            dp.fast = true;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
            dp.gravity = 0;
        }
        FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
    }
}
