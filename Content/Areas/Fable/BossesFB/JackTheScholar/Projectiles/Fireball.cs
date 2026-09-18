using Stellamod.Common.Shaders;
using Stellamod.Content.Dusts;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Fable.BossesFB.JackTheScholar.Projectiles;

public class Fireball : ScarletProjectile
{
    private float _fireTime;
    private ref float Timer => ref Projectile.ai[0];
    private Vector2[] _oldSmokeCenterPos;
    public Vector2[] SmokeOldCenterPos
    {
        get
        {
            if (_oldSmokeCenterPos == null)
                _oldSmokeCenterPos = new Vector2[SmokeTrailCacheLength];
            return _oldSmokeCenterPos;
        }
        private set
        {
            _oldSmokeCenterPos = value;
        }
    }


    private Vector2 StartWhipPosition;
    private Vector2 TargetWhipPosition;
    private Vector2 InitialVelocity;

    public override string Texture => TextureRegistry.CandleFlame;

    public int SmokeTrailCacheLength;

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(StartWhipPosition);
        writer.WriteVector2(TargetWhipPosition);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        StartWhipPosition = reader.ReadVector2();
        TargetWhipPosition = reader.ReadVector2();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        TrailCacheLength = 15;
        SmokeTrailCacheLength = 25;
        Projectile.width = 11;
        Projectile.height = 11;
        Projectile.hostile = true;
        Projectile.light = 0.278f;
        Projectile.timeLeft = 300;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
    }

    public override void AI()
    {
        base.AI();
        if (SmokeTrailCacheLength > 0)
        {
            for (int i = SmokeTrailCacheLength - 1; i > 0; i--)
            {
                SmokeOldCenterPos[i] = SmokeOldCenterPos[i - 1];
            }
            SmokeOldCenterPos[0] = Projectile.Center;
        }
        if (Main.myPlayer == Projectile.owner && TargetWhipPosition == Vector2.Zero)
        {
            StartWhipPosition = Projectile.Center;
            var player = PlayerHelper.FindClosestPlayer(Projectile.Center, 1000);
            if (player != null)
                TargetWhipPosition = player.Center;
            Projectile.netUpdate = true;
        }
        Timer++;
        if (Timer == 1)
        {
            SoundStyle shot = AssetRegistry.Sounds.MagicWand.FireCharge;
            shot.PitchVariance = 0.3f;
            SoundEngine.PlaySound(shot, Projectile.position);
        }

        float lightningAuraProgress = Timer / 180f;
        float easedLightningAuraProgress = Easing.SpikeOutCirc(lightningAuraProgress);
        if (Timer == 1)
        {
            InitialVelocity = Projectile.velocity;
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
        }

        if (Timer % 12 == 0)
        {
            Vector2 vel = Vector2.Zero;
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, vel, Scale: 1);
            d.noGravity = true;
        }
        if (Timer % 6 == 0)
        {
            Vector2 vel = Vector2.Zero;
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.Torch, vel, Scale: 1);
            d.noGravity = true;
        }

        if (Timer < 30)
        {
            Projectile.velocity *= 0.5f;
        }

        if (Timer < 30 && Timer % 5 == 0)
        {
            FXUtil.GlowCircleBoom(Projectile.Center,
              innerColor: Color.Yellow,
              glowColor: Color.Orange,
              outerGlowColor: Color.Red, duration: 5, baseSize: 0.04f);
        }
        if (Timer == 30)
        {
            LegacyParticle.NewParticle<SkullParticle>(Projectile.Center, Vector2.Zero, Color.Red);
        }
        if (Timer == 70)
        {
            //Ping Sound
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Jack_FirePing");
            soundStyle.PitchVariance = 0.1f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);

            for (float i = 0; i < 2; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                //     rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(TargetWhipPosition,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: 0.1f,
                    duration: 15);
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
        if (Timer == 30)
        {
            //Ping Sound
            var part = FXUtil.GlowCircleBoom(TargetWhipPosition,
                              innerColor: Color.Yellow,
                              glowColor: Color.OrangeRed,
                              outerGlowColor: Color.Red, duration: 12, baseSize: 0.06f);
            part.Scale *= 0.5f;
        }
        if (Timer == 50)
        {
            //Ping Sound
            var part = FXUtil.GlowCircleBoom(TargetWhipPosition,
                              innerColor: Color.Yellow,
                              glowColor: Color.OrangeRed,
                              outerGlowColor: Color.Red, duration: 12, baseSize: 0.06f);
            part.Scale *= 0.5f;
        }


        if (Timer == 90)
        {

            FXUtil.GlowCircleBoom(Projectile.Center,
                              innerColor: Color.Yellow,
                              glowColor: Color.Orange,
                              outerGlowColor: Color.Red, duration: 12, baseSize: 0.04f);
            Projectile.velocity = (TargetWhipPosition - Projectile.Center).SafeNormalize(Vector2.Zero) * InitialVelocity.Length();
            SoundStyle shot = AssetRegistry.Sounds.MagicWand.FireChargeShot;
            shot.PitchVariance = 0.3f;
            SoundEngine.PlaySound(shot, Projectile.position);

            var part = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 0.15f);
            part.innerColor = Color.Yellow;
            part.outerColor = Color.Orange;
            part.fadeToColor = Color.Red;
            part.Scale *= 0.125f;
            part.Rotation = Projectile.velocity.ToRotation();

            var part2 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 0.3f);
            part2.innerColor = Color.Yellow;
            part2.outerColor = Color.Orange;
            part2.fadeToColor = Color.Red;
            part2.Scale *= 0.25f;
            part2.Rotation = Projectile.velocity.ToRotation();
        }

        if (Timer > 200)
        {
            _fireTime += MathHelper.Lerp(0.1f, 0.0f, (Timer - 200) / 40f);
        }
        else
        {
            _fireTime += 0.1f;
        }

        if (Timer > 90 && Timer % 4 == 0)
        {
            LegacyParticle.NewParticle<FlareParticle>(Projectile.Center + Main.rand.NextVector2Circular(16, 16), Vector2.Zero);
        }
        if (Timer > 90 && Timer < 100)
        {
            Projectile.velocity *= 1.1f;
        }
        else if (Timer > 100)
        {
            if (Projectile.velocity.Length() > InitialVelocity.Length())
            {
                Projectile.velocity *= 0.9f;
            }
        }
        if (Timer % 4 == 0)
        {
            var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                innerColor: Color.Yellow,
                glowColor: Color.Orange,
                outerGlowColor: Color.Red,
                baseSize: Main.rand.NextFloat(0.03f, 0.1f),
                duration: Main.rand.NextFloat(5, 25));
            particle.Velocity = -Projectile.velocity.RotatedByRandom(0.6f);
            particle.Scale *= 0.5f;
            particle.Rotation = particle.Velocity.ToRotation();
        }

        if (Timer > 200)
        {
            Projectile.velocity *= 0.96f;
        }
        if (Timer % 6 == 0)
        {
            for (float f = 0; f < 1; f++)
            {
                Vector2 pVelocity = -Projectile.velocity.RotatedByRandom(MathHelper.PiOver4);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = LegacyParticle.NewParticle<SparkParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                spark.innerColor = Color.Yellow;
                spark.outerColor = Color.Red;
            }
        }


        if (Timer >= 120 && Projectile.velocity.Length() <= 3)
        {
            Projectile.Kill();
        }
        if (Timer >= 120)
        {
            Projectile.tileCollide = true;
        }

        Projectile.rotation = Projectile.velocity.X * 0.05f;
        DrawHelper.AnimateTopToBottom(Projectile, 4);
    }

    public float WidthFunction(float completionRatio)
    {
        float w = MathHelper.SmoothStep(32, 72, EasingFunction.QuadraticBump(completionRatio));
        w = MathHelper.Lerp(w, 0f, EasingFunction.InOutSine((Timer - 200) / 40f));
        return w;
    }

    public Color ColorFunction(float completionRatio)
    {
        Color tipColor = Color.Lerp(Color.Goldenrod, Color.DarkRed, completionRatio);
        Color finalColor = Color.Lerp(Color.Red, tipColor, EasingFunction.QuadraticBump(MathF.Pow(completionRatio, 0.5f)));
        Color finalColor2 = Color.Lerp(Color.Transparent, finalColor, EasingFunction.QuadraticBump(completionRatio));
        finalColor2 = Color.Lerp(finalColor2, Color.DarkRed, (Timer - 200) / 40f);
        return finalColor2;
    }
    public float SmokeWidthFunction(float completionRatio)
    {
        float w = MathHelper.SmoothStep(0, 100, EasingFunction.QuadraticBump(completionRatio));
        return w;
    }

    public Color SmokeColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.Transparent, Color.White, EasingFunction.InOutSine(completionRatio));
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Timer < 90)
            return false;
        BlackFireSmokeShader blackSmokeShader = BlackFireSmokeShader.Instance;
        TrailDrawer.Draw(Main.spriteBatch, SmokeOldCenterPos, OldCenterRot, SmokeColorFunction, SmokeWidthFunction, blackSmokeShader, Vector2.Zero);

        BlackFireOldShader blackFireShader = BlackFireOldShader.Instance;
        blackFireShader.Time = _fireTime;
        TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, OldCenterRot, ColorFunction, WidthFunction, blackFireShader, Vector2.Zero);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        SoundStyle shot = AssetRegistry.Sounds.Magic.RadianceCast1;
        shot.PitchVariance = 0.3f;
        SoundEngine.PlaySound(shot, Projectile.position);
        SoundStyle shot2 = SoundID.DD2_BetsyFireballImpact;
        shot2.PitchVariance = 0.3f;
        SoundEngine.PlaySound(shot2, Projectile.position);
        var part = FXUtil.GlowCircleBoom(Projectile.Center,
                          innerColor: Color.Yellow,
                          glowColor: Color.Orange,
                          outerGlowColor: Color.Red, duration: 24, baseSize: 0.14f);
        part.Scale *= 1.225f;
        for (float f = 0; f < 32; f++)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Torch,
                (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
        }
        for (int i = 0; i < SmokeOldCenterPos.Length; i++)
        {
            Vector2 pos = SmokeOldCenterPos[i];
            if (i < 8)
                continue;
            if (Main.rand.NextBool(4))
            {
                Vector2 velocity = -Projectile.oldVelocity;
                Particle<ThickSmokeParticle>.Spawn(pos, velocity * 0.5f, Color.White);
            }
        }

        for (float i = 0; i < 15; i++)
        {
            float rot = rot = -Vector2.UnitY.ToRotation();
            rot += Main.rand.NextFloat(-0.5f, 0.5f);

            Vector2 offset = rot.ToRotationVector2() * Main.rand.NextFloat(32, 64);
            Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(2, 15);
            var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center + offset,
                innerColor: Color.Yellow,
                glowColor: Color.Orange,
                outerGlowColor: Color.Red,
                baseSize: Main.rand.NextFloat(0.03f, 0.1f),
                duration: Main.rand.NextFloat(5, 25));
            particle.Velocity = velocity;
            particle.Scale *= 0.35f;
            particle.Rotation = rot;
        }

        FXUtil.ShakeCamera(Projectile.position, 100, 4);
        Vector2 position = Projectile.Center;
        Vector2 lvelocity = -Projectile.oldVelocity.SafeNormalize(Vector2.Zero) * 8;
        for (float f = 0; f < 8; f++)
        {
            Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
            pVelocity *= Main.rand.NextFloat(0.5f, 2f);
            var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
            FXUtil.GlowFragmentParticle(position, pVelocity,
                innerColor: Color.Yellow,
                outerColor: Color.Orange,
                fadeToColor: Color.Red,
                distortOut: true);

            if (Main.rand.NextBool(4))
            {
                Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                 lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
            }
            if (Main.rand.NextBool(4))
            {
                Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                 lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
            }
        }
        for (float f = 0; f < 8; f++)
        {
            Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
            pVelocity *= Main.rand.NextFloat(0.5f, 1f);
            var spark = LegacyParticle.NewParticle<SparkParticle>(position + Main.rand.NextVector2Circular(64, 64), pVelocity);
        }

        var sear = LegacyParticle.NewParticle<SearParticle>(Projectile.Center, Vector2.Zero);

        for (float f = 0; f < 4; f++)
        {
            Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
            pVelocity *= Main.rand.NextFloat(0.5f, 1f);
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), pVelocity, newColor: Color.Black);
        }

    }

    public override void PostDraw(Color lightColor)
    {
        base.PostDraw(lightColor);

    }
}
