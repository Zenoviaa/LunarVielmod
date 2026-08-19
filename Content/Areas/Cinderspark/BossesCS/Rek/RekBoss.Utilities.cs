using Stellamod.Common.Particles;
using Stellamod.Visual.Particles;
using Terraria;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private void CreateFlameSuckParticles(Vector2 position)
    {
        Vector2 spawnPos = position + Main.rand.NextVector2CircularEdge(444, 444);
        Vector2 spawnVelocity = position - spawnPos;
        spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
        spawnVelocity *= 16;
        /*
        var d = DustParticle.Spawn(spawnPos, spawnVelocity, DustParticleSpawnParams.Default);
        d.Scale *= 0.8f;
        d.gravity = 0;*/
        Particles.BitDust.Spawn(BitDustFactory.Default with { position = spawnPos, velocity = spawnVelocity, timeLeft = 24 });
        if (Main.rand.NextBool(2))
        {
            spawnPos = position + Main.rand.NextVector2CircularEdge(384, 384);
            spawnVelocity = position - spawnPos;
            spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
            spawnVelocity *= 16;
            var p = FXUtil.GlowStretch(spawnPos, spawnVelocity);
            p.InnerColor = Color.White;
            p.OuterGlowColor = Color.Red;
        }
    }
    /// <summary>
    /// Find the left side of the arena
    /// </summary>
    /// <returns></returns>
    private Vector2 FindEruptionLeft()
    {
        Point centerTile = _arenaCenter.ToTileCoordinates();
        for (int i = 0; i < 200; i++)
        {
            centerTile.Y++;

            if (WorldGen.SolidTile(centerTile))
            {
                centerTile.Y -= 1;
                break;
            }

        }
        for (int i = 0; i < 200; i++)
        {
            centerTile.X--;

            if (WorldGen.SolidTile(centerTile))
            {
                centerTile.X += 1;
                break;
            }

        }
        return centerTile.ToWorldCoordinates();
    }

    /// <summary>
    /// Find the right side of the arena
    /// </summary>
    /// <returns></returns>
    private Vector2 FindEruptionRight()
    {
        Point centerTile = _arenaCenter.ToTileCoordinates();
        for (int i = 0; i < 200; i++)
        {
            centerTile.Y++;

            if (WorldGen.SolidTile(centerTile))
            {
                centerTile.Y -= 1;
                break;
            }

        }
        for (int i = 0; i < 200; i++)
        {
            centerTile.X++;

            if (WorldGen.SolidTile(centerTile))
            {
                centerTile.X--;
                break;
            }

        }
        return centerTile.ToWorldCoordinates();
    }
}
