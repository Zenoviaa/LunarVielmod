using Stellamod.Common.Particles;
using System;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi
{
    static Color LightGoldenColor => Color.Lerp(Color.Gold, Color.Black, 0.5f);
    static Color DarkGoldenColor => Color.Lerp(Color.DarkGoldenrod, Color.Black, 0.5f);
    Vector2 RandomPositionOnBody => SegmentPositions[Main.rand.Next(SegmentPositions.Length)];
    float _crystalRotationRadians;
    void RotateCrystals(Vector2 point, float distanceOffset)
    {
        var offset = -Vector2.UnitY.RotatedBy(_crystalRotationRadians) * distanceOffset;
        _lightningCrystalPosition = point + offset;

        offset = offset.RotatedBy(MathHelper.TwoPi / 3f);
        _iceCrystalPosition = point + offset;

        offset = offset.RotatedBy(MathHelper.TwoPi / 3f);
        _starCrystalPosition = point + offset;

    }
    void AmbientSuckingParticles()
    {
        //The thing zemmie likes is when there's just a otn of these
        //Let's mix the bit dust and with the stretch particles and a few swirly particles, that should make it look very dynamic.
        var suckPosition = NPC.Center + NPC.rotation.ToRotationVector2() * 48;

        //First stretch particles
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Vector2 GetStretchSpawnPoint()
        {
            var edge = Main.rand.NextFloat(444, 666);
            return suckPosition + Main.rand.NextVector2CircularEdge(edge, edge);
        }


        if (Timer % 15 == 0)
        {
            var fx = FXUtil.GlowCircleBoom(suckPosition, DrawUtilities.Prismatic, DrawUtilities.Prismatic, Color.Black, 30, 0.2F);
            fx.Scale *= 1.5F;
        }
        if (Timer % 2 == 0)
        {
            var pos = GetStretchSpawnPoint();
            var vel = suckPosition - pos;
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = DrawUtilities.Prismatic;
            fx.InnerColor = DrawUtilities.Prismatic;
            fx.GlowColor = DrawUtilities.Prismatic;
            fx.Scale *= 3 * Main.rand.NextFloat(1f, 2f);
        }

        {
            var pos = GetStretchSpawnPoint();
            var vel = suckPosition - pos;
            vel *= 0.1f;
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
            var pos = suckPosition;
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
    void Teleport(Vector2 teleportToPos)
    {
        if (MultiplayerHelper.IsHost)
        {
            _teleportPosition = teleportToPos;
            NPC.netUpdate = true;
        }
    }

    /// <summary>
    /// Makes the ik target just follow the head until it's within range
    /// </summary>
    void LimpIKTarget()
    {
        var dist = Vector2.Distance(_ikTarget, NPC.Center);
        var rigDist = FabrikSolver.RigLength(Segments);
        var iterations = 0;
        while (dist > rigDist)
        {

            //JUST IN CASE
            //Not sure how this would happen tho
            iterations++;
            if (iterations >= 128)
                break;
            var dir = (NPC.Center - _ikTarget).SafeNormalize(Vector2.Zero);
            _ikTarget += dir;
            dist = Vector2.Distance(_ikTarget, NPC.Center);
        }
    }
    Vector2 ArcSwoopDLoop(float t, Vector2 origin, float rightD, float downD)
    {
        var left = origin;
        var right = origin + new Vector2(rightD, 0);
        right.Y += MathHelper.Lerp(0, downD, t * t);
        //right.RotateAround(origin, t * MathHelper.TwoPi * 2);
        var ltr = Vector2.Lerp(left, right, t);
        return ltr;

    }
    Vector2 ArcSwoop(float t, Vector2 origin, float down = 2000, float radius = 1500)
    {
        var left = origin + new Vector2(-radius, 0);
        var right = origin + new Vector2(radius, 0);
        var leftToRight = Vector2.Lerp(left, right, t);
        var topToBottom = MathHelper.Lerp(0, down, MathF.Sin(t * 6.28F * 0.5f) * 0.5f + 0.5f);
        topToBottom += MathHelper.Lerp(0, 64, MathF.Sin(t * 5 * 4f) * 0.5f + 0.5f);
        var lerp = leftToRight + new Vector2(0, topToBottom);
        return lerp;
    }
    Vector2 ArcSwoopUp(float t, Vector2 origin, float rightD, float upD)
    {
        var left = origin;
        var right = origin + new Vector2(rightD, 0);
        var leftToRight = Vector2.Lerp(left, right, t);
        leftToRight.Y += MathHelper.Lerp(0, upD, t * t);
        return leftToRight;
    }
    Vector2 ArcSwoopDown(float t, Vector2 origin, float rightD, float upD)
    {
        var left = origin;
        var right = origin + new Vector2(rightD, 0);
        var leftToRight = Vector2.Lerp(left, right, t);
        leftToRight.Y += MathHelper.Lerp(0, upD, (t * t));
        return leftToRight;

    }
    //Ok
    //so the way we're gonna do this boss is by making TONS OF HECKING CURVES
    //Every motion will be preset just with calcualted points
    //all the ik points will be calculated too except for a few
    //guh
    Vector2 ArcRightFallDownLeft(float t, Vector2 origin)
    {
        var width = 474;
        var height = 1222;
        var leftToRight = Vector2.Lerp(origin, origin + new Vector2(width, 0), t / 0.4f);
        var bottomToTop = Vector2.Lerp(origin + new Vector2(width, 0), origin + new Vector2(0, -height), t / 0.4f);
        var topToBottomLeft = Vector2.Lerp(origin + new Vector2(0, -height), origin + new Vector2(-512, 2000), t / 1f);

        var lerp1 = Vector2.Lerp(leftToRight, bottomToTop, t / 0.4f);
        var lerp2 = Vector2.Lerp(bottomToTop, topToBottomLeft, t);
        var lerp3 = Vector2.Lerp(lerp1, lerp2, t);
        return lerp3;
    }
    
    public static void PlayLightningSound(Vector2 position)
    {

        var zapSound = AssetReferences.Assets.Sounds.Dreadmire_LightingRain1.Asset with { PitchVariance = 1 };
        if (Main.rand.NextBool(2))
            zapSound = AssetReferences.Assets.Sounds.Dreadmire_LightingRain2.Asset with { PitchVariance = 1 };
        if (Main.rand.NextBool(2))
            zapSound = AssetReferences.Assets.Sounds.Dreadmire_LightingRain3.Asset with { PitchVariance = 1 };
        zapSound.Volume = 0.5f;
        SoundEngine.PlaySound(zapSound, position);
    }

    public static void MakeRainbowLightningSpark(Vector2 centerPos)
    {
        var pos = centerPos + Main.rand.NextVector2Circular(32, 32);
        var vel = -Vector2.UnitY;
        vel *= 6;
        vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));

        var lightPrismaticColor = DrawUtilities.Prismatic.Lighten(0.3f);
        var darkPrismaticColor = lightPrismaticColor.Darken(0.5F);
        var color = Color.Lerp(lightPrismaticColor, darkPrismaticColor, Main.rand.NextFloat(0f, 1f));
        color = Color.Lerp(color, Color.White, 0.6f);
        Particles.LightningBolt.Spawn(new()
        {
            position = pos - vel.SafeNormalize(Vector2.Zero) * 24,
            velocity = vel,
            color = color,
            timeLeft = 60
        });

        PlayLightningSound(centerPos);
        var darkColor = Color.Lerp(color, Color.Black, 0.25f);
        FXUtil.GlowCircleBoom(pos, lightPrismaticColor, darkPrismaticColor, Color.Black, 25, baseSize: 0.16f);
        for (var f = 0; f < 16; f++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos + Main.rand.NextVector2Circular(32, 32),
                velocity = Main.rand.NextVector2Circular(16, 16),
                innerColor = color.ToVector4(),
                outerColor = darkColor.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.6f, 1.2f)),
                timeLeft = 60
            });
        }
    }

    public static void CreateLightningSparks(Vector2 centerPos)
    {
        var pos = centerPos + Main.rand.NextVector2Circular(32, 32);
        var vel = -Vector2.UnitY;
        vel *= 6;
        vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));

        var color = Color.Lerp(LightGoldenColor, DarkGoldenColor, Main.rand.NextFloat(0f, 1f));
        color = Color.Lerp(color, Color.White, 0.6f);
        Particles.LightningSpark.Spawn(new()
        {
            position = pos,
            velocity = vel,
            color = color,
            scale = Main.rand.NextFloat(0.7f, 1.5f) * 0.3f,
            timeLeft = 60
        });
    }
}
