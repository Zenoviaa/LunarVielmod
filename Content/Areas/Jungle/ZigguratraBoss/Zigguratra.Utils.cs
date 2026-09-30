using Stellamod.Common.Particles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss;

public partial class Zigguratra
{
    Vector2 AxeAboveHeadPosition
    {
        get
        {
            var axePosition = NPC.Center;
            axePosition.X += NPC.direction * 60;
            axePosition.Y = NPC.Top.Y;
            axePosition.Y += 36;

            return axePosition;
        }
    }
    Vector2 AxeBackPosition
    {
        get
        {
            var axePosition = NPC.Center;
            axePosition.X -= NPC.direction * 111;
            axePosition.Y = NPC.Bottom.Y;
            axePosition.Y -= 12;

            return axePosition;
        }
    }

    Vector2 AxeFirstSplittingPosition
    {
        get
        {
            return AxeBackPosition;
        }
    }
    
    Vector2 AxeSecondSplittingPosition
    {
        get
        {
            //TODO, fix this to be aligned
            return AxeBackPosition;
        }
    }
    
    Vector2 AxeThirdSplittingPosition
    {
        get
        {
            return AxeBackPosition;
        }
    }

    Color LightGoldenColor => Color.Lerp(Color.Gold, Color.Black, 0.5f);
    Color DarkGoldenColor => Color.Lerp(Color.DarkGoldenrod, Color.Black, 0.5f);

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

    void Teleport(Vector2 centerPos)
    {
        if (MultiplayerHelper.IsHost)
        {
            _teleportPos = centerPos;
            NPC.netUpdate = true;
        }
    }

    void AmbientThundercloudParticles(Vector2 centerPos)
    {
        if (Timer % 8 != 0)
            return;
        var pos = centerPos + Main.rand.NextVector2Circular(32, 32);
        var sp = SmokeParticle.SpawnInAlphaLayer(pos, Main.rand.NextVector2Circular(1, 1), Scale: Main.rand.NextFloat(0.6f, 1.2f));
        sp.initialColor = Color.Lerp(Color.DarkGray, Color.Black, 0.6f);
        sp.fadeToColor = Color.Black;
        sp.behindLayer = true;
        sp.Scale *= 3;
    }

    void MakeLightningParticle(Vector2 centerPos)
    {
        var pos = centerPos + Main.rand.NextVector2Circular(32, 32);
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

        PlayLightningSound(centerPos);
        var darkColor = Color.Lerp(color, Color.Black, 0.25f);
        FXUtil.GlowCircleBoom(pos, Color.Gold, Color.DarkGoldenrod, Color.Black, 25, baseSize: 0.16f);
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
    void MakeJumpingParticles(Vector2 position)
    {
        if (Timer % 2 != 0)
            return;

        var pos = position + Main.rand.NextVector2Circular(48, 48);
        pos.Y -= 32;
        var sp = SmokeParticle.SpawnInAlphaLayer(pos, Main.rand.NextVector2Circular(1, 1), Scale: Main.rand.NextFloat(0.6f, 1.2f));
        sp.initialColor = Color.Lerp(Color.DarkGray, Color.Black, 0.8f);
        sp.fadeToColor = Color.Black;
        sp.behindLayer = true;
        sp.Scale *= 1.2f;
        sp.fast = true;
    }
    public GlowDonutParticle MakeGoldenDonut(Vector2 position, Vector2 velocity)
    {
        var p = LegacyParticle.NewParticle<GlowDonutParticle>(position, velocity);
        p.innerColor = Color.Gold;
        p.outerColor = Color.DarkOrange;
        p.fadeToColor = Color.Black;
        return p;
    }
    void AmbientLightningParticles(Vector2 centerPos)
    {
        if (!Main.rand.NextBool(32))
            return;
        MakeLightningParticle(centerPos);
    }
    void AmbientElectricParticles(Vector2 centerPos)
    {
        if (!Main.rand.NextBool(8))
            return;
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
    void GruntSound()
    {

    }

    void FocusOnMe()
    {
        CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.25f));
    }

    void SwirlParticlesAround(Vector2 centerPosition)
    {
        if (Timer % 3 == 0)
        {
            var offsetDistance = 80;
            var offset = Main.rand.NextVector2CircularEdge(offsetDistance, offsetDistance);
            var spawnPos = centerPosition + offset;
            var startVelocity = (spawnPos - centerPosition).SafeNormalize(Vector2.Zero) * 6;
            startVelocity = startVelocity.RotatedBy(MathHelper.PiOver2);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = spawnPos,
                innerColor = LightGoldenColor.ToVector4(),
                outerColor = DarkGoldenColor.ToVector4(),
                velocity = startVelocity,
                scale = new Vector2(Main.rand.NextFloat(0.4f, 1f)),
                timeLeft = 120
            });
        }
    }
}
