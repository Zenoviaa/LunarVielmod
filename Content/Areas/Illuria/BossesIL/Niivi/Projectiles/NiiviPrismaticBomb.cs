using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Core.Camera;
using Stellamod.Core.Palettes;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;


/*
 * 
 * 
 * The prismatic bobm is a bit different from legacy in that it'll aggressively charge at you multiple times
 * On the final charge it'll come crashing down and explode into little blobs afterwards that'll again attempt to dash into you
 * It's a very big attack
 * 
 * 
 * With each dash it rips the air and leaves like black afterimage sparkling stars with a cutting outline around them
 * There's a ripping sound everytime it whips by, Zemmi will wdo some cool sounds for this
 * 
 * When it is growing we're going to make use of metaballs to make it look a lot cooler and actually blobby, so the visuals we'll be much more
 * well done than legacy was
 * 
 * Many particles going into it and swirling/wraping around it
 */
public class NiiviPrismaticBomb : ModProjectile
{
    Vector2[] InitialMetaballPoints
    {
        get
        {
            if(field == null)
            {
                field = new Vector2[32];
                for(var i = 0; i < field.Length; i++)
                {
                    ref var normalOffset = ref field[i];
                    normalOffset = Main.rand.NextVector2Circular(1, 1);
                }
            }
            return field;
        }
    }
    Vector2[] _metaballs = new Vector2[32];
    bool _die;
    float _prisAlpha;

    float _metaballBursting;
    float _growthAlpha;
    float _flashAlpha;
    float _actualScale;
    float _attackCount;
    float _growthCount;
    float _targetScale;
    Vector2 _directionToDash;
    Vector2 _initialVelocity;
    Vector2 _bounceStartPosition;
    Vector2 _bounceEndPosition;

    enum AIState : byte
    {
        Charging,
        GetREADYLOL,
        FALL,
        BOUNCE,
        BOUNCEBOOM,
        Explode
    }

    float _faceAlpha;
    int _frame;
    int FrameCount => 2;
    float OrbScaleDivSquared => 25 * 25;
    Vector2 _squishScale;
    Vector2 OrbScale => Vector2.Lerp(Vector2.One, new Vector2(1.2f, 0.9f), Projectile.velocity.LengthSquared() / OrbScaleDivSquared) * _squishScale;
    float MetaballVelocityInfluence => 0.02f;
    float MetaballDiv => HitboxRadius;
    float MetaballRotDistanceOffset => 64;
    float MetaballRotSpeed => 3;
    float MetaballScalar => MathHelper.Lerp(1f, HitboxRadius, _growthCount) * 0.1f;
  
    float GetReadyTime => 100;
    float ChargeTime => 100;
    float MaxGrowthCount => 3;
    float HitboxRadius => 384 * _growthCount;
    float BounceTime => 120;
    float BounceHeight => 600;
    float BounceCount => 4;
    ref float Timer => ref Projectile.ai[0];
    AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    Player TargetPlayer => Main.player[(int)Projectile.ai[2]];

    public override string Texture => ModType.NoTexture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_bounceStartPosition);
        writer.WriteVector2(_bounceEndPosition);
        writer.WriteVector2(_initialVelocity);
        writer.WriteVector2(_directionToDash);
        writer.Write(_targetScale);
        writer.Write(_attackCount);
        writer.Write(_growthCount);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _bounceStartPosition = reader.ReadVector2();
        _bounceEndPosition = reader.ReadVector2();
        _initialVelocity = reader.ReadVector2();
        _directionToDash = reader.ReadVector2();
        _targetScale = reader.ReadSingle();
        _attackCount = reader.ReadSingle();
        _growthCount = reader.ReadSingle();
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (State == AIState.Charging)
            return false;
        return MoonUtils.CircularHitbox(projHitbox, targetHitbox, HitboxRadius * 0.25F);
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(128);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.WidthAndHeight = 64;
        Projectile.hostile = true;
        Projectile.timeLeft = 1800;
        Projectile.tileCollide = true;
        Projectile.light = 1;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();
  
        SimulateMetaballs();
        AmbientElectricParticles();
        _flashAlpha *= 0.98f;
        var gc = _growthCount / 3f;
        _growthAlpha = MathHelper.Lerp(_growthAlpha, gc, 0.1f);
        _faceAlpha = 0;
        _metaballBursting = MathHelper.Lerp(_metaballBursting, 1f, 0.1f);
        _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.06f);
        _frame = 0;
        switch (State)
        {
            case AIState.Charging:
                AI_Charging();
                break;
            case AIState.GetREADYLOL:
                AI_GetReady();
                break;
            case AIState.FALL:
                AI_Fall();
                break;
            case AIState.BOUNCE:
                AI_Bounce();
                break;
            case AIState.BOUNCEBOOM:
                AI_BOUNCEBOOM();
                break;
            case AIState.Explode:
                AI_Explode();
                break;
        }
        _actualScale = MathHelper.Lerp(_actualScale, _targetScale, 0.1f);
    }


    void SimulateMetaballs()
    {
        var scalar = MetaballScalar;

        //For this projectiles visual, we're going to use metaballs to get a blobby effect
        //First let's spawn initial offsets
        for(var i = 0; i < _metaballs.Length; i++)
        {
            ref var metaball = ref _metaballs[i];

            //We now have the metaballs origined around 0,0
            //So we apply a scalar to them
            var metaballOrigin = InitialMetaballPoints[i].RotatedBy(Main.GlobalTimeWrappedHourly)
                * scalar 
                * MathHelper.Lerp(24F, 0.6f, _growthAlpha) 
                * _metaballBursting
                * ExtraMath.Osc(0.5f, 1f, speed: 16);

            //This is the origin point for the metabll
            var metaballOffset = -Vector2.UnitY * MetaballRotDistanceOffset;
            var speed = 2f;
            var rotatedOffset = metaballOffset.RotatedBy(Main.GlobalTimeWrappedHourly * MetaballRotSpeed * speed + i * 4);
            var metaballPosition = metaballOrigin + rotatedOffset;
            metaballPosition += Projectile.velocity * MetaballVelocityInfluence;
            metaballPosition = Vector2.Lerp(metaballPosition, metaballOrigin, ExtraMath.Osc(0f, 1f, speed: 6, i * 4));
               
            metaball = metaballPosition / MetaballDiv;
            metaball += new Vector2(0.5f);
            //This should give a bunch of points that are rotating clockwise around origin points, creating an interesting visual
            //Hopefully, unsure lol
        }
    }
    void SwitchState(AIState state)
    {
        if (this.OwnedByLocalClient())
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }
    }

    void AmbientElectricParticles()
    {

    }
    void AmbientDashingParticles()
    {

        if (Main.rand.NextBool(1))
        {
            Particles.DiamondSparkle.Spawn(new()
            {
                position = Projectile.Center + Main.rand.NextVector2Circular(300, 300),
                velocity = Main.rand.NextVector2Circular(3, 3),
                color = DrawUtilities.Prismatic,
                timeLeft = 30
            });
        }

        for (var i =0;i < 2; i++)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(192, 192);
            var vel = Main.rand.NextVector2Circular(6, 6);
            Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                timeLeft = 60,
                innerColor = DrawUtilities.Prismatic.ToVector4(),
                outerColor = DrawUtilities.Prismatic.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.15f, 0.5F))
            });
        }



        for (var i = 0; i < 2; i++)
        {
            var pos2 = Projectile.Center + Main.rand.NextVector2Circular(384, 384);
            var vel2 = -Projectile.velocity.RotatedBy(Projectile.rotation);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos2,
                velocity = vel2,
                timeLeft = 60,
                innerColor = DrawUtilities.Prismatic.ToVector4(),
                outerColor = DrawUtilities.Prismatic.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.35f, 1f))
            });
        }
    }

    void AmbientSuckingParticles()
    {
        //The thing zemmie likes is when there's just a otn of these
        //Let's mix the bit dust and with the stretch particles and a few swirly particles, that should make it look very dynamic.

        //First stretch particles
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Vector2 GetStretchSpawnPoint()
        {
            var edge = Main.rand.NextFloat(444, 666);
            return Projectile.Center + Main.rand.NextVector2CircularEdge(edge, edge);
        }

        if (Timer % 2 == 0)
        {
            var pos = GetStretchSpawnPoint();
            var vel = Projectile.Center - pos;
            vel *= 0.13f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = DrawUtilities.Prismatic;
            fx.InnerColor = DrawUtilities.Prismatic;
            fx.GlowColor = DrawUtilities.Prismatic;
            fx.Scale *= 3 * Main.rand.NextFloat(1f, 2f);
        }
        {
            for(var i = 0; i < 8; i++)
            {
                var upOffset = Vector2.UnitY * MathHelper.Lerp(1024, 0, Timer / ChargeTime);
                upOffset = upOffset.RotatedBy((float)i / 8f * 6.28F);
                upOffset = upOffset.RotatedBy(Timer / ChargeTime * 6.28F);
                var pos = Projectile.Center + upOffset;
                var vel = Vector2.Zero;

                var color = Color.Red;
                color.ScrollHue((float)i / 8F * 360F + Main.GlobalTimeWrappedHourly);

                BitDustFactory fac = new();
                fac.color = Color.White.ToVector4();
                fac.position = pos;
                fac.velocity = vel;
                fac.innerColor = color.ToVector4();
                fac.outerColor = color.ToVector4();
                fac.timeLeft = 120;
                fac.scale = new Vector2(Main.rand.NextFloat(0.3f, 0.8f)) * 3;
                fac.timeLeft = 120;
                fac.velocityPerTickMult = 1;
                Particles.BitDust.Spawn(fac);

                if (Main.rand.NextBool(4))
                {
                    Particles.DiamondSparkle.Spawn(new()
                    {
                        position = pos,
                        velocity = vel,
                        color = color,
                        timeLeft = 30
                    });
                }
            }
        }
            
        { 
            var pos = GetStretchSpawnPoint();
            var vel = Projectile.Center - pos;
            vel *= 0.13f;
            Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = DrawUtilities.Prismatic.ToVector4(),
                outerColor = DrawUtilities.Prismatic.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.3f, 0.8f)),
                timeLeft = Main.rand.Next(30, 60)
            });
        }

        {
            var pos = Projectile.Center;
            var vel = Main.rand.NextVector2CircularEdge(25, 25);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = DrawUtilities.Prismatic.ToVector4(),
                outerColor = DrawUtilities.Prismatic.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.3f, 0.8f)),
                timeLeft = Main.rand.Next(30, 60)
            });
        }
    }
    void Grow()
    {
        SoundStyle sound;
        switch (_growthCount)
        {
            default:
            case 0:
                sound = AssetReferences.Assets.Sounds.PrimGrow1.Asset;
                break;
            case 1:
                sound = AssetReferences.Assets.Sounds.PrimGrow1.Asset;
                break;
            case 2:
                sound = AssetReferences.Assets.Sounds.PrimGrow2.Asset;
                break;
        }

        _flashAlpha = 1f;
        SoundEngine.PlaySound(sound);

        PixelPrimitiveCircleFactory.CreateGenericInBoom(Projectile.Center, DrawUtilities.Prismatic, DrawUtilities.Prismatic, 24, 777 * (1f + _growthCount / MaxGrowthCount));
        PixelPrimitiveCircleFactory.CreateGenericInBoom(Projectile.Center, DrawUtilities.Prismatic, DrawUtilities.Prismatic, 36, 666 * (1f + _growthCount / MaxGrowthCount));
        PixelPrimitiveCircleFactory.CreateGenericInBoom(Projectile.Center, DrawUtilities.Prismatic, DrawUtilities.Prismatic, 48, 512 * (1f + _growthCount / MaxGrowthCount));

        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, DrawUtilities.Prismatic, DrawUtilities.Prismatic, 64, 256 * (_growthCount + 1));
        FXUtil.CreateRipple(Projectile.Center);

        //Create a burst of particles
        for(var i = 0; i < 120; i++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = Projectile.Center,
                velocity = Main.rand.NextVector2Circular(65, 65),
                innerColor = DrawUtilities.Prismatic.ToVector4(),
                outerColor = DrawUtilities.Prismatic.ToVector4(),
                timeLeft = Main.rand.Next(60, 180),
                scale = new Vector2(Main.rand.NextFloat(0.3f, 2f))
            });
        }

        _targetScale = MathHelper.Lerp(0.33f, 1f, _growthCount / MaxGrowthCount);
        _growthCount++;
    }

    void FocusOnMe()
    {
        //Halfway point between particles
        CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, Projectile.Center, 0.5f));
    }
    void FocusOnMeHARDER()
    {
        //Halfway point between particles
        CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, Projectile.Center, 0.75f));
    }
    public override bool ShouldUpdatePosition()
    {
        if (State == AIState.Charging)
            return false;
        if (State == AIState.BOUNCE)
            return false;
        if (State == AIState.BOUNCEBOOM)
            return false;
        return base.ShouldUpdatePosition();
    }

    bool NeedsToGrowMore()
    {
        return _growthCount < MaxGrowthCount;
    }

    void AI_Charging()
    {

        Timer++;
        if(Timer == 1 && _growthCount == 0)
        {
            var sound = AssetReferences.Assets.Sounds.PrimAm.Asset with { Volume = 0.3f };
            SoundEngine.PlaySound(sound);
        }
        if(_growthCount != 0)
        {
            var contrast = MathHelper.Lerp(1.16f, 1f, EasingFunction.InSine(Timer / 30F));
            var brightness = MathHelper.Lerp(-0.04f, 0f, EasingFunction.InSine(Timer / 30F));
            FXUtil.ApplyContrastBrightness(contrast, brightness);

        }


        if (Timer >= ChargeTime)
        {
            Timer = 0;
            if (NeedsToGrowMore())
            {
                Grow();
                if (!NeedsToGrowMore())
                {
                    SwitchState(AIState.GetREADYLOL);
                }
            }
            else
            {
                SwitchState(AIState.GetREADYLOL);
            }
        }

        float strengthPerLevel = 64;
        float min = strengthPerLevel * _growthCount;
        float max = min + strengthPerLevel;
        float prisPullStrength = MathHelper.Lerp(min, max, Timer / ChargeTime);
        float alpha = MathHelper.Lerp(0f, 1f, Timer / ChargeTime);
        if (_growthCount > 0)
            alpha = 1f;
        FXUtil.ApplyPrismaticPull(Projectile.Center, -prisPullStrength, alpha);

        ShakeScreenPosition.Shake = MathHelper.Lerp(0f, 3f, (Timer / ChargeTime) * _growthCount);
        FocusOnMe();
        AmbientSuckingParticles();

        //we need to also simulate particles, but that'll be in a different function
        Projectile.rotation += 0.05f;
    }
    void AI_GetReady()
    {
        Timer++;

        _faceAlpha = MathHelper.Lerp(0f, 1f, Timer / GetReadyTime);
        FocusOnMe();
        AmbientDashingParticles();


        float strengthPerLevel = 64;
        
        float max = strengthPerLevel * _growthCount;
        float prisPullStrength = MathHelper.Lerp(max, 0, EasingFunction.InOutExpo(Timer / GetReadyTime));
        FXUtil.ApplyPrismaticPull(Projectile.Center, -max, MathHelper.Lerp(1f, 0f, EasingFunction.InSine(Timer / GetReadyTime)));

        //we need to also simulate particles, but that'll be in a different function
        Projectile.rotation += 0.05f;
        if (Timer >= GetReadyTime)
        {
            SwitchState(AIState.FALL);
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if(State == AIState.FALL)
        {
            SwitchState(AIState.BOUNCEBOOM);
        }
        return false;
    }
    void AI_Fall()
    {
        Timer++;
        if(Timer == 1)
        {
            Projectile.velocity.Y = -9;
        }
        if (Projectile.velocity.Y < 4)
            Projectile.velocity.Y += 0.15f;
        else
        {
            Projectile.velocity.Y *= 1.05f;

            if(Timer % 8 == 0)
            {
                var pris = DrawUtilities.Prismatic;
                var outer = Color.Lerp(pris, Color.Transparent, 0.5f);
                var fadeTo = Color.Lerp(outer, Color.Transparent, 0.5f);
                FXUtil.GlowDonut(Projectile.Bottom, -Projectile.velocity.SafeNormalize(Vector2.Zero), pris, outer, fadeTo, 2f);
            }

        }

        _faceAlpha = 1f;
        FocusOnMe();
        AmbientDashingParticles();

        //we need to also simulate particles, but that'll be in a different function
        Projectile.rotation += 0.05f;
    }

    void BounceBoomNoWave()
    {

        // PixelPrimitiveCircleFactory.CreateGenericInBoom(Projectile.Center, Color.White, Color.White, 42, 512 * (1f + _growthCount / MaxGrowthCount));
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, DrawUtilities.Prismatic, DrawUtilities.Prismatic, 42, 256 * (_growthCount + 1));
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, DrawUtilities.Prismatic, DrawUtilities.Prismatic, 42, 512 * (_growthCount + 1));
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, DrawUtilities.Prismatic, DrawUtilities.Prismatic, 42, 1024 * (_growthCount + 1));

        FXUtil.CreateRipple(Projectile.Center);
        FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
        ShakeScreenPosition.Shake = 4;

        //Create a burst of particles
        for (var i = 0; i < 128; i++)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
            var vel = Main.rand.NextVector2Circular(128, 128);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                timeLeft = Main.rand.Next(60, 120),
                innerColor = DrawUtilities.Prismatic.ToVector4(),
                outerColor = DrawUtilities.Prismatic.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.4f, 1f))
            });
        }
    }
    void BounceBoom()
    {
        if (this.OwnedByLocalClient())
        {
            var firer = ProjFirer.From<PrismaticBombWave>(Projectile);
            firer.position = Projectile.Bottom;
            if (_die)
                firer.ai1 = 1;
            firer.New();
        }
        _squishScale = new Vector2(1.3f, 0.6f);
        _flashAlpha = 1f;
        BounceBoomNoWave();
        SoundStyle sound = AssetReferences.Assets.Sounds.PrimGrow.Asset with { PitchVariance = 1f };
        SoundEngine.PlaySound(sound);


    }

    void AI_Bounce()
    {
        //Ok so, 
        Timer++;
        if(Timer == 1)
        {
            NewBounceParameters();
            Projectile.netUpdate = true;
        }
        var ratio = Timer / BounceTime;
        if(ratio < 0.5f)
        {
            _frame = 1;
        }
        var posToMoveTo = Vector2.Lerp(_bounceStartPosition, _bounceEndPosition, ratio);
        var yHeight = MathHelper.Lerp(0, BounceHeight, EasingFunction.OutExpo(ratio));
        var yDown = MathHelper.Lerp(BounceHeight, 0, EasingFunction.InExpo(ratio));
        var yOff = MathHelper.Lerp(yHeight, yDown, ratio);
        posToMoveTo.Y -= yOff;
        Projectile.Center = posToMoveTo;
        _faceAlpha = 1f;
        var lerp1 = MathHelper.Lerp(0, 1, EasingFunction.OutExpo(ratio));
        var lerp2 = MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(ratio));
        var lerp3 = MathHelper.Lerp(lerp1, lerp2, ratio);
        Projectile.velocity = Vector2.UnitX * lerp3 * 25 + Vector2.Lerp(Vector2.Zero, Vector2.UnitX * 45, EasingFunction.InExpo(ratio));
        var rot = (Projectile.position - Projectile.oldPosition).ToRotation();
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, rot, 0.1f);
        FocusOnMe();
        AmbientDashingParticles();

        if(ratio >= 0.5f)
        {
            ShakeScreenPosition.Shake = MathHelper.Lerp(0f, 3f, (ratio - 0.5f) / 0.5F);
            for(var i = 0;i < 2; i++)
            {
                var dir = Projectile.rotation.ToRotationVector2();
                var pos = Projectile.Center + dir * 256 + Main.rand.NextVector2Circular(64, 64);
                var vel = -dir.RotatedBy(1.54F * (Main.rand.NextBool(2) ? -1 : 1)) * Main.rand.NextFloat(5, 15);
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = vel,
                    timeLeft = 30,
                    innerColor = Color.White.ToVector4(),
                    outerColor = DrawUtilities.Prismatic.ToVector4(),
                    scale = new Vector2(1)
                });
            }
            {
                var dir = Projectile.rotation.ToRotationVector2();
                var pos = Projectile.Center + dir * 256 + Main.rand.NextVector2Circular(64, 64);
                var vel = -dir.RotatedBy(1.14F * (Main.rand.NextBool(2) ? -1 : 1)) * Main.rand.NextFloat(45, 65);
                var fx = FXUtil.GlowStretch(pos, vel);
                fx.OuterGlowColor = DrawUtilities.Prismatic;
                fx.InnerColor = DrawUtilities.Prismatic;
                fx.GlowColor = DrawUtilities.Prismatic;
                fx.Scale *= 3 * Main.rand.NextFloat(1f, 2f);
            }
        }
        if (Timer % 8 == 0)
        {
            var pris = DrawUtilities.Prismatic;
            var outer = Color.Lerp(pris, Color.Transparent, 0.5f);
            var fadeTo = Color.Lerp(outer, Color.Transparent, 0.5f);
            FXUtil.GlowDonut(Projectile.Bottom, -rot.ToRotationVector2(), pris, outer, fadeTo, 2f);
        }



        if (Timer >= BounceTime)
        {
            SwitchState(AIState.BOUNCEBOOM);
        }
    }

    void NewBounceParameters()
    {
        if (this.OwnedByLocalClient())
        {
            _bounceStartPosition = Projectile.Center;
            _bounceEndPosition = TargetPlayer.Center;
            _bounceEndPosition.X += Main.rand.Next(-384, 384);
            _bounceEndPosition.Y = _bounceStartPosition.Y;
        }
    }
    void AI_BOUNCEBOOM()
    {
        Timer++;
        if (Timer == 1)
        {
            _attackCount++;

            BounceBoom();
            Projectile.netUpdate = true;
        }
        _faceAlpha = 0f;

        float strengthPerLevel = 64;
        float max = strengthPerLevel * _growthCount;
        FXUtil.ApplyPrismaticPull(Projectile.Center, max * EasingFunction.QuickOutSlowIn(Timer / 30F), 1f * EasingFunction.QuickOutSlowIn(Timer / 30F));



        _metaballBursting = MathHelper.Lerp(1f, 12F, EasingFunction.OutExpo(Timer / 30f));
        Projectile.rotation = 0;
        if(Timer >= 30)
        {
            if (_die)
            {
                BounceBoomNoWave();
                Projectile.Kill();
            }
            else
            {
                if (_attackCount >= BounceCount)
                {
                    SwitchState(AIState.Explode);
                }
                else
                {
                    SwitchState(AIState.BOUNCE);
                }
            }

        }
 
    }

    void HappyBricks()
    {
        var dir = TargetPlayer.Center.X < Projectile.Center.X ? -1 : 1;
        var maxHeight = 2048;
        var height = 172f;
        var copies = maxHeight / height;
        var randIndex = Main.rand.Next(3, (int)copies - 3);
        if (this.OwnedByLocalClient())
        {
            var origin = Projectile.Bottom;
            var firer = ProjFirer.From<NiiviPrismaticBrick>(Projectile);
            for (var i = 1; i < copies + 1; i++)
            {
                if (i == randIndex)
                    continue;

                firer.ai0 = origin.X;
                firer.ai1 = origin.Y;
                firer.ai2 = (-i * 0.75f);
                firer.position = origin + new Vector2(0, i * -height + 64);
                firer.velocity.X = dir;
                firer.New();
            }
        }
    }
    void AI_Explode()
    {
        Timer++;
        if (Timer == 1)
        {
            NewBounceParameters();
            Projectile.netUpdate = true;
        }
        _faceAlpha = 1f;
        _frame = 1;
        var ratio = Timer / BounceTime;
        var posToMoveTo = Vector2.Lerp(_bounceStartPosition, _bounceEndPosition, ratio);
        var yHeight = MathHelper.Lerp(0, BounceHeight, EasingFunction.OutExpo(ratio));
        var yDown = MathHelper.Lerp(BounceHeight, 0, EasingFunction.InExpo(ratio));
        var yOff = MathHelper.Lerp(yHeight, yDown, ratio);
        posToMoveTo.Y -= yOff;
        Projectile.Center = posToMoveTo;
        _faceAlpha = 1f;
        var lerp1 = MathHelper.Lerp(0, 1, EasingFunction.OutExpo(ratio));
        var lerp2 = MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(ratio));
        var lerp3 = MathHelper.Lerp(lerp1, lerp2, ratio);
        Projectile.velocity = Vector2.UnitX * lerp3 * 25;
        var rot = (Projectile.position - Projectile.oldPosition).ToRotation();
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, rot, 0.1f);

        FocusOnMeHARDER();
        AmbientDashingParticles();

        if (Timer >= BounceTime)
        {
            _die = true;
            HappyBricks();
            SwitchState(AIState.BOUNCEBOOM);
        }
    }



    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
   
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }

    Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Main.DiscoColor, ratio);
    }
    
    float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(144, 128, EasingFunction.OutSine(ratio));
    }

    void DrawPrismaticTrail(GraphicsDevice graphicsDevice)
    {
        var pass = AssetReferences.Effects.Niivi.PrismBombTrail.CreatePrimitivesPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.distortionStrength = 0.05f;
        pass.Apply();
        graphicsDevice.Textures[0] = AssetReferences.Assets.LaserTextures.FlameTrail.Asset.Value;
        graphicsDevice.Textures[1] = AssetReferences.Assets.Textures.Noise.PerlinNoise.Asset.Value;
        graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        graphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;
        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor, GetTrailWidth, pass.Shader, Projectile.Size * 0.5f);
    }

    void DrawPrismaticAura(SpriteBatch sb, Vector2 sp)
    {
        var pass = AssetReferences.Effects.Niivi.PrismBombAura.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.outsideColor = Main.DiscoColor.ToVector4();
        pass.Parameters.insideColor = Color.White.ToVector4();
        pass.Apply();
        using (sb.Ctx(sb.Parameters with { effect = pass.Shader, samplerState = SamplerState.LinearWrap, blendState = BlendState.Additive}))
        {
            var drawer = SpritebatchDrawer.FromTextureAsset(
                AssetReferences.Assets.Noise.PainterlyNoise.Asset, Projectile.Center);
            drawer.color = Color.White * 0.2f * MathHelper.Lerp(0F, 1f, _growthAlpha); ;
            drawer.scale = OrbScale * 6 * MathHelper.Lerp(0F, 1f, _growthAlpha); ;
            drawer.rotation = Projectile.rotation;
            sb.Draw(drawer);
        }
    }

    void DrawPrismaticOrb(SpriteBatch sb, Vector2 sp)
    {
        var pass = AssetReferences.Effects.Niivi.PrismBomb.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.outsideColor = Main.DiscoColor.ToVector4();
        pass.Parameters.insideColor = Main.DiscoColor.ToVector4();
        pass.Parameters.metaballRadius = MathHelper.Lerp(0.05f, 1f, _growthAlpha);
        pass.Parameters.metaballStrength = 0.01f;
        pass.Parameters.metaballs = _metaballs;
        pass.Parameters.outlineColor = Main.DiscoColor.ToVector4();

        var noiseTex = AssetReferences.Assets.LaserTextures.Aura.Asset;
        var ditherTex = AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset;
        pass.Parameters.spriteSize = noiseTex.Size();
        pass.Parameters.noiseTexelSize = ditherTex.GetTexelSize();
        pass.Apply();

        Main.graphics.GraphicsDevice.Textures[2] = ditherTex.Value;
        Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.LinearWrap;
        using (sb.Ctx(sb.Parameters with { effect = pass.Shader }))
        {
            var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.NoiseTextures.PerlinNoise.Asset, Projectile.Center);
            drawer.color = Color.White * MathHelper.Lerp(0.3F, 1f, _growthAlpha); ;
            drawer.scale = OrbScale * 1.3F;
            drawer.rotation = Projectile.rotation;
            sb.Draw(drawer);
        }
    }

    void DrawPrismaticFlash(SpriteBatch sb, Vector2 sp)
    {
        var starDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.Star2.Asset, Projectile.Center);
        starDrawer.color = Color.White;
        starDrawer.color.A = 0;
        starDrawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.One, _flashAlpha) * 8;
        starDrawer.rotation = _flashAlpha * 4.6f;
        sb.Draw(starDrawer);
    }
    void DrawFace(SpriteBatch sb, Vector2 sp)
    {
        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.Illuria.NPCs.HappySoul.Asset, Projectile.Center);
        drawer.color = Color.White * _faceAlpha;
        drawer.color.A = 0;
        drawer.worldPosition.Y += ExtraMath.Osc(0f, 32, speed: 2);
        drawer.rotation = ExtraMath.Osc(-0.05f, 0.05f, speed: 2);
        drawer.worldPosition += Projectile.rotation.ToRotationVector2() * 4;
        drawer.worldPosition += (TargetPlayer.Center - Projectile.Center).Resize(64);
        if(_frame == 1)
            drawer.worldPosition.Y += ExtraMath.Osc(0, 64F, speed: 16);
        drawer.VerticalFrame(_frame, FrameCount);
        drawer.CenterOrigin();
        sb.Draw(drawer);
        sb.Draw(drawer);
        sb.Draw(drawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        //The pixelation effect will be done in the shader, so no need to offload it to the pixelation render layer.
        PixelationManager.QueuePrimitivesDrawAction(DrawPrismaticTrail, DrawLayer.OverNPCs);
        DrawPrismaticAura(Main.spriteBatch, Main.screenPosition);
        DrawPrismaticOrb(Main.spriteBatch, Main.screenPosition);
        PixelationManager.QueueSpritebatchDrawAction(DrawPrismaticFlash, DrawLayer.OverPlayers);
        DrawFace(Main.spriteBatch, Main.screenPosition);
        return false;
    }
}


public class PrismaticBombWave : ModProjectile
{
    struct WaveData
    {
        public Vector2 position;
        public float time;
    }
    List<WaveData> Waves = new();
    ref float Timer => ref Projectile.ai[0];
    ref float ShowBricks => ref Projectile.ai[1];
    float _travelDistance;
    float Time => 600;
    float WaveTravelSpeed => 22;
    float WaveInteractDistanceSquared => 164 * 164;
    float DistanceBetweenWaves => 48;
    public override string Texture => AssetReferences.Assets.Textures.Illuria.NPCs.PrismaticWave.KEY;

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        var waveOrigin = Projectile.Bottom;
        waveOrigin.X += _travelDistance;

        var inverseWaveOrigin = Projectile.Bottom;
        inverseWaveOrigin.X -= _travelDistance;

        var rect1 = new Rectangle(0, 0, 128, 384);
        rect1.Location = waveOrigin.ToPoint(); ;
        rect1.Location += new Point(-32, -384);

        var rect2 = rect1;
        rect2.Location = inverseWaveOrigin.ToPoint(); ;
        rect2.Location += new Point(-32, -384);

        return rect1.Intersects(targetHitbox) || rect2.Intersects(targetHitbox);
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4800;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = true;
        Projectile.timeLeft = (int)Time;
        Projectile.width = 64;
        Projectile.height = 384;
        Projectile.light = 0.7f;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer < 60)
        {
            var contrast = MathHelper.Lerp(1.56f, 1f, EasingFunction.InSine(Timer / 60f));
            var brightness = MathHelper.Lerp(-0.04f, 0f, EasingFunction.InSine(Timer / 60f));
            FXUtil.ApplyContrastBrightness(contrast, brightness);

        }

        if (Timer >= 120)
            Projectile.Kill();
        //So basically for this effect, we need to calculate mid points for where the waves are and set t
        Waves.Clear();
        _travelDistance += MathHelper.Lerp(
            MathHelper.Lerp(WaveTravelSpeed, WaveTravelSpeed * 0.1f,
            EasingFunction.OutExpo(Timer / 30f)), WaveTravelSpeed, 
            EasingFunction.InOutExpo(Timer / 60f));
        var waveOrigin = Projectile.Center;
        waveOrigin.X += _travelDistance;

        var inverseWaveOrigin = Projectile.Center;
        inverseWaveOrigin.X -= _travelDistance;

        //Make particles come up
        if (Main.rand.NextBool(4))
        {
            void PillarFlame(Vector2 origin)
            {


                for (var j = 0; j < 4; j++)
                {
                    var t = origin;
                    t.Y += Main.rand.NextFloat(-512, 5);
                    Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
                    {
                        position = t,
                        velocity = Vector2.Zero,
                        innerColor = DrawUtilities.Prismatic.ToVector4(),
                        outerColor = DrawUtilities.Prismatic.ToVector4(),
                        timeLeft = Main.rand.Next(60, 120),
                        scale = new Vector2(Main.rand.NextFloat(0.5f, 2f)),
                    });
                }

            }
            for (var i = 0; i < 1; i++)
            {
                var randPos = waveOrigin + Main.rand.NextVector2Circular(64, 64);
                var randVelocity = -Vector2.UnitY * 16;
                randVelocity = randVelocity.RotatedByRandom(0.25f);
                Particles.DiamondSparkle.Spawn(new()
                {
                    color = DrawUtilities.Prismatic,
                    position = randPos,
                    velocity = randVelocity,
                    timeLeft = Main.rand.Next(30, 80),
                });

                PillarFlame(randPos);

                randPos = inverseWaveOrigin + Main.rand.NextVector2Circular(64, 64);
                randVelocity = -Vector2.UnitY * 16;
                randVelocity = randVelocity.RotatedByRandom(0.25f);
                Particles.DiamondSparkle.Spawn(new()
                {
                    color = DrawUtilities.Prismatic,
                    position = randPos,
                    velocity = randVelocity,
                    timeLeft = Main.rand.Next(30, 80),
                });

                PillarFlame(randPos);

            }
        }


        var numWaveDatas = 32;
        var halfWaveDatas = numWaveDatas / 2;
        for(var i = -halfWaveDatas; i < halfWaveDatas; i++)
        {
            var wavePosition = Projectile.Center;
            wavePosition.X += i * DistanceBetweenWaves;

            WaveData waveData = new();
            waveData.position = wavePosition;
            var d = Vector2.DistanceSquared(wavePosition, waveOrigin);
            var d2 = Vector2.DistanceSquared(wavePosition, inverseWaveOrigin);
            var closest = MathF.Min(d, d2);
            closest += 4;

            var lerp = MathHelper.Clamp(closest / WaveInteractDistanceSquared, 0f, 1f);
            lerp = 1f - lerp;


            waveData.time = lerp;
            Waves.Add(waveData);
        }
    }

    void DrawBricks()
    {
        if (ShowBricks == 0)
            return;

        for (var i = 0; i < 32; i++)
        {
            var wave = Waves[i];
            var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.Illuria.NPCs.HappyBrick.Asset, wave.position);
            //     drawer.BottomCenterOrigin();
            drawer.worldPosition = Projectile.Bottom;
            drawer.worldPosition.Y -= 196;
            drawer.worldPosition.X += MathHelper.Lerp(-3155, 3155, (float)i / 32f);
            drawer.color = Color.Red * MathHelper.Lerp(1f, 0f, EasingFunction.InSine(Timer / 60F));
            drawer.color.ScrollHue(360 * (float)i / (float)Waves.Count + Main.GlobalTimeWrappedHourly * 4);
            drawer.color.A = 0;

            Main.spriteBatch.Draw(drawer);

            //  Main.spriteBatch.Draw(drawer);
        }
    }
    void DrawWavesOld()
    {
        var ditherTex = AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset;
        var pass = AssetReferences.Effects.Niivi.PrismWave.CreatePixelPass();
        pass.Parameters.spriteSize = Projectile.Drawer.Size;
        pass.Parameters.ditherTexelSize = ditherTex.GetTexelSize();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.ColorSpectrumTexture = PaletteAssets.FromPaletteFile("Prismatic").Value.ColorAtlas;
        pass.Apply();
        var gDevice = Main.graphics.graphicsDevice;

        gDevice.Textures[1] = AssetReferences.Assets.NoiseTextures.Crystals.Asset.Value;
        gDevice.SamplerStates[1] = SamplerState.AnisotropicWrap;


        gDevice.Textures[2] = AssetReferences.Assets.GlowMasks.PrismaticColorMap.Asset.Value;
        gDevice.SamplerStates[2] = SamplerState.LinearClamp;


        gDevice.Textures[3] = ditherTex.Value;
        gDevice.SamplerStates[3] = SamplerState.LinearWrap;


        using (Main.spriteBatch.Ctx(Main.spriteBatch.Parameters with { effect = pass.Shader, blendState = BlendState.Additive, samplerState = SamplerState.AnisotropicWrap }))
        {
            for (var i = 0; i < Waves.Count; i++)
            {
                var wave = Waves[i];
                var drawer = Projectile.Drawer;
                drawer.BottomCenterOrigin();
                drawer.worldPosition = wave.position;
                byte t = (byte)(MathHelper.Lerp(0.09f, 0.45F, wave.time) * 255F);
                byte o = (byte)(((float)i / (float)Waves.Count) * 255F);

                drawer.color = Color.White;
                drawer.color.R = o;
                drawer.color.A = t;

                drawer.scale.X *= 0.03F;
                drawer.scale.Y *= MathHelper.Lerp(0.5F, 2f, EasingFunction.InOutSine(wave.time)) * EasingFunction.InOutSine(Timer / 60f);

                Main.spriteBatch.Draw(drawer);

                //  Main.spriteBatch.Draw(drawer);
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawBricks();
        //    DrawWavesOld();
        var ditherTex = AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset;
        var waveOrigin = Projectile.Center;
        waveOrigin.X += _travelDistance;

        var inverseWaveOrigin = Projectile.Center;
        inverseWaveOrigin.X -= _travelDistance;


        var bottomOfWave = Projectile.Bottom;
        var topLeft = bottomOfWave - Projectile.Drawer.Size * 0.5f;
        var pass = AssetReferences.Effects.Niivi.PrismWaveV2.CreatePixelPass();
        pass.Parameters.spriteSize = Projectile.Drawer.Size;
        pass.Parameters.centerPoint = (waveOrigin - topLeft) / Projectile.Drawer.Size;
        pass.Parameters.centerPoint2 = (inverseWaveOrigin - topLeft) / Projectile.Drawer.Size;
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.ColorSpectrumTexture = PaletteAssets.FromPaletteFile("Prismatic").Value.ColorAtlas;
        pass.Parameters.ditherTexelSize = ditherTex.GetTexelSize();
        pass.Apply();
        var gDevice = Main.graphics.graphicsDevice;

        gDevice.Textures[1] = AssetReferences.Assets.Noise.Swirl.Asset.Value;
        gDevice.SamplerStates[1] = SamplerState.LinearWrap;

        gDevice.Textures[3] = ditherTex.Value;
        gDevice.SamplerStates[3] = SamplerState.LinearWrap;


        using (Main.spriteBatch.Ctx(Main.spriteBatch.Parameters with
        { 
            effect = pass.Shader, 
            blendState = BlendState.Additive, 
            samplerState = SamplerState.AnisotropicWrap }))
        {
            var drawer = Projectile.Drawer;
            drawer.BottomCenterOrigin();
            drawer.color = Color.White * MathHelper.Lerp(1f, 0f, EasingFunction.InQuad(Timer / 120F));
            drawer.worldPosition = bottomOfWave + new Vector2(0, -76);
            drawer.scale.Y *= MathHelper.Lerp(0.1f, 1f, EasingFunction.OutExpo(Timer / 180F));
            drawer.scale.X *= MathHelper.Lerp(0.1f, 1f, EasingFunction.OutExpo(Timer / 30F));
  //          drawer.scale.Y *= MathHelper.Lerp(1f, 0f, EasingFunction.InSine(Timer / 120));
            Main.spriteBatch.Draw(drawer);
        }
        return false;
    }
}

public class NiiviPrismaticBrick : ModProjectile
{
    float _fallSpeed;
    float _incre;
    float FallingTime => 240;
    Vector2 _initialPosition;
    Vector2 Origin => new Vector2(OriginX, OriginY);
    ref float OriginX => ref Projectile.ai[0];
    ref float OriginY => ref Projectile.ai[1];
    ref float Timer => ref Projectile.ai[2];
    public override string Texture => AssetReferences.Assets.Textures.Illuria.NPCs.HappyBrick.KEY;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_initialPosition);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _initialPosition = reader.ReadVector2();
    }
    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        _initialPosition = Projectile.Center;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(8);
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return base.Colliding(projHitbox, targetHitbox);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = true;
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.timeLeft = 350;
        Projectile.tileCollide = false;
        Projectile.light = 0.7f;
        Projectile.penetrate = -1;
    }
    public override void AI()
    {
        base.AI();
        Timer++; Projectile.SetTrailCacheLength(8);
        var rotateRadians = MathHelper.PiOver2 * MathF.Sign(Projectile.velocity.X);
        if(MathF.Abs(_incre) < MathF.Abs(rotateRadians))
        {
            _incre += _fallSpeed * MathF.Sign(Projectile.velocity.X);
            _fallSpeed += MathHelper.Lerp(0.0000085f, 0.001f, EasingFunction.InExpo(Timer / 240F));
        }
        else
        {
            Projectile.Kill();
        }
        var pos = Main.rand.NextVector2Circular(80, 80) + Projectile.Center;
        var vel = Main.rand.NextVector2Circular(4, 4);
        Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
        {
            position = pos,
            velocity = vel,
            innerColor = DrawUtilities.Prismatic.ToVector4(),
            outerColor = DrawUtilities.Prismatic.ToVector4(),
            timeLeft = 60,
            scale = new Vector2(Main.rand.NextFloat(0.5f, 1f))
        });
        ShakeScreenPosition.Shake = MathHelper.Lerp(0, 8, EasingFunction.InQuad(Timer / 240F));
        CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, Projectile.Center, 0.2f));
        var newPosition = _initialPosition.RotatedBy(_incre, Origin);
        Projectile.Center = newPosition;
        Projectile.rotation = (Projectile.Center - Origin).ToRotation();
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        var drawPosition = Vector2.Lerp(Origin, Projectile.Center, EasingFunction.OutExpo(Timer / 60f));
        var drawer2 = Projectile.Drawer;
        drawer2.worldPosition = drawPosition;
        drawer2.color = Main.DiscoColor * 0.25f;
        drawer2.color.A = 0;
    
        foreach(OldPosition oldPos in Projectile.IterateOldPosBackwards())
        {
            drawer2.worldPosition = oldPos.position + Projectile.Size * 0.5f;
            drawer2.color = Color.Red * 0.2f * MathHelper.SmoothStep(1f, 0f, oldPos.progress);
            drawer2.color.ScrollHue(255 * Main.GlobalTimeWrappedHourly + Projectile.identity * 36);
            drawer2.color.A = 0;
            Main.spriteBatch.Draw(drawer2);
        }
        var rotateRadians = MathHelper.PiOver2 * MathF.Sign(Projectile.velocity.X);
        var endPos = _initialPosition.RotatedBy(rotateRadians, Origin);
        drawer2.worldPosition = endPos;
        drawer2.rotation = 0;
        drawer2.color = Color.Red * 0.5f * ExtraMath.Osc(0.5f, 1f, speed: 16, endPos.X * 0.1f);
        drawer2.color.ScrollHue(255 * Main.GlobalTimeWrappedHourly + Projectile.identity * 36);
        drawer2.color.A = 0;
        Main.spriteBatch.Draw(drawer2);

        var drawer = Projectile.Drawer;
        drawer.worldPosition = drawPosition;
        drawer.color = Color.Red * 1f * ExtraMath.Osc(0.5f, 1f, speed: 16, endPos.X * 0.1f);
        drawer.color.ScrollHue(255 * Main.GlobalTimeWrappedHourly + Projectile.identity * 36);
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);

        drawer.color = Color.White * 1f * ExtraMath.Osc(0.5f, 1f, speed: 16, endPos.X * 0.1f);
        //drawer.color.ScrollHue(255 * Main.GlobalTimeWrappedHourly + Projectile.identity * 36);
        drawer.color.A = 0;
        drawer.scale *= 0.92f;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var sound = AssetReferences.Assets.Sounds.Melee.HammerSmash1.Asset with { PitchVariance = 0.7f, MaxInstances = 0 };
        SoundEngine.PlaySound(sound, Projectile.position);
        //FXUtil.GlowDonut(Projectile.Center, -Vector2.UnitY, Color.White, DrawUtilities.Prismatic, Color.Black);
        FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, DrawUtilities.Prismatic, Color.Black, 25, baseSize: 0.18F);
        fx.VectorScale.X *= 2;
        for(var i = 0; i < 4; i++)
        {
            var pos = Projectile.Center + new Vector2(Main.rand.Next(-64, 64), -80 * i);
            Particles.DiamondSparkle.Spawn(new()
            {
                position = pos,
                velocity = -Vector2.UnitY * 6,
                timeLeft = 30 + i * 3,
                color = DrawUtilities.Prismatic
            });
        }
        for(var i= 0; i < 40; i++)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(128, 16);
            var vel = -Vector2.UnitY * Main.rand.NextFloat(8, 25);
            vel = vel.RotatedByRandom(0.5f);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.Lerp(DrawUtilities.Prismatic, Color.White, 0.5F).ToVector4() ,
                outerColor = DrawUtilities.Prismatic.ToVector4(),
                timeLeft = Main.rand.Next(90, 300),
                scale = new Vector2(Main.rand.NextFloat(0.25f, 1.5F))
            });
        }
    }
}
public class NiiviPrismaticBombBoom : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => ModType.NoTexture;
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return MoonUtils.CircularHitbox(projHitbox, targetHitbox, 512);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60;
        Projectile.light = 5;
        Projectile.penetrate = -1;
        Projectile.width = 16;
        Projectile.height = 16;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(4, 50));
            fac.particleCount *= 12;
            ParticleUtils.CreateSwirlingDustCircle(fac);
            for(var i = 0; i < 128; i++)
            {
                Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = Projectile.Center + Main.rand.NextVector2Circular(64, 64),
                    velocity = Main.rand.NextVector2Circular(45, 45),
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.SkyBlue.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.2f, 1f)),
                    timeLeft = Main.rand.Next(30, 120),
                });
            }
            
            for (int i = 0; i < 32; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, speed * 17, Scale: 5f);
                d.noGravity = true;

                Vector2 speeda = Main.rand.NextVector2CircularEdge(4f, 4f);
                var da = Dust.NewDustPerfect(Projectile.Center, DustID.WhiteTorch, speeda * 11, Scale: 5f);
                da.noGravity = false;
            }

            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.Transparent, 30, 1024);
        }

        if(Timer > 24)
        {
            Projectile.hostile = false;
        }
        var contrast = MathHelper.Lerp(1.16f, 1f, EasingFunction.InSine(Timer / 60f));
        var brightness = MathHelper.Lerp(-0.04f, 0f, EasingFunction.InSine(Timer / 60f));
        FXUtil.ApplyContrastBrightness(contrast, brightness);
    }

    void DrawDarkness(SpriteBatch sb, Vector2 sp)
    {
        var squareDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.WhiteSquare.Asset, Vector2.Zero);
        squareDrawer.color = Color.Lerp(Color.Black * 0.3f, Color.Transparent, EasingFunction.OutSine(Timer / 30f));
        squareDrawer.drawOrigin = Vector2.Zero;
        squareDrawer.dstRect = new((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y, Main.screenWidth * 2, Main.screenHeight * 2);
    
        sb.Draw(squareDrawer);
        if (Timer < 4)
        {
            var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.Impact.Asset, Projectile.Center);
            drawer.color = Color.White;
            sb.Draw(drawer);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
   

        PixelationManager.QueueSpritebatchDrawAction(DrawDarkness, DrawLayer.OverPlayers);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}