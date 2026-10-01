using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Common.ShockCircleSystem;
using Stellamod.Content.Areas.Collosseum.BossesCL.CommanderGintzia.Hands;
using Stellamod.Content.Dusts;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss.Projectiles;

public class AxeLightningLingering : ModProjectile
{
    Color LightGoldenColor => Color.Lerp(Color.Gold, Color.Black, 0.5f);
    Color DarkGoldenColor => Color.Lerp(Color.DarkGoldenrod, Color.Black, 0.5f);
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.height = 256;
        Projectile.width = 256;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.light = 1f;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 240;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeCrawlingLightning(Vector2 pos)
    {
        var vel = Main.rand.NextVector2Circular(4, 0.2f);
        var color = Color.Lerp(LightGoldenColor, DarkGoldenColor, Main.rand.NextFloat(0f, 1f));
        color = Color.Lerp(color, Color.White, 0.6f);
        Particles.LightningArcCrawl.Spawn(new()
        {
            position = pos,
            velocity = vel,
            scale = Main.rand.NextFloat(0.9f, 1f),
            color = color,
            timeLeft = 100
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeLightningSpikeySpark(Vector2 pos)
    {
        var vel = -Vector2.UnitY;
        vel *= 6;
        vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));

        var color = Color.Orange;
        Particles.LightningSpikeySpark.Spawn(new()
        {
            position = pos,
            velocity = vel,
            scale = Main.rand.NextFloat(0.4f, 0.8f),
            color = color,
            timeLeft = 100
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeCracks(Vector2 pos)
    {
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = pos, timeLeft = 200, color = Color.DarkOrange });
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = pos, scale = 2f, timeLeft = 120, color = Color.DarkOrange });
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = pos, scale = 3f, timeLeft = 45, color = Color.DarkOrange });
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = pos, scale = 5f, timeLeft = 25, color = Color.DarkOrange });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeBigLightningBolt(Vector2 pos)
    {
        var vel = -Vector2.UnitY;
        vel *= 6;
        vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));
        vel.Y -= 6;

        var color = Color.Lerp(LightGoldenColor, DarkGoldenColor, Main.rand.NextFloat(0f, 1f));
        color = Color.Lerp(color, Color.White, 0.6f);
        Particles.LightningBoltBig.Spawn(new()
        {
            position = pos - vel.SafeNormalize(Vector2.Zero) * 24,
            velocity = vel,
            color = color,
            timeLeft = Main.rand.NextFloat(45, 100)
        });

        Zigguratra.PlayLightningSound(pos);
        var darkColor = Color.Lerp(color, Color.DarkOrange, 0.25f);
        FXUtil.GlowCircleBoom(pos, Color.Gold, Color.DarkGoldenrod, Color.DarkOrange, 25, baseSize: 0.16f);
        for (var f = 0; f < 16; f++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos + Main.rand.NextVector2Circular(32, 32),
                velocity = Main.rand.NextVector2Circular(16, 16),
                innerColor = color.ToVector4(),
                outerColor = darkColor.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.6f, 1.2f)),
                timeLeft = 90
            });
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeLightningBolt(Vector2 pos)
    {
        var vel = -Vector2.UnitY;
        vel *= 6;
        vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));

        var color = Color.Lerp(LightGoldenColor, DarkGoldenColor, Main.rand.NextFloat(0f, 1f));
        color = Color.Lerp(color, Color.White, 0.6f);
        Particles.LightningBolt.Spawn(new()
        {
            position = pos - vel.SafeNormalize(Vector2.Zero) * 24,
            velocity = vel,
            color = color,
            timeLeft = 60
        });

        Zigguratra.PlayLightningSound(pos);
        var darkColor = Color.Lerp(color, Color.DarkOrange, 0.25f);
        FXUtil.GlowCircleBoom(pos, Color.Gold, Color.DarkGoldenrod, Color.DarkOrange, 25, baseSize: 0.16f);
        for (var f = 0; f < 16; f++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos + Main.rand.NextVector2Circular(32, 32),
                velocity = Main.rand.NextVector2Circular(16, 16),
                innerColor = color.ToVector4(),
                outerColor = darkColor.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.6f, 1.2f)),
                timeLeft = 90
            });
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeLightningSpark(Vector2 pos)
    {
        var vel = -Vector2.UnitY;
        vel *= 6;
        vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));
        vel *= Main.rand.NextFloat(0.5f, 3f);

        var color = Color.Lerp(LightGoldenColor, DarkGoldenColor, Main.rand.NextFloat(0f, 1f));
        color = Color.Lerp(color, Color.White, 0.9f);
        Particles.LightningSpark.Spawn(new()
        {
            position = pos,
            velocity = vel,
            color = color,
            scale = Main.rand.NextFloat(0.7f, 1.5f) * 0.3f,
            timeLeft = 60
        });
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            Particles.LightningCrackedGround.Spawn(new()
            {
                position = Projectile.Center + new Vector2(0, -64),
                velocity = Vector2.Zero,
                timeLeft = 240,
                color = Color.Gold
            });
        }
        if(Timer % 5 == 0)
        {
            var pos = Projectile.position;
            pos.X += Main.rand.Next(0, Projectile.width);
            pos.Y += Main.rand.Next(0, Projectile.height);
            MakeLightningSpikeySpark(pos);
        }
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        var contrast = MathHelper.Lerp(1.16f, 1f, EasingFunction.InSine(Timer / 60f));
        var brightness = MathHelper.Lerp(-0.04f, 0f, EasingFunction.InSine(Timer / 60f));
        FXUtil.ApplyContrastBrightness(contrast, brightness);
        return false;
        //return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class AxeLightningCrash : ModProjectile
{
    Color LightGoldenColor => Color.Lerp(Color.Gold, Color.Black, 0.5f);
    Color DarkGoldenColor => Color.Lerp(Color.DarkGoldenrod, Color.Black, 0.5f);
    private ref float Timer => ref Projectile.ai[0];
    ref float Style => ref Projectile.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.height = 64;
        Projectile.width = 1280;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.light = 1f;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeCrawlingLightning(Vector2 pos)
    {
        var vel = Main.rand.NextVector2Circular(4, 0.2f);
        var color = Color.Lerp(LightGoldenColor, DarkGoldenColor, Main.rand.NextFloat(0f, 1f));
        color = Color.Lerp(color, Color.White, 0.6f);
        Particles.LightningArcCrawl.Spawn(new()
        {
            position = pos,
            velocity = vel,
            scale = Main.rand.NextFloat(0.9f, 1f),
            color = color,
            timeLeft = 100
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeLightningSpikeySpark(Vector2 pos)
    {
        var vel = -Vector2.UnitY;
        vel *= 6;
        vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));

        var color = Color.Orange;
        Particles.LightningSpikeySpark.Spawn(new()
        {
            position = pos,
            velocity = vel,
            scale = Main.rand.NextFloat(0.4f, 0.8f),
            color = color,
            timeLeft = 100
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeCracks(Vector2 pos)
    {
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = pos, timeLeft = 200, color = Color.DarkOrange });
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = pos, scale = 2f, timeLeft = 120, color = Color.DarkOrange });
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = pos, scale = 3f, timeLeft = 45, color = Color.DarkOrange });
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = pos, scale = 5f, timeLeft = 25, color = Color.DarkOrange });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeBigLightningBolt(Vector2 pos)
    {
        var vel = -Vector2.UnitY;
        vel *= 6;
        vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));
        vel.Y -= 6;

        var color = Color.Lerp(LightGoldenColor, DarkGoldenColor, Main.rand.NextFloat(0f, 1f));
        color = Color.Lerp(color, Color.White, 0.6f);
        Particles.LightningBoltBig.Spawn(new()
        {
            position = pos - vel.SafeNormalize(Vector2.Zero) * 24,
            velocity = vel,
            color = color,
            timeLeft = Main.rand.NextFloat(45, 100)
        });

        Zigguratra.PlayLightningSound(pos);
        var darkColor = Color.Lerp(color, Color.DarkOrange, 0.25f);
        FXUtil.GlowCircleBoom(pos, Color.Gold, Color.DarkGoldenrod, Color.DarkOrange, 25, baseSize: 0.16f);
        for (var f = 0; f < 16; f++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos + Main.rand.NextVector2Circular(32, 32),
                velocity = Main.rand.NextVector2Circular(16, 16),
                innerColor = color.ToVector4(),
                outerColor = darkColor.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.6f, 1.2f)),
                timeLeft = 90
            });
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeLightningBolt(Vector2 pos)
    {
        var vel = -Vector2.UnitY;
        vel *= 6;
        vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));

        var color = Color.Lerp(LightGoldenColor, DarkGoldenColor, Main.rand.NextFloat(0f, 1f));
        color = Color.Lerp(color, Color.White, 0.6f);
        Particles.LightningBolt.Spawn(new()
        {
            position = pos - vel.SafeNormalize(Vector2.Zero) * 24,
            velocity = vel,
            color = color,
            timeLeft = 60
        });

        Zigguratra.PlayLightningSound(pos);
        var darkColor = Color.Lerp(color, Color.DarkOrange, 0.25f);
        FXUtil.GlowCircleBoom(pos, Color.Gold, Color.DarkGoldenrod, Color.DarkOrange, 25, baseSize: 0.16f);
        for (var f = 0; f < 16; f++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos + Main.rand.NextVector2Circular(32, 32),
                velocity = Main.rand.NextVector2Circular(16, 16),
                innerColor = color.ToVector4(),
                outerColor = darkColor.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.6f, 1.2f)),
                timeLeft = 90
            });
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeLightningSpark(Vector2 pos)
    {
        var vel = -Vector2.UnitY;
        vel *= 6;
        vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));
        vel *= Main.rand.NextFloat(0.5f, 3f);

        var color = Color.Lerp(LightGoldenColor, DarkGoldenColor, Main.rand.NextFloat(0f, 1f));
        color = Color.Lerp(color, Color.White, 0.9f);
        Particles.LightningSpark.Spawn(new()
        {
            position = pos,
            velocity = vel,
            color = color,
            scale = Main.rand.NextFloat(0.7f, 1.5f) * 0.3f,
            timeLeft = 60
        });
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Style == 1 && Timer == 1)
        {
            //Create daedus lightning bolts basically
            //lmao
            if (this.OwnedByLocalClient())
            {
                var lightningFirer = ProjFirer.From<AxeLightningStrike>(Projectile);
                lightningFirer.velocity = Vector2.UnitY;
                lightningFirer.position += new Vector2(0, -512);
                lightningFirer.New();
            }

            if (this.OwnedByLocalClient())
            {
                var firer = ProjFirer.From<AxeLightningLingering>(Projectile);
                firer.New();
            }
       
        }

        if (Timer % 6 == 0)
        {
            MakeLightningBolt(Projectile.Center + Main.rand.NextVector2Circular(100, 100));
        }
        if (Timer == 1)
        {
            for (var i = 0; i < 4; i++)
            {
                MakeBigLightningBolt(Projectile.Center + Main.rand.NextVector2Circular(64, 64));
            }

            ShockCircles.CreateQuickWhiteFlash(Projectile.Center);
            MakeCracks(Projectile.Center);
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.LightGoldenrodYellow, Color.Gold, Color.Black, 20, baseSize: 0.21f);
            fx.Scale *= 2.4f;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 24);
            ShakeScreenPosition.Shake = 4;

            for (var i = 0; i < 100; i++)
            {
                var pos = Projectile.Center;
                pos.X += Main.rand.NextFloat(-64, 64);
                pos.Y -= 64;
                pos.Y += Main.rand.NextFloat(-16, 16);
                var vel = pos - Projectile.Center;
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(6f, 45);
                if (i % 2 == 0)
                {
                    Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
                    {
                        innerColor = Color.LightGoldenrodYellow.ToVector4(),
                        outerColor = Color.DarkOrange.ToVector4(),
                        position = pos,
                        velocity = vel,
                        timeLeft = Main.rand.NextFloat(40, 120),
                        scale = new Vector2(Main.rand.NextFloat(0.5f, 1.5f)),

                    });
                }
                else
                {
                    Particles.BitDust.Spawn(BitDustFactory.Default with
                    {
                        innerColor = Color.LightGoldenrodYellow.ToVector4(),
                        outerColor = Color.DarkOrange.ToVector4(),
                        position = pos,
                        velocity = vel,
                        velocityPerTickMult = 0.9f,
                        timeLeft = Main.rand.NextFloat(40, 120),
                        scale = new Vector2(Main.rand.NextFloat(0.5f, 1.5f)),
                    });
                }
            }

            for (var i = 0; i < 24; i++)
            {
                var pos = Projectile.Center;
                pos.X += Main.rand.NextFloat(-768, 768);
                var vel = -Vector2.UnitY;
                vel *= 6;
                vel *= Main.rand.NextFloat(1f, 2f);
                FXUtil.MakeSoilParticle(pos, vel);
            }

            for (var i = 0; i < Main.rand.Next(5, 8); i++)
            {
                var pos = Projectile.Center;
                pos += Main.rand.NextVector2Circular(32, 32);
                var vel = Main.rand.NextVector2Circular(8, 8);
                Particles.LightningBolt.Spawn(new()
                {
                    position = pos,
                    velocity = vel,
                    timeLeft = Main.rand.Next(20, 60),
                    color = Color.Gold
                });
            }

            //Lightning crash vfx
            Particles.LightningImpact.Spawn(new()
            {
                position = Projectile.Center,
                timeLeft = 30,
                color = Color.LightGoldenrodYellow,
                velocity = -Vector2.UnitY
            });


            for (var i = 0; i < 24; i++)
            {
                var pos = Projectile.Center;
                pos.X += Main.rand.NextFloat(-768, 768);
                var sp = FaintSmokeParticle.Spawn(pos, Main.rand.NextVector2Circular(1, 1), Scale: Main.rand.NextFloat(0.6f, 1.2f));
                sp.color *= 0.5f;
                sp.fadeToColor = Color.Black;
                sp.behindLayer = true;
                sp.Scale *= 2;
            }

            for (var i = 0; i < 16; i++)
            {
                var pos = Projectile.Center + Main.rand.NextVector2Circular(80, 32);
                var sp = SmokeParticle.SpawnInAlphaLayer(pos, Main.rand.NextVector2Circular(1, 1), Scale: Main.rand.NextFloat(0.6f, 1.2f));
                sp.initialColor = Color.Lerp(Color.DarkGray, Color.Black, 0.6f);
                sp.fadeToColor = Color.Black;
                sp.behindLayer = true;
                sp.Scale *= 4;
            }

            for (var i = 0; i < 24; i++)
            {
                MakeLightningSpark(Projectile.Center + Main.rand.NextVector2Circular(64, 64));
            }

            for (var i = 0; i < 12; i++)
            {
                var edge = Main.rand.NextFloat(64, 164);
                MakeLightningSpikeySpark(Projectile.Center + Main.rand.NextVector2CircularEdge(edge, edge));
            }
            /*
            for (var i = 0; i < 4; i++)
            {
                var edge = 16;
                MakeCrawlingLightning(Projectile.Center + Main.rand.NextVector2Circular(edge, edge));
            }*/
        }

        if (Timer >= 33)
        {
            Projectile.hostile = false;
        }


    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        var contrast = MathHelper.Lerp(1.16f, 1f, EasingFunction.InSine(Timer / 60f));
        var brightness = MathHelper.Lerp(-0.04f, 0f, EasingFunction.InSine(Timer / 60f));
        FXUtil.ApplyContrastBrightness(contrast, brightness);
        return false;
        //return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}


public class AxeLightningStrike : ModProjectile
{
    private Vector2 _lightningHitPos;
    private bool _calculatedStrikePoints;
    public float BeamLength;
    public Vector2[] BeamPoints;
    public float[] BeamRot;
    private float _lightningPower;
    private float _lightningTime;
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 6;
        Projectile.height = 6;
        Projectile.penetrate = -1;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.timeLeft = 60;
        Projectile.tileCollide = false;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {

            _lightningPower = 10;
        }

        if (Timer == 5)
        {
            _lightningPower = 5;
        }

        if (Timer == 15)
        {
            _lightningPower = 30;
        }

        float targetBeamLength = ProjectileHelper.PerformBeamHitscan(Projectile.position, Projectile.velocity.SafeNormalize(Vector2.Zero), 2400);
        BeamLength = targetBeamLength;
        if (Timer == 2)
        {
            _lightningPower = 0.9f;
            _lightningTime = 0;
            //Sound Effect Goooo
            SoundStyle lightningSoundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_LightingZap");
            lightningSoundStyle.PitchVariance = 0.1f;
            SoundEngine.PlaySound(lightningSoundStyle, Projectile.position);


            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 16; i++)
            {
                Vector2 dustSpawnPoint = Projectile.Center + direction * BeamLength;
                Vector2 dustVelocity = Main.rand.NextVector2Circular(8, 8);
                Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, dustVelocity, Scale: 0.5f);
                d.noGravity = true;
            }


            _lightningHitPos = Projectile.position + new Vector2(0, BeamLength);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
            var part = FXUtil.GlowCircleBoom(_lightningHitPos,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Blue, duration: 12, baseSize: 0.14f);
            part.Scale *= 2;
            for (float f = 0; f < 32; f++)
            {
                Dust.NewDustPerfect(_lightningHitPos, DustID.Torch,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }


            for (float i = 0; i < 15; i++)
            {
                float rot = rot = -Vector2.UnitY.ToRotation();
                rot += Main.rand.NextFloat(-0.5f, 0.5f);

                Vector2 offset = rot.ToRotationVector2() * Main.rand.NextFloat(32, 64);
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(2, 15);
                var particle = FXUtil.GlowCircleDetailedBoom1(_lightningHitPos + offset,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Blue,
                    baseSize: Main.rand.NextFloat(0.03f, 0.1f),
                    duration: Main.rand.NextFloat(5, 25));
                particle.Velocity = velocity;
                particle.Scale *= 0.35f;
                particle.Rotation = rot;
            }


            Vector2 position = _lightningHitPos;
            Vector2 lvelocity = -Vector2.UnitY * 8;
            for (float f = 0; f < 8; f++)
            {
                Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: Color.White,
                    outerColor: Color.Yellow,
                    fadeToColor: Color.Purple,
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

            var sear = LegacyParticle.NewParticle<SearParticle>(_lightningHitPos, Vector2.Zero);

            for (int i = 0; i < BeamPoints.Length; i++)
            {
                if (Main.rand.NextBool(16))
                {
                    Vector2 pos = BeamPoints[i];
                    pos += Main.rand.NextVector2Circular(32, 32);
                    var zap = LegacyParticle.NewParticle<ZapParticle>(pos, Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(2, 4));

                }
            }
        }

        if (Timer > 35)
        {
            _lightningPower = MathHelper.Lerp(_lightningPower, 10, 0.1f);



        }

        if (Timer == 42)
        {
            _lightningPower = 1.5f;
        }
        if (Timer == 42)
        {
            var part = FXUtil.GlowCircleBoom(_lightningHitPos,
                              innerColor: Color.White,
                              glowColor: Color.Yellow,
                              outerGlowColor: Color.Blue, duration: 6, baseSize: 0.12f);
        }
        if (Timer == 52)
        {
            _lightningPower = 2.3f;
        }
        if (Timer == 52)
        {
            var part = FXUtil.GlowCircleBoom(_lightningHitPos,
                              innerColor: Color.White,
                              glowColor: Color.Yellow,
                              outerGlowColor: Color.Blue, duration: 6, baseSize: 0.07f);
        }


        if (Timer == 58)
        {
            SoundStyle zap = SoundID.DD2_LightningBugZap;
            zap.PitchVariance = 0.3f;
            SoundEngine.PlaySound(zap, Projectile.position);

            for (float f = 0; f < 2; f++)
            {
                Vector2 pVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = LegacyParticle.NewParticle<ZapParticle>(_lightningHitPos + Main.rand.NextVector2Circular(64, 64), pVelocity);
                spark.Scale *= 0.5f;
                spark.Rotation = Main.rand.NextFloat(0f, 3.14f);
            }
        }
        _lightningTime -= 0.1f;
        if (!_calculatedStrikePoints)
        {
            List<Vector2> beamPoints = new List<Vector2>();
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            float numPoints = 80;
            float randOffset = Main.rand.NextFloat(-1f, 1f);
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + direction * BeamLength;
            end.X += Main.rand.Next(-16, -16);
            for (float i = 0; i <= numPoints; i++)
            {


                float interp = i / numPoints;
                Vector2 point = Vector2.Lerp(start, end, interp);
                point.X += EasingFunction.QuadraticBump(interp) * 64 * randOffset;
                //if(i % 4 == 0)
                //point.X += Main.rand.Next(-16, 16);
                beamPoints.Add(point);
            }

            BeamPoints = beamPoints.ToArray();
            BeamRot = new float[BeamPoints.Length];

            _calculatedStrikePoints = true;
        }
    }

    public override bool? CanDamage()
    {
        return Timer < 30;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float _ = 0f;
        float width = Projectile.width * 0.8f;
        Vector2 start = Projectile.Center;

        Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
        Vector2 end = start + direction * BeamLength;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        LightningShader lightningShader = LightningShader.Instance;
        lightningShader.Time = _lightningTime;
        lightningShader.Power = _lightningPower;
        TrailDrawer.Draw(spriteBatch, BeamPoints, BeamRot, LightningColorFunction, LightningWidthFunction, lightningShader);
        if(Timer < 15)
           TrailDrawer.Draw(spriteBatch, BeamPoints, BeamRot, LightningColorFunction, LightningWidthFunction, lightningShader);

        return false;
    }

    private float LightningWidthFunction(float completionRatio)
    {
        return MathHelper.Lerp(180, 0, completionRatio);
    }

    private Color LightningColorFunction(float completionRatio)
    {
        Color lerpColor = Color.Lerp(Color.White, Color.DarkOrange, Timer / 30f);
        return Color.Lerp(Color.Transparent, lerpColor, EasingFunction.QuadraticBump(completionRatio)); ;
    }

}