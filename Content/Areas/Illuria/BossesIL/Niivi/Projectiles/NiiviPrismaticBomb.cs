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
        Projectile.SetTrailCacheLength(64);
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
            var metaballOrigin = InitialMetaballPoints[i].RotatedBy(Main.GlobalTimeWrappedHourly) * scalar * MathHelper.Lerp(24F, 0.6f, _growthAlpha) * _metaballBursting;

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
        if (Main.rand.NextBool(2))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(192, 192);
            var vel = Main.rand.NextVector2Circular(1, 1);
            Particles.LightningSpikeySpark.Spawn(new()
            {
                position = pos,
                velocity = vel,
                timeLeft = 24,
                scale = Main.rand.NextFloat(0.4f, 1f),
                color = Color.Lerp(Color.White, Main.DiscoColor, 0.15f)
            });
        }

    }
    void AmbientDashingParticles()
    {
        for(var i =0;i < 2; i++)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(192, 192);
            var vel = Main.rand.NextVector2Circular(6, 6);
            Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                timeLeft = 60,
                innerColor = Color.White.ToVector4(),
                outerColor = Main.DiscoColor.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.3f, 1f))
            });
        }



        for (var i = 0; i < 2; i++)
        {
            var pos2 = Projectile.Center + Main.rand.NextVector2Circular(192, 192);
            var vel2 = -Projectile.velocity.RotatedBy(Projectile.rotation);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos2,
                velocity = vel2,
                timeLeft = 60,
                innerColor = Color.White.ToVector4(),
                outerColor = Main.DiscoColor.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.3f, 2f))
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

        if (Main.rand.NextBool(2))
        {
            var pos = GetStretchSpawnPoint();
            var vel = Projectile.Center - pos;
            vel *= 0.05f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Main.DiscoColor;
            fx.InnerColor = Color.White;
            fx.GlowColor = Color.DarkGray;
        }

        if (Main.rand.NextBool(2))
        {
            var pos = GetStretchSpawnPoint();
            var vel = Projectile.Center - pos;
            vel *= 0.05f;
            Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.White.ToVector4(),
                outerColor = Main.DiscoColor.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.3f, 0.8f)),
                timeLeft = Main.rand.Next(30, 60)
            });
        }

        if (Main.rand.NextBool(8))
        {
            var pos = GetStretchSpawnPoint();
            var vel = Projectile.Center - pos;
            vel *= 0.05f;
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.White.ToVector4(),
                outerColor = Main.DiscoColor.ToVector4(),
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

        SoundEngine.PlaySound(sound);
        PixelPrimitiveCircleFactory.CreateGenericInBoom(Projectile.Center, Color.White, Color.White, 42, 512 * (1f + _growthCount / MaxGrowthCount));
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.White, 42, 256 * (_growthCount + 1));
        FXUtil.CreateRipple(Projectile.Center);

        //Create a burst of particles
        var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(5, 65));
        factory.particleCount *= 12;
        ParticleUtils.CreateSwirlingDustCircle(factory);

        _targetScale = MathHelper.Lerp(0.33f, 1f, _growthCount / MaxGrowthCount);
        _growthCount++;
    }

    void FocusOnMe()
    {
        //Halfway point between particles
        CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, Projectile.Center, 0.25f));
    }
    void FocusOnMeHARDER()
    {
        //Halfway point between particles
        CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, Projectile.Center, 0.5f));
    }
    public override bool ShouldUpdatePosition()
    {
        if (State == AIState.Charging)
            return false;
        if (State == AIState.BOUNCE)
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
        if(Timer >= ChargeTime)
        {
            Timer = 0;
            if (NeedsToGrowMore())
            {
                Grow();
            }
            else
            {
                SwitchState(AIState.GetREADYLOL);
            }
        }
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
            Projectile.velocity.Y = -8;
        }
        if (Projectile.velocity.Y < 15)
            Projectile.velocity.Y += 0.15f;
        _faceAlpha = 1f;
        FocusOnMe();
        AmbientDashingParticles();

        //we need to also simulate particles, but that'll be in a different function
        Projectile.rotation += 0.05f;
    }

    void BounceBoom()
    {
        if (this.OwnedByLocalClient())
        {
            var firer = ProjFirer.From<PrismaticBombWave>(Projectile);
            firer.position = Projectile.Bottom;
            firer.New();
        }
        _squishScale = new Vector2(1.3f, 0.6f);
        _flashAlpha = 1f;
        SoundStyle sound = AssetReferences.Assets.Sounds.PrimGrow1.Asset with { PitchVariance = 1f };
        SoundEngine.PlaySound(sound);

       // PixelPrimitiveCircleFactory.CreateGenericInBoom(Projectile.Center, Color.White, Color.White, 42, 512 * (1f + _growthCount / MaxGrowthCount));
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.White, 42, 256 * (_growthCount + 1));
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.White, 42, 512 * (_growthCount + 1));
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.White, 42, 1024 * (_growthCount + 1));

        FXUtil.CreateRipple(Projectile.Center);
        FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
        ShakeScreenPosition.Shake = 4;

        //Create a burst of particles
        var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(5, 65));
        factory.particleCount *= 4;
        ParticleUtils.CreateSwirlingDustCircle(factory);
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
        Projectile.velocity = Vector2.UnitX * lerp3 * 25;
        var rot = (Projectile.position - Projectile.oldPosition).ToRotation();
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, rot, 0.1f);
        FocusOnMe();
        AmbientDashingParticles();
        if(Timer >= BounceTime)
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
   
        _metaballBursting = MathHelper.Lerp(1f, 12F, EasingFunction.OutExpo(Timer / 30f));
        Projectile.rotation = 0;
        if(Timer >= 30)
        {
            if (_die)
            {
        
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
            BounceBoom();
            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(4, 45));
            fac.particleCount *= 20;
            ParticleUtils.CreateSwirlingDustCircle(fac);
            Projectile.Kill();
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
        return Color.Lerp(Color.White, Color.Transparent, ratio);
    }
    
    float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(128, 32, ratio);
    }

    void DrawPrismaticTrail(GraphicsDevice graphicsDevice)
    {
        var pass = AssetReferences.Effects.Niivi.PrismBombTrail.CreatePrimitivesPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.distortionStrength = 0.05f;
        pass.Apply();
        graphicsDevice.Textures[0] = AssetReferences.Assets.LaserTextures.Aura.Asset.Value;
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
        pass.Parameters.insideColor = Color.White.ToVector4();
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
            drawer.scale = OrbScale * 1.1F;
            drawer.rotation = Projectile.rotation;
            sb.Draw(drawer);
        }
    }

    void DrawPrismaticFlash(SpriteBatch sb, Vector2 sp)
    {
        var starDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.Star2.Asset, Projectile.Center);
        starDrawer.color = Color.Lerp(Color.Lerp(Color.White, Main.DiscoColor, 0.4f), Color.White, ExtraMath.Osc(0f, 1f, speed: 16));
        starDrawer.color.A = 0;
        starDrawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.One, _flashAlpha) * 6;
        starDrawer.rotation = _flashAlpha * 4.6f;
        sb.Draw(starDrawer);
    }
    void DrawFace(SpriteBatch sb, Vector2 sp)
    {
        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.Illuria.NPCs.HappySoul.Asset, Projectile.Center);
        drawer.color = Color.White * _faceAlpha * 0.125f;
        drawer.worldPosition.Y += ExtraMath.Osc(0f, 32, speed: 2);
        drawer.rotation = ExtraMath.Osc(-0.05f, 0.05f, speed: 2);
        drawer.worldPosition += Projectile.rotation.ToRotationVector2() * 4;
        drawer.worldPosition += (TargetPlayer.Center - Projectile.Center).Resize(64);
        if(_frame == 1)
            drawer.worldPosition.Y += ExtraMath.Osc(0, 64F, speed: 16);
        drawer.VerticalFrame(_frame, FrameCount);
        drawer.CenterOrigin();
        sb.Draw(drawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        //The pixelation effect will be done in the shader, so no need to offload it to the pixelation render layer.
        PixelationManager.QueuePrimitivesDrawAction(DrawPrismaticTrail, DrawLayer.OverNPCs);
        DrawPrismaticAura(Main.spriteBatch, Main.screenPosition);
        DrawPrismaticOrb(Main.spriteBatch, Main.screenPosition);
        DrawPrismaticFlash(Main.spriteBatch, Main.screenPosition);
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
    float _travelDistance;
    float Time => 600;
    float WaveTravelSpeed => 32;
    float WaveInteractDistanceSquared => 128 * 128;
    float DistanceBetweenWaves => 64;
    public override string Texture => AssetReferences.Assets.Textures.Illuria.NPCs.PrismaticWave.KEY;

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        var waveOrigin = Projectile.Bottom;
        waveOrigin.X += _travelDistance;

        var inverseWaveOrigin = Projectile.Bottom;
        inverseWaveOrigin.X -= _travelDistance;

        var rect1 = new Rectangle(0, 0, 64, 384);
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
            var contrast = MathHelper.Lerp(1.16f, 1f, EasingFunction.InSine(Timer / 60f));
            var brightness = MathHelper.Lerp(-0.04f, 0f, EasingFunction.InSine(Timer / 60f));
            FXUtil.ApplyContrastBrightness(contrast, brightness);

        }

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
        for (var i = 0; i < 3; i++)
        {
            var randPos = waveOrigin + Main.rand.NextVector2Circular(32, 32);
            var randVelocity = -Vector2.UnitY * 64;
            randVelocity = randVelocity.RotatedByRandom(0.25f);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = randPos,
                velocity = randVelocity,
                innerColor = Color.White.ToVector4(),
                outerColor = Main.DiscoColor.ToVector4(),
                timeLeft = Main.rand.Next(30, 60),
                scale = new Vector2(Main.rand.NextFloat(0.3f, 1f))
            });

        
            randPos = inverseWaveOrigin + Main.rand.NextVector2Circular(32, 32);
            randVelocity = -Vector2.UnitY * 64;
            randVelocity = randVelocity.RotatedByRandom(0.25f);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = randPos,
                velocity = randVelocity,
                innerColor = Color.White.ToVector4(),
                outerColor = Main.DiscoColor.ToVector4(),
                timeLeft = Main.rand.Next(30, 60),
                scale = new Vector2(Main.rand.NextFloat(0.3f, 1f))
            });
        }

        var numWaveDatas = 64;
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
            var influence = WaveInteractDistanceSquared / closest;
            waveData.time = MathF.Sqrt(influence);
            Waves.Add(waveData);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var ditherTex = AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset;
        var pass = AssetReferences.Effects.Niivi.PrismWave.CreatePixelPass();
        pass.Parameters.spriteSize = Projectile.Drawer.Size;
        pass.Parameters.ditherTexelSize = ditherTex.GetTexelSize();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.ColorSpectrumTexture = PaletteAssets.FromPaletteFile("Prismatic").Value.ColorAtlas;
        pass.Apply();
        var gDevice = Main.graphics.graphicsDevice;

        gDevice.Textures[1] = AssetReferences.Assets.NoiseTextures.IceWaterCaustics.Asset.Value;
        gDevice.SamplerStates[1] = SamplerState.LinearWrap;


        gDevice.Textures[2] = AssetReferences.Assets.GlowMasks.PrismaticColorMap.Asset.Value;
        gDevice.SamplerStates[2] = SamplerState.LinearClamp;


        gDevice.Textures[3] = ditherTex.Value;
        gDevice.SamplerStates[3] = SamplerState.LinearWrap;


        using (Main.spriteBatch.Ctx(Main.spriteBatch.Parameters with { effect = pass.Shader, blendState = BlendState.AlphaBlend }))
        {
            for (var i = 0; i < Waves.Count; i++)
            {
                var wave = Waves[i];
                var drawer = Projectile.Drawer;
                drawer.BottomCenterOrigin();
                drawer.worldPosition = wave.position;
                byte t = (byte)(wave.time * 255F);
                byte o = (byte)(((float)i / (float)Waves.Count) * 255F);

                drawer.color = Color.White;
                drawer.color.R = o;
                drawer.color.A = t;


                drawer.scale.Y *= MathHelper.Lerp(0.5F, 2f, EasingFunction.InOutSine(wave.time));
      
                Main.spriteBatch.Draw(drawer);

                //  Main.spriteBatch.Draw(drawer);
            }
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
        Projectile.SetTrailCacheLength(4);
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
        Timer++;
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
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
        drawer2.scale *= 1.2f;
        Main.spriteBatch.Draw(drawer2);

        foreach(OldPosition oldPos in Projectile.IterateOldPosBackwards())
        {
            drawer2.worldPosition = oldPos.position + Projectile.Size * 0.5f;
            drawer2.color = Main.DiscoColor * 0.25f * oldPos.progress;
            drawer2.color.A = 0;
            Main.spriteBatch.Draw(drawer2);
        }
        var rotateRadians = MathHelper.PiOver2 * MathF.Sign(Projectile.velocity.X);
        var endPos = _initialPosition.RotatedBy(rotateRadians, Origin);
        drawer2.worldPosition = endPos;
        drawer2.rotation = 0;
        drawer2.color = Main.DiscoColor * 0.25f * ExtraMath.Osc(0.5f, 1f, speed: 16, endPos.X * 0.1f);
        drawer2.color.A = 0;
        Main.spriteBatch.Draw(drawer2);

        var drawer = Projectile.Drawer;
        drawer.worldPosition = drawPosition;
        drawer.color = Color.White;
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var sound = AssetReferences.Assets.Sounds.Melee.HammerSmash1.Asset with { PitchVariance = 0.7f, MaxInstances = 0 };
        SoundEngine.PlaySound(sound, Projectile.position);

        FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
        for(var i= 0; i < 24; i++)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(128, 16);
            var vel = -Vector2.UnitY * Main.rand.NextFloat(8, 16);
            Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.White.ToVector4(),
                outerColor = Main.DiscoColor.ToVector4(),
                timeLeft = Main.rand.Next(90, 300),
                scale = new Vector2(Main.rand.NextFloat(1f, 3f))
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