using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles;

public class MoonShot : ModProjectile
{
    private float _inScale;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Style => ref Projectile.ai[1];
    private ref float AIStyle => ref Projectile.ai[2];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 128;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.ignoreWater = true;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 480;
        Projectile.tileCollide = false;
        Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        base.AI();
        //   ProjectileID.Sets.TrailCacheLength[Type] = 128;
        Timer++;
        if (Timer == 1)
        {
            float numDust = 12;
            for (float n = 0; n < numDust; n++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(8, 8);
                DustParticleSpawnParams spawnparams = DustParticleSpawnParams.Default;
                spawnparams.outerColor = Color.Blue;
                var d = DustParticle.Spawn(Projectile.Center, vel, spawnparams);
                d.dampening = 0.05f;
                d.gravity = 0;
                d.fast = true;
                d.noTileCollide = true;
            }
            var boom = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
            boom.Scale *= 2f;
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/FrostShot{Style}"), Projectile.position);
        }
        if (Timer % 18 == 0)
        {
            var sp = CrescentMoonParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
            sp.gravity = 0;
        }

        if (Timer % 14 == 0)
        {
            var sp = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
            sp.Scale *= Main.rand.NextFloat(0.125f, 0.25f);
            sp.behindLayer = true;
            sp.noShrink = true;
            sp.fadeToColor = Color.Black;
            sp.color = Color.Lerp(Color.Blue, Color.Black, 0.75f);

        }
        float targetScale = 1f * EasingFunction.InOutSine(Projectile.timeLeft / 30f);
        _inScale = MathHelper.Lerp(_inScale, targetScale, 0.1f);
        Player player = PlayerHelper.FindClosestPlayer(Projectile.Center, 1024);
        if (player != null)
        {
            Vector2 targetVelocity = (player.Center - Projectile.Center);
            targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
            targetVelocity *= MathHelper.Lerp(12f, 25f, EasingFunction.InExpo(Timer / 180f));

            float lerp = AIStyle == 1 ? 0.005f : 0.01f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, lerp);
        }
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
    }
}
