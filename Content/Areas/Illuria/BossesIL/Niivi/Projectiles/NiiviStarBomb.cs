using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;

public class NiiviSillyStar : ModProjectile
{
    public override string Texture => AssetReferences.Assets.Textures.Illuria.NPCs.NiiviSillyStar.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override bool ShouldUpdatePosition()
    {
        return base.ShouldUpdatePosition();
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
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
    Color GetBloomColor(float ratio)
    {
        var color = Color.Red;
        color.ScrollHue(Main.GlobalTimeWrappedHourly * 512 + ratio * 255);
        var myColor = Color.Lerp(Color.White, color, ratio);
        return myColor;
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
        return GetTrailColor(ratio) * 0.7F;
    }
    float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(5, 0, ratio);
    }
    float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 3f;
    }
    float GetTrailWidth3(float ratio)
    {
        return GetTrailWidth(ratio) * 7f;
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
        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.FivePointedStar.Asset, InterpolatePentagonPattern(_globalTimer));
        drawer.color = Color.White;
        drawer.color.A = 0;
        drawer.scale *= 0.15f;
        Main.spriteBatch.Draw(drawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPentagonTrail);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelated, DrawLayer.OverNPCs);
        return false;
    }
}
