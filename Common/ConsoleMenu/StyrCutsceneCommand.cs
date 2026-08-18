using Stellamod.Content.Areas.WondrousDarkspace.TilesWD;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.ConsoleMenu;

public class StyrCutsceneCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "styrscene";
    }

    public override Arguments GetArguments()
    {
        return null;
    }

    public override bool Invoke(params string[] args)
    {
        var p = TileUtilities.CenterTileBoundsTileSpace(Main.LocalPlayer.Center, 14096, 14096);
        for(int x = p.topLeft.X; x < p.bottomRight.X; x++)
        {
            for(int y = p.topLeft.Y; y < p.bottomRight.Y; y++)
            {
                Tile tile = Main.tile[x, y];
                if(tile.TileType == ModContent.TileType<MiracleSilkTile>())
                {
                    WorldGen.KillTile(x, y);
                }
            }
        }
        return true;
    }
}
