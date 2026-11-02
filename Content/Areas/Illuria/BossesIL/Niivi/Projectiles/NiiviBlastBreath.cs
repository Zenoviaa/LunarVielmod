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
        Projectile.extraUpdates = 1;
        Projectile.timeLeft = 600;
    }

    void Flicker()
    {
        var currentPosition = Projectile.Center;
        var starSingle = AssetReferences.Assets.Sounds.Starrer.Asset with { PitchVariance = 0.75f, Pitch = -0.4f };
        SoundEngine.PlaySound(starSingle, Projectile.position);
        PixelPrimitiveCircleFactory.CreateGenericInBoom(Projectile.Center, DrawUtilities.Prismatic, DrawUtilities.Prismatic, 25, 64);

        var pos = Main.rand.NextVector2Circular(7, 7) + currentPosition;
        var color = DrawUtilities.Prismatic.ToVector4();
        for (var i = 0; i < 8; i++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = Main.rand.NextVector2Circular(25, 25),
                timeLeft = 60,
                innerColor = Vector4.Lerp(color, Vector4.One, 0.65f),
                outerColor = color,
                scale = new Vector2(Main.rand.NextFloat(0.3f, 0.8f))
            });
        }
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer < 100)
        {
            if (Projectile.velocity.Length() > 0.5f)
                Projectile.velocity *= 0.98f;
        } else if (Timer < 150)
        {
            if(Timer == 101)
            {
                Projectile.velocity = Vector2.One;
                Flicker();
            }
            Projectile.tileCollide = true;
            var player = PlayerHelper.FindClosestPlayer(Projectile.Center, 2048);
            if(player != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, player.Center, 4);
            }

            if (Projectile.velocity.Length() < 15)
                Projectile.velocity *= 1.01f;
        }
        else
        {
            Projectile.extraUpdates = 2;
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

    void DrawPixelated(SpriteBatch sb, Vector2 sp)
    {
        Projectile.scale = 0.4f;
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.White, Color.Transparent, 0.14f);
        var drawer = Projectile.Drawer;
       sb.Draw(drawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelated);

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
    Vector2 _leftStart;
    Vector2 _rightStart;
    Vector2 _leftImpactPoint;
    Vector2 _rightImpactPoint;
    Vector2 _hitPoint;
    public override string Texture => ModType.NoTexture;
    float Time => 45;
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
            _hitPoint += Projectile.velocity.SafeNormalize(Vector2.Zero) * 144;
            PixelPrimitiveCircleFactory.CreateGenericBoom(_hitPoint, Color.White, Color.White, 65, 1024);
            PixelPrimitiveCircleFactory.CreateGenericBoom(_hitPoint, Color.White, Color.White, 45, 512);
            var perpVel = Projectile.velocity.SafeNormalize(Vector2.Zero);
            _leftStart = Projectile.Center - perpVel * 0.4f ;
            _rightStart = Projectile.Center + perpVel * 0.4f ;


            var leftVelocity = _leftStart - Projectile.Center;
            leftVelocity = leftVelocity.SafeNormalize(Vector2.Zero);

            var rightVelocity = _rightStart - Projectile.Center;
            rightVelocity = rightVelocity.SafeNormalize(Vector2.Zero);

            _leftImpactPoint = CollisionHelper.SolidTileRayCast(_leftStart, leftVelocity, 2400);
            _rightImpactPoint = CollisionHelper.SolidTileRayCast(_rightStart, rightVelocity, 2400);


            for (var i = 0; i < 128; i++)
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
            if (this.OwnedByLocalClient())
            {
                for (var i = 0; i < 6; i++)
                {
                    var firer = ProjFirer.From<NiiviFlyingStar>(Projectile);
                    firer.damage /= 3;
                    firer.position = _hitPoint + Main.rand.NextVector2Circular(64, 64);
                    firer.velocity = -Vector2.UnitY * 17;
                    firer.velocity = firer.velocity.RotatedByRandom(1.5F);
                    firer.New();
                }

            }

            var fx = FXUtil.GlowCircleBoom(_hitPoint, Color.White, Color.SkyBlue, Color.DarkBlue);
            fx.Scale *= 4f;

            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(_hitPoint, TriColorPalette.Icey, new Vector2(5, 45));
            fac.particleCount *= 8;
            ParticleUtils.CreateSwirlingDustCircle(fac);

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
        FXUtil.ApplyContrastBrightness(MathHelper.Lerp(1.1f, 1f, EasingFunction.InSine(Timer / Time)), 0f);
    }


    float GetTrailWidth(float ratio)
    {
        return MathHelper.Lerp(MathHelper.Lerp(0, 128, EasingFunction.OutQuad(ratio)), 0, EasingFunction.OutSine(Timer / Time));
    }

    float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.3f;
    }

    Color GetTrailColor(float ratio)
    {
      
        return Color.White;
    }

    Color GetTrailColor2(float ratio)
    {
        Color prismatic = Color.Red;
        prismatic.ScrollHue(Timer * 24 + ratio * 255);
        prismatic = Color.Lerp(prismatic, Color.White, 0.9F);
        return prismatic * 0.4f;
    }

    void DrawPixelatedBeam(GraphicsDevice gDevice)
    {
        var points = DrawUtilities.InterpolateBetweenPoints(Projectile.Center, _hitPoint, 128);
        BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
        bloomTrailShader.InnerColor = Main.DiscoColor;
        bloomTrailShader.OuterColor = Main.DiscoColor;

        TrailDrawer.ClearPrimitives();
        TrailDrawer.PreparePrimitives(points, GetTrailColor2, GetTrailWidth2);
        TrailDrawer.DrawCached(bloomTrailShader);

        var pass = AssetReferences.Effects.Niivi.PrismaticBreath.CreatePrimitivesPass();
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.distortionStrength = 0.03f;
        pass.Apply();
        gDevice.Register(0, AssetReferences.Assets.LaserTextures.TexturedLaser.Asset.Value, SamplerState.LinearWrap);
        gDevice.Register(1, AssetReferences.Assets.Noise.PerlinBlurred.Asset.Value, SamplerState.LinearWrap);


        TrailDrawer.ClearPrimitives();
        TrailDrawer.PreparePrimitives(points, GetTrailColor, GetTrailWidth);
        TrailDrawer.DrawCached(pass.Shader);
    }

    void DrawPixelatedBeamImpact(SpriteBatch sb, Vector2 sp)
    {
        if (Timer < 4)
        {
            var impactFrameDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.ImpactFrame.Asset, _hitPoint);
            impactFrameDrawer.color = Color.Black;
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
