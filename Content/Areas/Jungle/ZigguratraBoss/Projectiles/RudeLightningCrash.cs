using Stellamod.Common.Particles;
using Stellamod.Common.ShockCircleSystem;
using Stellamod.Visual.Particles;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss.Projectiles;
public class RudeLightningCrash : ModProjectile
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
        Projectile.height = Projectile.width = 256;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.light = 1f;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60;
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
        if (Timer % 12 == 0)
        {
       //     MakeLightningBolt(Projectile.Center + Main.rand.NextVector2Circular(100, 100));
        }
        if (Timer == 1)
        {
            MakeLightningBolt(Projectile.Center + Main.rand.NextVector2Circular(32, 32));
            for (var i = 0; i < 1; i++)
            {
                MakeBigLightningBolt(Projectile.Center + Main.rand.NextVector2Circular(64, 64));
            }

            ShockCircles.CreateQuickWhiteFlash(Projectile.Center);
            MakeCracks(Projectile.Center);
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.LightGoldenrodYellow, Color.Gold, Color.Black, 20, baseSize: 0.21f);
            fx.Scale *= 1.6f;
            for (var i = 0; i < 16; i++)
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

            for (var i = 0; i < 4; i++)
            {
                var pos = Projectile.Center;
                pos.X += Main.rand.NextFloat(-768, 768);
                var vel = -Vector2.UnitY;
                vel *= 6;
                vel *= Main.rand.NextFloat(1f, 2f);
                FXUtil.MakeSoilParticle(pos, vel);
            }

            for (var i = 0; i < Main.rand.Next(2, 5); i++)
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


            for (var i = 0; i < 4; i++)
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

            for (var i = 0; i < 6; i++)
            {
                MakeLightningSpark(Projectile.Center + Main.rand.NextVector2Circular(64, 64));
            }

            for (var i = 0; i < 6; i++)
            {
                var edge = Main.rand.NextFloat(64, 164);
                MakeLightningSpikeySpark(Projectile.Center + Main.rand.NextVector2CircularEdge(edge, edge));
            }
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
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
