using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.WaterSide.KingJellyfishBoss;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.BossesWS.Projectiles;

public class LightningCrawl : ModProjectile
{
    private Vector2 _hitPoint;
    private Asset<Texture2D> _gradientTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    private NPC Parent => Main.npc[(int)Projectile.ai[1]];
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
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 180;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collisionPoint = 0f;
        Vector2 start = Projectile.Center;
        Vector2 end = _hitPoint;
        if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), end, start, 12, ref collisionPoint))
            return true;
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            if(Main.netMode != NetmodeID.Server)
            {
                var screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                screenShaderSystem.TintScreen(Color.White, 0.2f, 15);
            }

        }


        ShakeModSystem.Shake = 2;
        Projectile.Center = Parent.Center + new Vector2(80, -70).RotatedBy(Parent.rotation);
        float maxBeamLength = 2000; 
        Vector2 fireVelocity = Parent.rotation.ToRotationVector2();
        float length = ProjectileHelper.PerformBeamHitscan(Projectile.Center, fireVelocity, maxBeamLength);
        Projectile.velocity = fireVelocity;
        _hitPoint = Projectile.Center + fireVelocity.Resize(length);

        if (Timer % 16 == 0 && this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetItemSource_FromThis(), Projectile.Center, fireVelocity * length, 
                ModContent.ProjectileType<BabyZap>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        if (Main.rand.NextBool(4))
        {
            Vector2 inverseVelocity = -Projectile.velocity;
            inverseVelocity = inverseVelocity.SafeNormalize(Vector2.Zero);
            inverseVelocity = inverseVelocity.RotatedByRandom(1f);
            inverseVelocity *= Main.rand.NextFloat(5f, 25f);
            var dp = DustParticle.Spawn(_hitPoint, inverseVelocity);
      
            dp.outerColor = Color.DarkBlue;
            dp.dampening = 0.05f;
            dp.noTileCollide = true;

            var smoke = FaintSmokeParticle.SpawnInAlphaLayer(_hitPoint, Vector2.Zero);
            smoke.fadeToColor = Color.Black * 0.2f;
            smoke.color = Color.SandyBrown * 0.2f;
        }

        if(Timer % 8 == 0)
        {
            var sp = SparkleParticle.Spawn(_hitPoint + Main.rand.NextVector2Circular(64, 64), Vector2.Zero);
            sp.gravity = 0;
            sp.flickering = true;
            sp.Scale *= 0.4f;
            sp.outerColor = Color.DarkBlue;
        }
    }

    private Color GetTrailColor(float ratio)
    {
        return Color.White;
    }

    private float GetTrailWidth(float ratio)
    {
        float w = 44 * MathHelper.Lerp(0.9f, 1f, ExtraMath.Osc(0f, 1f, speed: 16));
        float outEasing = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        return MathHelper.SmoothStep(w * 0.85f, w, EasingFunction.QuadraticBump(ratio)) * outEasing * MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(ratio));
    }

    private Color GetTrailColor2(float ratio)
    {
        return Color.DarkBlue;
    }

    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 2f;
    }

    private void DrawPixelatedLightning(GraphicsDevice gDevice)
    {
        List<Vector2> points = new List<Vector2>();
        float numPoints = 64;
        for (float f = 0; f < numPoints; f++)
        {
            Vector2 start = Projectile.Center;
            Vector2 end = _hitPoint;
            Vector2 easedPoint = Vector2.Lerp(start, end, f / numPoints);
            Vector2 up = (end - start).RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);
            easedPoint += up * 16 * MathF.Sin(f * 0.3f);
            points.Add(easedPoint);
        }
        Vector2[] lightningPoints = points.ToArray();
        ZapLightningShader lightingShader = ZapLightningShader.Instance;
        lightingShader.Amplitude = MathHelper.Lerp(0f, 0.8f, EasingFunction.InOutSine(Timer / 100f));

        float time = Main.GlobalTimeWrappedHourly * 16;
        float levels = 4;
        time = MathF.Floor(time * levels) / levels;
        lightingShader.Time = time;
        Asset<Texture2D> laserTexture = AssetManager.LaserTextures.TexturedLaser2;
        lightingShader.LaserTexture = laserTexture;
        lightingShader.Noise = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise").Value;
        lightingShader.Gradient = _gradientTextureAsset.Value;
        lightingShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        lightingShader.Levels = 64;
       
        lightingShader.Tiling = new Vector2(0.5f);
        TrailDrawer.Draw(Main.spriteBatch, lightningPoints, GetTrailColor, GetTrailWidth, lightingShader, Projectile.Size * 0.5f);

        lightingShader.BloomColor = Main.DiscoColor;
        lightingShader.LaserTexture = TrailRegistry.SpikyTrail2;
        lightingShader.Tiling = new Vector2(1);
        TrailDrawer.Draw(Main.spriteBatch, lightningPoints, GetTrailColor2, GetTrailWidth2, lightingShader, Projectile.Size * 0.5f);


    }

    private void DrawPixelatedImpact(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer flashDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, _hitPoint);
        flashDrawer.color = Color.Lerp(Color.SkyBlue, Color.White, ExtraMath.Osc(0f, 1f, speed: 32));
        flashDrawer.color.A = 0;
        flashDrawer.scale *= 0.66f * ExtraMath.Osc(0.97f, 1f, speed: 2);
        flashDrawer.rotation = Main.GlobalTimeWrappedHourly * 2;
        sb.Draw(flashDrawer);

        SpritebatchDrawer flashDrawer2 = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, _hitPoint);
        flashDrawer2.color = Color.Lerp(Color.LightSkyBlue, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 32));
        flashDrawer2.color.A = 0;
        flashDrawer2.scale *= 0.66f;
        sb.Draw(flashDrawer2);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        _gradientTextureAsset ??= ModContent.Request<Texture2D>(ModContent.GetInstance<ZapShockwave>().Texture + "_Gradient");
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedLightning);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedImpact);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
