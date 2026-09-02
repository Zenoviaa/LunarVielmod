using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB.Gores;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;

public static class AbyssEnemyCommon
{
    public static void HitAndDeathEffects(NPC NPC)
    {
        float numDust = 3;
        for (float n = 0; n < numDust; n++)
        {
            Vector2 inverseVelocity = -NPC.oldVelocity;
            inverseVelocity = inverseVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f);
            var dp = DustParticle.Spawn(NPC.Center, inverseVelocity);
            dp.dampening = 0.1f;
            dp.Scale *= 0.5f;
            dp.innerColor = Color.White;
            dp.outerColor = Color.SkyBlue;
        }
        if (NPC.life <= 0)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + velocity, velocity * 2, ModContent.GoreType<AbyssFeatherGore>());
                }

                var sound = AssetReferences.Assets.Sounds.NiiviWingFlap.Asset with { Pitch = 0.5f, PitchVariance = 0.3f };
                SoundEngine.PlaySound(sound, NPC.Center);
                for(int i = 0; i < 8; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
                    velocity.Y -= 8;
                    Particles.FeatherDust.Spawn(FeatherDustData.Default with { 
                        position = NPC.Center + Main.rand.NextVector2Circular(16, 16), 
                        velocity = velocity,
                        scale = Main.rand.NextFloat(0.2f, 0.6f) });
                }   

                for (int i = 0; i < 16; i++)
                {
                    EmberParticle ep = LegacyParticle.NewParticle<EmberParticle>(NPC.position +
                        new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(0, NPC.height)),
                        -Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi), Color.White, Main.rand.NextFloat(0.9f, 1.5f));
                    ep.innerColor = Color.White;
                    ep.outerColor = Color.SkyBlue;
                }
                for (int i = 0; i < 12; i++)
                {
                    Vector2 spawnPosition = new Vector2();
                    spawnPosition.X = NPC.position.X + Main.rand.Next(0, NPC.width);
                    spawnPosition.Y = NPC.position.Y + Main.rand.Next(0, NPC.height);
                    SparkleParticle.Spawn(spawnPosition, Vector2.Zero, Scale: 0.3f);
                }
            }
        }
    }
}
