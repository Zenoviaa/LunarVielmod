using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Content.Areas.TheFalling.PerfectSingularityBoss;

public partial class PerfectSingularity
{
    private void FlareUp(float time, float intensity)
    {

    }

    private void PlayChainwhipSound()
    {

    }
    private void PlayIntensifySound()
    {
        FXUtil.CreateRipple(NPC.Center);
    }
    private void EmitIntensityParticles(int rate)
    {
        //Periodically emites particles to show things are getting intense fr fr
        if(Timer % rate == 0)
        {
            float range = Main.rand.NextFloat(300, 400);
            Vector2 spawnPos = NPC.Center + Main.rand.NextVector2CircularEdge(range, range);
            Vector2 vel = (spawnPos - NPC.Center);
            vel = vel.SafeNormalize(Vector2.Zero);
            vel *= Main.rand.NextFloat(3f, 6f) * 6;
            var dust = DustParticle.Spawn(spawnPos, vel); ;
            dust.outerColor = Color.DarkGray;
            dust.Scale *= 1f;
            dust.noTileCollide = true;
            dust.gravity = 0;
            dust.dampening = 0.05f;

            if (Main.rand.NextBool(2))
            {
                spawnPos = NPC.Center + Main.rand.NextVector2CircularEdge(range, range);
                vel = (spawnPos - NPC.Center);
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(3f, 6f) * 6;

                var sparkle = SparkleParticle.Spawn(spawnPos, vel);
                sparkle.dampening = 0.05f;
                sparkle.gravity = 0;
                sparkle.noTileCollide = true;
                sparkle.outerColor = Color.DarkGray;
                sparkle.Scale *= 0.5f;
            }
        }


    }

    private void Recoil(Vector2 recoilStrength)
    {

    }

    private void Intensify(float time, float intensity)
    {
        _intensityTimeLeft = MathF.Max(_intensityTimeLeft, time);
        _intensity = MathF.Max(_intensity, intensity);
    }
}
