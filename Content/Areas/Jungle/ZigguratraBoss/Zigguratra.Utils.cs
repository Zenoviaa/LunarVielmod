using Stellamod.Common.Particles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss;

public partial class Zigguratra
{
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
    Color LightGoldenColor => Color.Lerp(Color.Gold, Color.Black, 0.5f);
    Color DarkGoldenColor => Color.Lerp(Color.DarkGoldenrod, Color.Black, 0.5f);

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

    void AmbientLightningParticles(Vector2 centerPos)
    {
        if (!Main.rand.NextBool(32))
            return;
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

        var zapSound = AssetReferences.Assets.Sounds.Dreadmire_LightingRain.Asset with { PitchVariance = 0.6f };
        zapSound.Volume = 0.5f;
        SoundEngine.PlaySound(zapSound, centerPos);
        var darkColor = Color.Lerp(color, Color.Aquamarine, 0.25f);
        FXUtil.GlowCircleBoom(pos, Color.Gold, Color.DarkGoldenrod, Color.Aquamarine, 25, baseSize: 0.16f);
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
