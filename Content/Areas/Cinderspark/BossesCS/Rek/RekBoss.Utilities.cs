using Terraria;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
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
