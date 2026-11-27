using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB.Gores;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.NPCsWD;

public static class DarkspaceEnemyCommon
{
    public static void HitAndDeathEffectsMini(NPC NPC)
    {
        float numDust = 3;
        for (float n = 0; n < numDust; n++)
        {
            var factory = ParticleUtils.ParticleFactory.FromSmallBurst(NPC.Center, TriColorPalette.Moonly, new Vector2(4, 10));
            ParticleUtils.CreateSwirlingDustCircle(factory);
            
        }
        if (NPC.life <= 0)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                for(var i = 0; i < 8; i++)
                {
                    var pos = NPC.Center;
                    var vel = MoonUtils.Rand.NextVector2CircularEdge(12, 12);
                    PS.GlowDust.Spawn(new()
                    {
                        position = pos,
                        velocity = vel,
                        timeLeft = 120,
                        scale = MoonUtils.Rand.NextFloat(0.15f, 0.25F),
                        color = Color.SkyBlue
                    });
                }

                var sound = AssetReferences.Assets.Sounds.NiiviWingFlap.Asset with { Pitch = 0.5f, PitchVariance = 0.3f };
                SoundEngine.PlaySound(sound, NPC.Center);
            }
        }
    }
}
