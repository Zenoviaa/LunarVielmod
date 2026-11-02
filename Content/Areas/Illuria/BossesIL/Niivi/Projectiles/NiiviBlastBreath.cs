using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;

public class NiiviFlyingStar : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => AssetReferences.Assets.Textures.Illuria.NPCs.NiiviFlyingStar.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(24);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.hostile = true;
        Projectile.light = 0.7F;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer < 60)
        {
            if (Projectile.velocity.Length() > 0.5f)
                Projectile.velocity *= 0.98f;
        } else if (Timer < 70)
        {
            var player = PlayerHelper.FindClosestPlayer(Projectile.Center, 2048);
            if(player != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, player.Center, 3);
            }

            if (Projectile.velocity.Length() < 15)
                Projectile.velocity *= 1.01f;
        }
        else
        {
            if(Projectile.velocity.Length() < 15)
                Projectile.velocity *= 1.01f;
        }

        MakeDusts();
        Projectile.rotation += MathF.Sign(Projectile.velocity.X) * 0.04f;
    }

    void DeathFX()
    {
        var starSingle = AssetReferences.Assets.Sounds.Starrer.Asset with { PitchVariance = 0.75f, Volume = 0.3f };
        SoundEngine.PlaySound(starSingle, Projectile.position);

        var currentPosition = Projectile.Center;
        FXUtil.GlowCircleBoom(currentPosition, DrawUtilities.Prismatic, DrawUtilities.Prismatic, Color.Black, 24);
        var pos = Main.rand.NextVector2Circular(7, 7) + currentPosition;
        var color = DrawUtilities.Prismatic.ToVector4();
        for (var i = 0; i < 12; i++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = Main.rand.NextVector2Circular(15, 15),
                timeLeft = 60,
                innerColor = Vector4.Lerp(color, Vector4.One, 0.65f),
                outerColor = color,
                scale = new Vector2(Main.rand.NextFloat(0.3f, 0.8f))
            });
        }

        Particles.DiamondSparkle.Spawn(new()
        {
            position = currentPosition + Main.rand.NextVector2Circular(7, 7),
            timeLeft = 30,
            color = DrawUtilities.Prismatic
        });
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        DeathFX();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.White, Color.Transparent, 0.3f);
        var drawer = Projectile.Drawer;
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    void MakeDusts()
    {
        var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
        var vel = Main.rand.NextVector2Circular(1, 1);
        Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
        {
            position = pos,
            velocity = vel,
            innerColor = DrawUtilities.Prismatic.ToVector4(),
            outerColor = Color.Lerp(DrawUtilities.Prismatic, Color.Black, 0.5F).ToVector4(),
            timeLeft = 60,
            scale = new Vector2(Main.rand.NextFloat(0.4f, 1f))
        });

        pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
        vel = Main.rand.NextVector2Circular(1, 1);
        Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
        {
            position = pos,
            velocity = vel,
            innerColor = DrawUtilities.Prismatic.ToVector4(),
            outerColor = Color.Lerp(DrawUtilities.Prismatic, Color.Black, 0.5F).ToVector4(),
            timeLeft = 60,
            scale = new Vector2(Main.rand.NextFloat(0.4f, 1f))
        });

        if (Main.rand.NextBool(8))
        {
            pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            vel = Main.rand.NextVector2Circular(1, 1);
            //    var sp = SparkleParticle.Spawn(pos, vel)
            Particles.DiamondSparkle.Spawn(new()
            {
                position = pos,
                color = DrawUtilities.Prismatic,
                timeLeft = 24,
            });
        }

        if (Main.rand.NextBool(32))
        {
            pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            vel = Main.rand.NextVector2Circular(1, 1);
            var sp = SparkleParticle.Spawn(pos, vel);
            sp.innerColor = Color.White;
            sp.outerColor = DrawUtilities.Prismatic;
            sp.gravity = 0;
            sp.Scale *= 0.4F;
        }

        if (Main.rand.NextBool(4))
        {
            pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            vel = Main.rand.NextVector2Circular(1, 1);
            Particles.FaintSmokeDust.Spawn(new()
            {
                position = pos,
                velocity = vel,
                timeleft = 14,
                color = DrawUtilities.Prismatic,
            });
        }
    }
}

public class NiiviBlastBreath : ModProjectile
{
    Vector2 _hitPoint;
    public override string Texture => ModType.NoTexture;
    float Time => 120;
    ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = true;
        Projectile.light = 0.7F;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = (int)Time;
    }
    public override bool CanHitPlayer(Player target)
    {
        return Timer < 36;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return CollisionHelper.LaserCollision(projHitbox, targetHitbox, Projectile.Center, Projectile.Center + Projectile.velocity, 64);
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
            _hitPoint = CollisionHelper.SolidTileRayCast(Projectile.Center, Projectile.velocity, Projectile.velocity.Length());
            for(var i = 0; i < 128; i++)
            {
                var pos = Vector2.Lerp(Projectile.Center, _hitPoint, Main.rand.NextFloat(0f, 1f));
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4,
                    timeLeft = 60,
                    innerColor = DrawUtilities.Prismatic.ToVector4(),
                    outerColor = DrawUtilities.Prismatic.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.24f, 0.5f)),
                });
            }
            for(var i = 0; i < 5; i++)
            {
                var firer = ProjFirer.From<NiiviFlyingStar>(Projectile);
                firer.damage /= 3;
                firer.position = _hitPoint + Main.rand.NextVector2Circular(64, 64);
                firer.velocity = -Vector2.UnitY * 12;
                firer.velocity = firer.velocity.RotatedByRandom(1.5F);
                firer.New();
            }

            var fx = FXUtil.GlowCircleBoom(_hitPoint, Color.White, Color.SkyBlue, Color.DarkBlue);
            fx.Scale *= 2f;

            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(5, 15));
            ParticleUtils.CreateSwirlingDustBurst(fac);

            ShakeScreenPosition.Shake = 4;
            FXUtil.ShakeCamera(_hitPoint, 1024, 16);

            var sound = AssetReferences.Assets.Sounds.NStarblast.Asset with { PitchVariance = 0.5f };
            if (Main.rand.NextBool(2))
                sound = AssetReferences.Assets.Sounds.NStarblast2.Asset with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(sound, Projectile.position);

            for (var i = 0; i < 7; i++)
            {
                var pos2 = _hitPoint;
                pos2.X += Main.rand.NextFloat(-100, 100);
                Particles.IceSpike.Spawn(new()
                {
                    position = pos2,
                    rotation = Main.rand.NextFloat(-0.3f, 0.3f),
                    timeLeft = 60,
                    timediv = 60,
                });
            }

            for (var i = 0; i < 7; i++)
            {
                var pos2 = _hitPoint;
                pos2.X += Main.rand.NextFloat(-100, 100);
                Particles.IceSpike.Spawn(new()
                {
                    position = pos2,
                    rotation = Main.rand.NextFloat(-0.3f, 0.3f),
                    timeLeft = 120,
                    timediv = 120,
                });
            }

            for (var i = 0; i < 4; i++)
            {
                var pos2 = _hitPoint;
                pos2.X += Main.rand.NextFloat(-100, 100);
                Particles.FaintSmokeDust.Spawn(new()
                {
                    position = pos2,
                    velocity = Vector2.Zero,
                    timeleft = 45,
                    color = Color.SkyBlue * 0.14f
                });
            }
        }
    }


    float GetTrailWidth(float ratio)
    {
        return MathHelper.Lerp(48, 0, EasingFunction.OutSine(Timer / Time));
    }

    float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.3f;
    }

    Color GetTrailColor(float ratio)
    {
        Color prismatic = Color.Red;
        prismatic.ScrollHue(Timer * 24 + ratio * 255);
        return prismatic;
    }

    Color GetTrailColor2(float ratio)
    {
        Color prismatic = Color.Red;
        prismatic.ScrollHue(Timer * 24 + ratio * 255);
        return prismatic;
    }

    void DrawPixelatedBeam(GraphicsDevice gDevice)
    {
        BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
        bloomTrailShader.InnerColor = Main.DiscoColor;
        bloomTrailShader.OuterColor = Main.DiscoColor;

        TrailDrawer.ClearPrimitives();
        TrailDrawer.PreparePrimitives(Projectile.oldPos, GetTrailColor2, GetTrailWidth2, Projectile.Size * 0.5f);
        TrailDrawer.DrawCached(bloomTrailShader);

        var pass = AssetReferences.Effects.Niivi.PrismaticBreath.CreatePrimitivesPass();
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.distortionStrength = 0.03f;
        pass.Apply();
        gDevice.Register(0, AssetReferences.Assets.LaserTextures.TexturedLaser.Asset.Value, SamplerState.LinearWrap);
        gDevice.Register(1, AssetReferences.Assets.Noise.PerlinBlurred.Asset.Value, SamplerState.LinearWrap);


        TrailDrawer.ClearPrimitives();
        TrailDrawer.PreparePrimitives(Projectile.oldPos, GetTrailColor, GetTrailWidth, Projectile.Size * 0.5f);
        TrailDrawer.DrawCached(pass.Shader);
    }

    void DrawPixelatedBeamImpact(SpriteBatch sb, Vector2 sp)
    {
        if (Timer < 4)
        {
            var impactFrameDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.ImpactFrame.Asset, _hitPoint);
            sb.Draw(impactFrameDrawer);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedBeam, DrawLayer.OverPlayers);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedBeamImpact, DrawLayer.OverPlayers);
        return false;
    }
}
