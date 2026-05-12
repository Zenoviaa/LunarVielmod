using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.WaterSide.KingJellyfishBoss;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.BossesWS;

public class SuperZap : ModProjectile
{
    private float _widthMultiplier;
    private float _zapTime;
    private float _flashTimer;
    private Vector2 _controlPoint1;
    private Vector2 _controlPoint2;
    private Vector2 _controlPoint3;
    private Vector2 _controlPoint4;
    private Asset<Texture2D> _gradientTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    private Vector2 EndPoint => Projectile.Center + Projectile.velocity;
    private Vector2 EndPoint2 => Projectile.Center - Projectile.velocity;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();

    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = false;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 120;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collisionPoint = 0f;
        Vector2 start = Projectile.Center;
        Vector2 end = EndPoint;
        if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), end, start, 12, ref collisionPoint))
            return true;
        return false;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        if(Timer >= 60 && Timer < 90)
        {
            Projectile.hostile = true;
        }
        else
        {
            Projectile.hostile = false;
        }
        Timer++;
        if (Timer == 60)
        {
            SoundStyle zapSound;
            int rand = Main.rand.Next(4);
            switch (rand)
            {
                default:
                case 0:
                    zapSound = AssetRegistry.Sounds.LeviathanEel.LeviZap1 with { PitchVariance = 0.3f };
                    break;
                case 1:
                    zapSound = AssetRegistry.Sounds.LeviathanEel.LeviZap2 with { PitchVariance = 0.3f };
                    break;
                case 2:
                    zapSound = AssetRegistry.Sounds.LeviathanEel.LeviZap3 with { PitchVariance = 0.3f };
                    break;
                case 3:
                    zapSound = AssetRegistry.Sounds.LeviathanEel.LeviZap4 with { PitchVariance = 0.3f };
                    break;
            }
            zapSound.MaxInstances = 3;
            zapSound.Volume = 0.6f;
            SoundEngine.PlaySound(zapSound, Projectile.position);


            FXUtil.ShakeCamera(Projectile.Center, 1024, 32);

            for (float f = 0; f < 10; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(9, 9);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.innerColor = Color.Lerp(Color.White, Color.SkyBlue, Main.rand.NextFloat(0f, 1f));
                spawnParams.outerColor = Color.DarkBlue;
                var d = DustParticle.Spawn(EndPoint, vel, spawnParams);
                d.dampening = 0.05f;
                d.gravity = 0;
                d.noTileCollide = true;
                d.Scale *= 0.5f;
            }
            for (float f = 0; f < 10; f++)
            {
                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(10f, 15f);
                vel = vel.RotatedByRandom(MathHelper.ToRadians(45));
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.innerColor = Color.Lerp(Color.White, Color.SkyBlue, Main.rand.NextFloat(0f, 1f));
                spawnParams.outerColor = Color.DarkBlue;
                var d = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
                d.dampening = 0.05f;
                d.gravity = 0;
                d.noTileCollide = true;
                d.Scale *= 0.5f;
            }
            _controlPoint1 = Projectile.Center + Main.rand.NextVector2CircularEdge(192, 192);
            _controlPoint2 = Projectile.Center + Main.rand.NextVector2CircularEdge(192, 192);
            _controlPoint3 = Projectile.Center + Main.rand.NextVector2Circular(192, 192);
            _controlPoint4 = Projectile.Center + Main.rand.NextVector2Circular(192, 192);
            _widthMultiplier = Main.rand.NextFloat(0.5f, 1f);
            var fx = FXUtil.GlowCircleBoom(EndPoint, Color.White, Main.DiscoColor, Color.Blue);
            fx.Scale *= 2;
        }

        float length = ProjectileHelper.PerformBeamHitscan(Projectile.Center, Projectile.velocity, Projectile.velocity.Length());
        Projectile.velocity = Projectile.velocity.Resize(length);
        if (Timer % 5 == 0 && Timer > 30)
        {
            var fx = FXUtil.GlowCircleBoom(EndPoint, Main.DiscoColor, Color.Lerp(Main.DiscoColor, Color.Black, 0.5f), Color.Black);
            fx.Scale *= 2;
        }

        if (Timer % 3 == 0)
        {


        }
        if (Timer % 10 == 0)
        {
            _zapTime = Main.rand.NextFloat(0, 100);

        }

        if (Timer % 40 == 0)
        {
            _flashTimer = 28;
        }
        _flashTimer--;
    }

    private Color GetTrailColor(float ratio)
    {
        float osc = MathF.Sin(Main.GlobalTimeWrappedHourly * 4 + ratio * 8) * 0.5f + 0.5f;
        return Color.Lerp(Color.White, Main.DiscoColor, osc);
    }
    private float GetTrailWidth(float ratio)
    {
        float ease = EasingFunction.InOutSine(_flashTimer / 30f);
        float w = 20 * _widthMultiplier;
        float outEasing = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        return MathHelper.SmoothStep(w * 0.85f, w, EasingFunction.QuadraticBump(ratio)) * outEasing;
    }
    private Color GetTrailColor2(float ratio)
    {
        float osc = MathF.Sin(Main.GlobalTimeWrappedHourly * 4 + ratio * 8) * 0.5f + 0.5f;
        return Color.Lerp(Color.White, Main.DiscoColor, osc);
    }
    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.6f;
    }
    private float GetTrailWidth3(float ratio)
    {
        return 32;
    }
    private void DrawPixelatedLightning(GraphicsDevice gDevice)
    {
        int numPoints = 32;
        List<Vector2> trailPoints = new List<Vector2>(numPoints);
        Vector2 up = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);

        float outEase = EasingFunction.QuadraticBump((float)Projectile.timeLeft / 60f);
        Vector2 cp1 = Vector2.Lerp(_controlPoint1, _controlPoint3, outEase);
        Vector2 cp2 = Vector2.Lerp(_controlPoint2, _controlPoint4, outEase);
        for (float f = 0; f < numPoints; f++)
        {
            float ratio = (float)f / (float)numPoints;
            Vector2 startPoint = Projectile.Center;
            Vector2 trailPoint = ExtraMath.CubicBezier(startPoint,
                cp1, cp2, EndPoint2, ratio);
            // Vector2 trailPoint = Vector2.Lerp(startPoint, endPoint, ratio);
            trailPoint += up * MathF.Sin(ratio * 16 + _zapTime) * 32;
            trailPoints.Add(trailPoint);
        }
        for (int i = 0; i < 4; i++)
            trailPoints.Add(trailPoints[trailPoints.Count - 1]);
        //trailPoints.Add(trailPoints[trailPoints.Count - 1]);

        Vector2[] lightningPoints = trailPoints.ToArray();
        ZapLightningShader lightingShader = ZapLightningShader.Instance;
        lightingShader.Amplitude = 0.8f;

        float time = Main.GlobalTimeWrappedHourly * 16;
        float levels = 4;
        time = MathF.Floor(time * levels) / levels;
        lightingShader.Time = time;
        Asset<Texture2D> laserTexture = Timer > 15 ? AssetManager.LaserTextures.TexturedLaser : AssetManager.LaserTextures.TexturedLaser2;
        lightingShader.LaserTexture = laserTexture;
        lightingShader.Noise = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise").Value;
        lightingShader.Gradient = _gradientTextureAsset.Value;
        lightingShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        lightingShader.Levels = 64;
        lightingShader.Tiling = new Vector2(2f);
        //   lightingShader.BloomColor= Main.DiscoColor;
        TrailDrawer.Draw(Main.spriteBatch, lightningPoints, GetTrailColor, GetTrailWidth, lightingShader, Projectile.Size * 0.5f);

        BloomTrailShader bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.White;
        bloom.OuterColor = Main.DiscoColor;
        TrailDrawer.Draw(Main.spriteBatch, lightningPoints, GetTrailColor2, GetTrailWidth2, bloom, Projectile.Size * 0.5f);
    }
    private void DrawPixelatedLightning(SpriteBatch sb, Vector2 sp)
    {
        if (Timer >= 60)
            return;

        SpritebatchDrawer lineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.Spotlight, Projectile.Center);
        Vector2 aimingPoint = EndPoint;
        float rot = (aimingPoint - Projectile.Center).ToRotation();
        lineDrawer.rotation = rot;
        lineDrawer.LeftCenterOrigin();
        float scaleX = (aimingPoint - Projectile.Center).Length() / lineDrawer.texture.Width;
        lineDrawer.scale.X *= scaleX;
        lineDrawer.scale.Y *= 0.2f;
        lineDrawer.color = Main.DiscoColor;
        lineDrawer.color *= ExtraMath.Osc(0.5f, 1f, speed: 12);
        lineDrawer.color.A = 0;
        sb.Draw(lineDrawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        _gradientTextureAsset ??= ModContent.Request<Texture2D>(ModContent.GetInstance<ZapShockwave>().Texture + "_Gradient");
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedLightning);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedLightning);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
