using Stellamod.Common.Particles;
using Stellamod.Visual.Particles;
using Terraria;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    public Rectangle ArenaRectangleUpToLava()
    {

        Point center = _arenaCenter.ToTileCoordinates();
        int left = center.X;
        int top = center.Y;
        int bottom = center.Y;
        int right = center.X;


        //Find left
        while (left > 0)
        {
            left--;
            Tile tile = Main.tile[left, center.Y];
            if (WorldGen.SolidTile(tile))
                break;
        }

        //Find right
        while (right < Main.maxTilesX - 1)
        {
            right++;
            Tile tile = Main.tile[right, center.Y];
            if (WorldGen.SolidTile(tile))
                break;
        }


        //Find top
        while (top > 0)
        {
            top--;
            Tile tile = Main.tile[center.X, top];
            if (WorldGen.SolidTile(tile))
                break;
        }

        //Find bottom
        while (bottom < Main.maxTilesY - 1)
        {
            bottom++;
            Tile tile = Main.tile[center.X, bottom];
            if (tile.LiquidAmount > 0)
                break;
        }

        Point topLeft = new Point(left, top);
        Point bottomRight = new Point(right, bottom);

        Vector2 topLeftWorld = topLeft.ToWorldCoordinates();
        Vector2 bottomRightWorld = bottomRight.ToWorldCoordinates();

        Point topLeftPoint = topLeftWorld.ToPoint();
        Point bottomRightPoint = bottomRightWorld.ToPoint();
        return  new Rectangle(
            topLeftPoint.X,
            topLeftPoint.Y,
            bottomRightPoint.X - topLeftPoint.X,
            bottomRightPoint.Y - topLeftPoint.Y);
    }
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
