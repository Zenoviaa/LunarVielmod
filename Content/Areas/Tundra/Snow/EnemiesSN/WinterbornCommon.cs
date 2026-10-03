using Stellamod.Common.Particles;
using Stellamod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.Areas.Tundra.Snow.EnemiesSN;

public static class WinterbornCommon
{
    public static void TransformEffect(Vector2 centerPos)
    {
        var transformSound = AssetReferences.Assets.Sounds.StarSheith.Asset with { PitchVariance = 1f, Volume = 0.5f };
        SoundEngine.PlaySound(transformSound, centerPos);
        var fx = FXUtil.GlowCircleBoom(centerPos, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 0.24F, baseSize: 0.16F);
        fx.Scale *= 1.5f;
        for(var i =0; i < 32; i++)
        {
            var pos = centerPos + Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(10, 10);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.SkyBlue.ToVector4(),
                outerColor = Color.DarkBlue.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.4f, 0.8f)),
                timeLeft = 120
            });
        }
        for(var i = 0; i < 4; i++)
        {
            var pos = centerPos + Main.rand.NextVector2Circular(32, 32);
            var vel = (pos - centerPos).SafeNormalize(Vector2.Zero);
            FXUtil.GlowStretch(pos, vel * 6);
        }
    }
}
