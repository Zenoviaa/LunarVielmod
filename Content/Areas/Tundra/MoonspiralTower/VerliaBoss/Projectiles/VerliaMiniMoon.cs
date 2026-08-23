using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles;

public class VerliaMiniMoon : ModProjectile
{

    private float _flashAlpha;
    private Vector2 _squishScale;
    private Vector2 _targetScale;
    private ref float Timer => ref Projectile.ai[0];
    private Asset<Texture2D> _scrollingMoonTextureAsset;
    private Asset<Texture2D> _shadowTextureAsset;
    private Asset<Texture2D> _outlineMoonTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;

    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 130;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.hostile = true;
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        if (Timer == 1)
        {
            SoundStyle spawnSound = new SoundStyle($"Stellamod/Assets/Sounds/SoftSummon");
            spawnSound.PitchVariance = 0.4f;
            SoundEngine.PlaySound(spawnSound, Projectile.position);
            _flashAlpha = 1f;
            _squishScale = new Vector2(0.9f, 1.2f);
        }
        _targetScale = Vector2.Lerp(_targetScale, Vector2.One, 0.1f);
        _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
        _flashAlpha = MathHelper.Lerp(_flashAlpha, 0f, 0.1f);

        if (Timer < 40)
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.Pi / 40f);
        }
        else if (Timer < 70)
        {
            Projectile.velocity *= 0.8f;
        }
        else
        {
            Player player = PlayerHelper.FindClosestPlayer(Projectile.Center, 1024);
            if (Timer == 72 && player != null)
            {
                SoundStyle spawnSound = new SoundStyle($"Stellamod/Assets/Sounds/StarFlower2");
                spawnSound.PitchVariance = 0.4f;
                spawnSound.Volume = 0.3f;
                SoundEngine.PlaySound(spawnSound, Projectile.position);
                Vector2 redirectVelocity = player.Center - Projectile.Center;
                redirectVelocity = redirectVelocity.SafeNormalize(Vector2.Zero);
                redirectVelocity *= 2;
                Projectile.velocity = redirectVelocity;
            }
            if (Projectile.velocity.Length() < 25)
            {
                if (Timer % 7 == 0)
                {
                    Vector2 pos = Projectile.Center;
                    pos += Main.rand.NextVector2Circular(48, 48);
                    Vector2 vel = -Projectile.velocity;
                    vel = vel.SafeNormalize(Vector2.Zero);
                    vel *= 15;
                    FXUtil.GlowStretch(pos, vel);
                }

                if (Timer % 7 == 0)
                {
                    var sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero);
                    sp.flickering = true;
                    sp.outerColor = Color.Blue;
                    sp.fast = true;
                    sp.behindLayer = true;
                    sp.gravity = 0;
                }

                Projectile.velocity *= 1.1f;
            }
        }
    }

    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.Transparent, ratio);
    }

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(64, 0, ratio);
    }
    private void DrawPixelatedTrails(GraphicsDevice gDevice)
    {
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserTexture = TrailRegistry.CorkscrewTrail;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.Lerp(Color.LightSkyBlue, Color.Black, 0.6f);
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, laserShader, Projectile.Size * 0.5f);
    }
    private void DrawPixelatedMoon(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        _scrollingMoonTextureAsset ??= ModContent.Request<Texture2D>(ModContent.GetInstance<VerliaBouncingMoon>().Texture + "_ScrollingMoon");
        moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.3f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.7f;
        glowDrawer.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(glowDrawer);


        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.2f;
        glowDrawer.color.A = 0;
        glowDrawer.scale.X *= 1.2f;
        glowDrawer.scale.Y *= 0.6f;
        glowDrawer.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(glowDrawer);


        ScrollingMoonShader scrollingMoonShader = ScrollingMoonShader.Instance;
        scrollingMoonShader.ScrollingTexture = _scrollingMoonTextureAsset.Value;
        scrollingMoonShader.MaskSize = TextureAssets.Projectile[Type].Value.Size();

        float time = Main.GlobalTimeWrappedHourly * 1f;
        time += Projectile.whoAmI * 0.5f;
        scrollingMoonShader.ScrollOffset = new Vector2(time, 0f);
        scrollingMoonShader.BendStrength = 1.8f;
        scrollingMoonShader.Tiling = new Vector2(0.13f, 0.45f);


        //Draw the moon itself
        sb.Restart(effect: scrollingMoonShader.Effect);
        moonSprite.rotation = MathHelper.ToRadians(-12);
        moonSprite.color = Color.Lerp(Color.White, Color.LightSkyBlue, ExtraMath.Osc(0f, 0.3f, speed: 8));
        moonSprite.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(moonSprite);
        sb.RestartDefaults();

        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SolarRing, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.6f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.5f;
        glowDrawer.scale *= _squishScale * _targetScale * 0.75f;
        Main.spriteBatch.Draw(glowDrawer);


        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SolarEye, Projectile.Center);
        glowDrawer.color = Color.Blue * 0.16f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 1f;
        glowDrawer.rotation = Main.GlobalTimeWrappedHourly;
        glowDrawer.scale *= _squishScale * _targetScale * 0.75f;
        Main.spriteBatch.Draw(glowDrawer);


        moonSprite.color = Color.Lerp(Color.Transparent, Color.White, _flashAlpha);
        Main.spriteBatch.Draw(moonSprite);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        _outlineMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedMoon);
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrails);

        _shadowTextureAsset ??= ModContent.Request<Texture2D>(ModContent.GetInstance<VerliaBouncingMoon>().Texture + "_Shadow");
        SpritebatchDrawer shadowDrawer = SpritebatchDrawer.FromTextureAsset(_shadowTextureAsset, Projectile.Center);
        shadowDrawer.color *= 0.45f;
        shadowDrawer.scale *= _squishScale * _targetScale * 0.5f;
        Main.spriteBatch.Draw(shadowDrawer);


        SpritebatchDrawer outlineDrawer = SpritebatchDrawer.FromTextureAsset(_outlineMoonTextureAsset, Projectile.Center);
        outlineDrawer.color = Color.Red;
        outlineDrawer.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(outlineDrawer);

        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (float f = 0; f < 5f; f++)
        {
            Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
            Vector2 spawnVelocity = Vector2.Zero;
            spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            var steamParticle = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }

        for (int i = 0; i < 4; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.DarkGray;
            spawnParams.scaleRange *= 0.5f;
            spawnParams.innerColor = Color.White;
            DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
        }

        for (int i = 0; i < 8; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.DarkGray;
            spawnParams.scaleRange *= 0.5f;
            spawnParams.innerColor = Color.White;
            DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
        }

        var fx = FXUtil.GlowCircleBoom(Projectile.Center,
            innerColor: Color.White,
            glowColor: Color.LightSkyBlue,
            outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.24f);
        fx.Scale *= 1f;
        FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
    }
}
