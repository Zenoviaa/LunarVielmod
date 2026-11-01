using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Core.Camera;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;


public class NiiviSillyStarBombBoom : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => ModType.NoTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.width = 30;
        Projectile.height = 30;
        Projectile.penetrate = -1;
        Projectile.light = 2f;
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
        if(Timer == 1)
        {
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, DrawUtilities.Prismatic, DrawUtilities.Prismatic, DrawUtilities.Prismatic);
            fx.Scale *= 2;

            PixelPrimitiveCircleFactory.CreateGenericInBoom(Projectile.Center, DrawUtilities.Prismatic, DrawUtilities.Prismatic, 24, 777);
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, DrawUtilities.Prismatic, DrawUtilities.Prismatic, 64, 256);

            var starBlast = AssetReferences.Assets.Sounds.NStarblast2.Asset with { Pitch = 0.2f, PitchVariance = 0.2f };
            SoundEngine.PlaySound(starBlast, Projectile.Center);
            //Create a burst of particles
            for (var i = 0; i < 160; i++)
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

            for (var i = 0; i < 16; i++)
            {
                var pos = Projectile.Center + ((float)i / 16F * MathHelper.TwoPi).ToRotationVector2() * 64;
                Particles.DiamondSparkle.Spawn(new()
                {
                    position = pos,
                    timeLeft = 30,
                    color = DrawUtilities.Prismatic
                });
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
        //return base.PreDraw(ref lightColor);
    }
}
public class NiiviSillyStarMaker : ModProjectile
{
    float _numAttacksDone = 0;
    ref float Timer => ref Projectile.ai[0];
    ref float AttackCount => ref Projectile.ai[1];
    ref float GlobalTimer => ref Projectile.ai[2];
    Player TargetPlayer
    {
        get
        {
            Player player = null; 
            if (player == null || !player.active)
            {
                player = PlayerHelper.FindClosestPlayer(Projectile.Center, 4096);
            }
            if(player == null)
            {
                player = Main.LocalPlayer;
            }
            return player;
        }
    }

    public override string Texture => ModType.NoTexture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_numAttacksDone);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _numAttacksDone = reader.ReadSingle();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 120000;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.penetrate = -1;
        Projectile.timeLeft = 1200;
        Projectile.ignoreWater = true;
        Projectile.WidthAndHeight = 60;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
        OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -100);
        Timer++;
        GlobalTimer++;
        if(Timer >= 74)
        {
            if (this.OwnedByLocalClient())
            {
                var rand = Main.rand.Next(3);
                var firer = ProjFirer.From<NiiviSillyStar>(Projectile);
                firer.ai0 = rand;
                firer.ai1 = TargetPlayer.whoAmI;
                firer.position = TargetPlayer.Center + new Vector2(Main.rand.NextFloat(-666, 666), -1000);
                firer.New();
            }
            _numAttacksDone++;
            if(_numAttacksDone >= AttackCount)
            {
                Projectile.Kill();
            }
            Timer = 0;
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

    void DrawGradient(SpriteBatch sb, Vector2 sp)
    {
        var startColor = Color.Red;
        startColor.ScrollHue(Main.GlobalTimeWrappedHourly * 25);
        startColor = Color.Lerp(startColor, Color.White, 0.5F);
        var endColor = startColor;
        endColor.ScrollHue(45);
        var pass = AssetReferences.Effects.CrystalShaders.BetterYGradient.CreatePixelPass();
        pass.Parameters.startColor = startColor.ToVector3();


        pass.Parameters.endColor = endColor.ToVector3();
        pass.Apply();
        var fade = EasingFunction.InOutSine(GlobalTimer / 30f) * EasingFunction.InOutSine((float)Projectile.timeLeft / 30F);
        using (sb.Ctx(sb.Parameters with { effect = pass.Shader, blendState = BlendState.Additive }))
        {
            sb.Draw(
                AssetReferences.Assets.GlowMasks.VerticalGradient.Asset.Value,
                new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                Color.White * fade * 0.7f);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawGradient, DrawLayer.BehindTiles);
 

        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

}
public class NiiviSillyStar : ModProjectile
{
    float _dirX;
    float _squishTimer;
    bool _starWalker;
    float _starWalkerFrameTimer;
    int _starWalkerFrameIndex;
    bool _mad;
    float _deathTimer;
    Vector2 _startPosition;
    Vector2 _endPosition;
    bool _hasCollided;
    enum StarType : byte
    {
        FallignStar,
        RunningStar,
        SpiralStar
    }
    StarType Style
    {
        get => (StarType)Projectile.ai[0];
    }
    Color GlowColor
    {
        get
        {
            switch (Style)
            {
                default:
                case StarType.FallignStar:
                    return Color.DarkOrange;
                case StarType.RunningStar:
                    return Color.DarkRed;
                case StarType.SpiralStar:
                    return Color.DarkGreen;
            }
        }
    }
    float SquishTime => 30F;
    Vector2 Scale => Vector2.One  *Vector2.Lerp(Vector2.One, new Vector2(1.1f, 0.9f), EasingFunction.OutSine(_squishTimer / SquishTime));
    Player TargetPlayer => Main.player[(int)Projectile.ai[1]];
    ref float Timer => ref Projectile.ai[2];
    float StarWalkerFrameSpeed => 4f;
    int StarWalkerFrameCount => 4;
    float RunningStarDeathTime => 140F;
    float SpiralTime => 190;
    float SpiralSize => 384;
    float SpiralDashTime => 90F;
    public override string Texture => AssetReferences.Assets.Textures.Illuria.NPCs.NiiviSillyStar.KEY;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_hasCollided);
        writer.Write(_deathTimer);
        writer.WriteVector2(_startPosition);
        writer.WriteVector2(_endPosition);
        writer.Write(_dirX);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _hasCollided = reader.ReadBoolean();
        _deathTimer = reader.ReadSingle();
        _startPosition = reader.ReadVector2();
        _endPosition = reader.ReadVector2();
        _dirX = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 9;
        Projectile.SetTrailCacheLength(8);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.penetrate = -1;
        Projectile.light = 1f;
        Projectile.timeLeft = 1200;
        Projectile.hostile = true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if(Style == StarType.RunningStar)
        {
            if (!_hasCollided)
            {
                _squishTimer = 30f;
                Blast();
                Projectile.velocity.Y = -7;
                var dir = (TargetPlayer.Center.X < Projectile.Center.X) ? -1 : 1;
                Projectile.velocity.X = dir;
                _dirX = dir;
                _hasCollided = true;
            }
            else
            {
                Projectile.velocity.X = oldVelocity.X;
                Projectile.velocity.Y = 0;
            }
        }

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
            innerColor = GlowColor.ToVector4(),
            outerColor = Color.Lerp(GlowColor, Color.Black, 0.5F).ToVector4(),
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
            outerColor = Color.Lerp(GlowColor, Color.Black, 0.5F).ToVector4(),
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
                color = GlowColor,
                timeLeft = 24,
            });
        }
        if (Main.rand.NextBool(32))
        {
            pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            vel = Main.rand.NextVector2Circular(1, 1);
            var sp = SparkleParticle.Spawn(pos, vel);
            sp.innerColor = Color.White;
            sp.outerColor = GlowColor;
            sp.gravity = 0;
            //sp.flickering = true;
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
                color = GlowColor,
            });

        }
    }
    public override void AI()
    {
        base.AI();
        switch (Style)
        {
            case StarType.FallignStar:
                {
                    AI_FallingStar();
                }
                break;
            case StarType.RunningStar:
                {
                    AI_RunningStar();
                }
                break;
            case StarType.SpiralStar:
                {
                    AI_SpiralStar();
                }
                break;
        }

        MakeDusts();
        if(_squishTimer > 0)
        {
            _squishTimer--;
        }

    }

    public override void PostAI()
    {
        base.PostAI();
        if (_starWalker)
        {
            _starWalkerFrameTimer++;
            if(_starWalkerFrameTimer >= StarWalkerFrameSpeed)
            {
                _starWalkerFrameTimer = 0;
                _starWalkerFrameIndex++;
                _starWalkerFrameIndex %= StarWalkerFrameCount;
            }
        }
    }
    
    void DeathFX()
    {
        var currentPosition = Projectile.Center;
        var starSingle = AssetReferences.Assets.Sounds.Starrer.Asset with { PitchVariance = 0.75f, Pitch = -0.8f };
        SoundEngine.PlaySound(starSingle, Projectile.position);

        var pos = Main.rand.NextVector2Circular(7, 7) + currentPosition;
        var color = DrawUtilities.Prismatic.ToVector4();
        for (var i = 0; i < 16; i++)
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

    void Blast()
    {
        var starSingle = AssetReferences.Assets.Sounds.Starrer.Asset with { PitchVariance = 0.75f };
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

    void AI_FallingStar()
    {
        Projectile.tileCollide = false;
        Timer++;
        if(Timer == 1)
        {
            Projectile.velocity.X = -4;
            Projectile.velocity.Y = 8;
        }
        if(Timer == 100)
        {
            Flicker();
        
        }
        

        if(Timer > 100 && Timer < 120)
        {
            Projectile.velocity *= 0.97f;
        }



        if(Timer >= 100)
        {
            _mad = true;
            if(Projectile.velocity.Length() < 15)
                Projectile.velocity *= 1.02f;
            Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, TargetPlayer.Center, 3F);
            Projectile.rotation += MathF.Sign(Projectile.velocity.X) * 0.04f;
        }

        if(Timer >= 170)
        {
            Projectile.Kill();
        }
    }

    void AI_RunningStar()
    {
        Timer++;
        if(Timer == 1)
        {
            Projectile.velocity.X = 0;
            Projectile.velocity.Y = 8;
        }

        if (!_hasCollided)
        {
            if(Projectile.velocity.Length() < 16f)
                Projectile.velocity.Y *= 1.01f;
            Projectile.rotation += 0.04f;
        }
        else
        {
            _starWalker = true;
            _mad = true;
            Projectile.direction = (int)_dirX;
            Projectile.spriteDirection = (int)_dirX;
            Projectile.velocity.X = _dirX * MathHelper.Lerp(1f, 9F, _deathTimer / RunningStarDeathTime);
            Projectile.velocity.Y += 0.6f;

            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, 0, 0.1f);
            _deathTimer++;
            if (_deathTimer >= RunningStarDeathTime)
                Projectile.Kill();
        }
    }

    void AI_SpiralStar()
    {
        Timer++;

        if(Timer < SpiralTime)
        {
            var size = MathHelper.Lerp(1f, 0.6f, Timer / SpiralTime);
            size *= SpiralSize;
            var upOffset = -Vector2.UnitY * size;
            upOffset = upOffset.RotatedBy(MathHelper.TwoPi * EasingFunction.InOutExpo(Timer / SpiralTime));
            var upPos = TargetPlayer.Center + upOffset;
            Projectile.Center = upPos;
            Projectile.rotation += 0.04F;
        }
        else if (Timer == SpiralTime)
        {
            Flicker();
            _startPosition = Projectile.Center;
            _endPosition = TargetPlayer.Center;
        }
        else
        {
            _mad = true;
            var t = Timer - SpiralTime;
            var ratio = t / SpiralDashTime;
            var ease = EasingFunction.InOutBack(ratio);
            var posToMoveTo = Vector2.Lerp(_startPosition, _endPosition, ease);
            Projectile.Center = posToMoveTo;
            var dir = (_endPosition.X < _startPosition.X) ? -1 : 1;
            Projectile.rotation += dir * 0.04f;
            if(t >= SpiralDashTime)
            {
                Projectile.Kill();
            }
        }
    }

    public override bool ShouldUpdatePosition()
    {
        return base.ShouldUpdatePosition();
    }
    
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }
    
    void PixelGlow(SpriteBatch sb, Vector2 sp)
    {
        var glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, Projectile.Center);
        glowDrawer.color = GlowColor * 0.6F;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.24f;
        sb.Draw(glowDrawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {


        var frame = (int)((int)Style * 3);
        var outlineFrame = frame + 2;
        if (_mad)
            frame++;
        var drawer = Projectile.Drawer;
        drawer.scale *= Scale;
        drawer.VerticalFrame(frame, Main.projFrames[Type]);
        drawer.CenterOrigin();
        if (_starWalker)
            drawer.worldPosition.Y -= 8;

        var glowDrawer = drawer;
        foreach(var offset in Vector2.CardinalOffsets)
        {
            var o = offset.RotatedBy(Main.GlobalTimeWrappedHourly * 5) * 8;
            glowDrawer.worldPosition = Projectile.Center + o;
            glowDrawer.scale = Vector2.One * ExtraMath.Osc(0.6f, 1f, speed: 2, offset: Projectile.identity);
            glowDrawer.color = GlowColor * 0.4f;
            glowDrawer.color.A = 0;
            Main.spriteBatch.Draw(glowDrawer);

        }

        var afDrawer = Projectile.Drawer;
        afDrawer.VerticalFrame(frame, Main.projFrames[Type]);
        afDrawer.CenterOrigin();
        foreach (OldPosition oldPos in Projectile.IterateOldPosBackwards())
        {
            afDrawer.worldPosition = oldPos.position + Projectile.Size * 0.5f;
            afDrawer.color = Color.Lerp(Color.White, Color.Transparent, oldPos.progress) * 0.1f;
            afDrawer.color.A = 0;
            afDrawer.rotation = Projectile.oldRot[oldPos.index];
            Main.spriteBatch.Draw(afDrawer);
        }

        if (_starWalker)
        {
            var walkerDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.Illuria.NPCs.StarWalker.Asset, Projectile.Center);
            walkerDrawer.VerticalFrame(_starWalkerFrameIndex, StarWalkerFrameCount);
            walkerDrawer.TopCenterOrigin();
            walkerDrawer.scale *= Scale;
            walkerDrawer.color = Color.Black;
            walkerDrawer.spriteEffects = _dirX == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(walkerDrawer);
        }


        Main.spriteBatch.Draw(drawer);

        drawer.color = Color.White * ExtraMath.Osc(0.02f, 0.2f, speed: 2, offset: Projectile.identity);
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);


        drawer.color = Color.Red;
        drawer.VerticalFrame(outlineFrame, Main.projFrames[Type]);
      //  Main.spriteBatch.Draw(drawer);

        PixelationManager.QueueSpritebatchDrawAction(PixelGlow);
        return false;
    }
    
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        DeathFX();
    }
}

public class NiiviStarBomb : ModProjectile
{
    float _sizeDown;
    float _globalTimer;
    float _speed;
    int _oldIndex;
    float KaboomTime => 60;
    ref float Timer => ref Projectile.ai[0];
    ref float ChargeLevel => ref Projectile.ai[1];
    ref float ChargeTime => ref Projectile.ai[2];
    float MaxChargeLevel => 64;
    float Size => MathHelper.Lerp(24, 128 + _globalTimer * 0.1f, EasingFunction.OutExpo(_globalTimer / 360F)) * ExtraMath.Osc(0.88f, 1f, speed: 6) * _sizeDown;
    float DistanceBetweenSteps => 1f;
    float DistanceBetweenCircleSteps => 7;
    int[] PentagonPattern => field ??= [3, 0, 2, 4, 1];
    Vector2[] PentagonPoints
    {
        get
        {
            field ??= new Vector2[5];
            for (var i = 0; i < field.Length; i++)
            {
                ref var point = ref field[i];
                var progress = (float)i / (float)field.Length;
                var radians = progress * MathHelper.TwoPi;
                point = -Vector2.UnitY.RotatedBy(radians + _globalTimer * 0.025F);
                point *= Size;
                point += Projectile.Center;

            }
            return field;
        }
    }

    int GetIndex(float time)
    {
        float t = time * 0.02f;
        t %= 1.00F;

        int currentIndex = (int)(t * PentagonPattern.Length) % PentagonPattern.Length;
        return currentIndex;
    }

    Vector2 InterpolateRingPattern(float time, float size)
    {
        return Projectile.Center + -Vector2.UnitY.RotatedBy(time * 0.02f) * size;
    }
    Vector2 InterpolatePentagonPattern(float time)
    {
        float t = time * 0.02f;
        t %= 1.00F;

        int currentIndex = (int)(t * PentagonPattern.Length) % PentagonPattern.Length;
        int nextIndex = (currentIndex + 1) % PentagonPattern.Length;

        float stepSize = (1F) / (float)PentagonPattern.Length;
        float bottom = currentIndex * stepSize;
        float ratio = (t - bottom) / stepSize;

        return Vector2.Lerp(PentagonPoints[PentagonPattern[currentIndex]], PentagonPoints[PentagonPattern[nextIndex]], ratio);
    }

    public override string Texture => ModType.NoTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.timeLeft = 900;
        Projectile.hostile = true;
        Projectile.light = 2f;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
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

        if (Timer % 3 == 0)
        {
            var pos = GetStretchSpawnPoint();
            var vel = Projectile.Center - pos;
            vel *= 0.06f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = DrawUtilities.Prismatic;
            fx.InnerColor = DrawUtilities.Prismatic;
            fx.GlowColor = DrawUtilities.Prismatic;
            fx.Scale *= 2 * Main.rand.NextFloat(1f, 2f);
        }
    }
    public override void AI()
    {
        base.AI();
        _speed += 0.003f;
        _globalTimer += _speed;
     
        if (ChargeLevel < MaxChargeLevel)
        {
            _sizeDown = 1f;
            Timer++;
            if (Timer >= ChargeTime)
            {
                Timer = 0;
                ChargeTime *= 0.94f;
                ChargeLevel++;
            }
        }
        else
        {

            _sizeDown = MathHelper.Lerp(1f, 0f, EasingFunction.InSine(Timer / KaboomTime));
            Timer++;
            if (Timer >= KaboomTime)
            {
                Projectile.Kill();
            }
        }
        var currentPosition = InterpolatePentagonPattern(_globalTimer);
        var currentIndex = GetIndex(_globalTimer);
        if(_oldIndex != currentIndex)
        {
            _oldIndex = currentIndex;
            FXUtil.GlowCircleBoom(currentPosition, DrawUtilities.Prismatic, DrawUtilities.Prismatic, Color.Black, 24);
            var pos = Main.rand.NextVector2Circular(7, 7) + currentPosition;
            var color = DrawUtilities.Prismatic.ToVector4();
            for (var i =0; i < 12; i++)
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
        AmbientSuckingParticles();

        for(var i = 0; i < 3; i++)
        {
            var pos = Main.rand.NextVector2Circular(7, 7) + currentPosition;
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = Main.rand.NextVector2Circular(3, 3),
                timeLeft = 30,
                innerColor = Vector4.Lerp(DrawUtilities.Prismatic.ToVector4(), Vector4.One, 0.65f),
                outerColor = DrawUtilities.Prismatic.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.3f, 0.8f))
            });
        }
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    Color GetTrailColor(float ratio)
    {
        var color = Color.Red;
        color.ScrollHue(Main.GlobalTimeWrappedHourly * 512 + ratio * 255);
        var myColor = Color.Lerp(Color.White, color, ratio);
        return Color.White;
    }
    Color GetTrailColorFaint(float ratio)
    {
        return GetTrailColor(ratio) * 0.4F;
    }
    float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(12, 0, ratio);
    }
    float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 3f;
    }

    void DrawPentagonTrail(GraphicsDevice gDevice)
    {
        var points = new List<Vector2>();
        points.Add(InterpolatePentagonPattern(_globalTimer));
        for (var i = 1; i < ChargeLevel * 2; i++)
        {
            points.Add(InterpolatePentagonPattern(_globalTimer - i * DistanceBetweenSteps));

        }
        var starPoints = points.ToArray();

        BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
        bloomTrailShader.InnerColor = Main.DiscoColor;
        bloomTrailShader.OuterColor =Main.DiscoColor;
 
        TrailDrawer.ClearPrimitives();
        TrailDrawer.PreparePrimitives(starPoints, GetTrailColorFaint, GetTrailWidth2);
        for (var j = 0; j < 3; j++)
        {
            points.Clear();
            for (var i = 0; i < 32; i++)
            {
                var size = 128F + j * 80F;
                var t = EasingFunction.InOutExpo((_globalTimer - (j + 1) * 60F) / 60F);
                size *= t;
                size *= _sizeDown;
                points.Add(InterpolateRingPattern(_globalTimer * (4 + j) - i * DistanceBetweenCircleSteps + j * 4, size));
            }
            TrailDrawer.PreparePrimitives(points.ToArray(), GetTrailColorFaint, GetTrailWidth2);
        }

        TrailDrawer.DrawCached(bloomTrailShader);



        var pass = AssetReferences.Effects.Niivi.StarBombTrail.CreatePrimitivesPass();
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.distortionStrength = 0.03f;
        pass.Apply();
        gDevice.Register(0, AssetReferences.Assets.LaserTextures.TexturedLaser.Asset.Value, SamplerState.LinearWrap);
        gDevice.Register(1, AssetReferences.Assets.Noise.PerlinBlurred.Asset.Value, SamplerState.LinearWrap);


        TrailDrawer.ClearPrimitives();
        TrailDrawer.PreparePrimitives(starPoints, GetTrailColor, GetTrailWidth);
        for(var j = 0; j < 3; j++)
        {
            points.Clear();
            for (var i = 0; i < 32; i++)
            {
                var size = 128F + j * 80F;
                var t = EasingFunction.InOutExpo((_globalTimer - (j+1) * 60F) / 60F);
                size *= t;
                size *= _sizeDown;
                points.Add(InterpolateRingPattern(_globalTimer * (4 + j) - i * DistanceBetweenCircleSteps + j * 4, size));
            }
            TrailDrawer.PreparePrimitives(points.ToArray(), GetTrailColor, GetTrailWidth);
        }

        TrailDrawer.DrawCached(pass.Shader);




 
    }

    void DrawPixelated(SpriteBatch sb, Vector2 sp)
    {
        var lerp = _sizeDown * MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(_globalTimer / 60f));;
        var glow = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, Projectile.Center);
        glow.color = Main.DiscoColor * 0.2f * lerp;
        glow.color.A = 0;
        glow.scale *= 2f;
        Main.spriteBatch.Draw(glow);

        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.FivePointedStar.Asset, InterpolatePentagonPattern(_globalTimer));
        drawer.color = Color.White;
        drawer.color.A = 0;
        drawer.scale *= 0.15f * lerp; 
        Main.spriteBatch.Draw(drawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPentagonTrail);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelated, DrawLayer.OverNPCs);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            var firer = ProjFirer.From<NiiviSillyStarBombBoom>(Projectile);
            firer.New();

            var firer2 = ProjFirer.From<NiiviSillyStarMaker>(Projectile);
            firer2.ai1 = 24;
            firer2.New();
        }
    }
}
