using Stellamod.Content.Biomes;
using Stellamod.WorldG;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.ConsoleMenu;

public class WorldGenCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "worldgen";
    }

    public override Arguments GetArguments()
    {
        Arguments arguments0 = new Arguments();
        arguments0.potentialArguments = new()
        {
            "abyss",
            "square",
            "darkspace"
        };

        return arguments0;
    }

    public override bool Invoke(params string[] args)
    {
        if (args.Length <= 0)
            return false;
        switch (args[1])
        {
            case "darkspace":

                {
                    int padding = 2600;
                    var DarkspaceStart = ModContent.GetInstance<StellaWorld>().DarkspaceStart;
                    var DarkspaceEnd = ModContent.GetInstance<StellaWorld>().DarkspaceEnd;
                    int darkspaceLeft = 3000;
                    int darkspaceRight = Main.maxTilesX - 1800;
                    int crystalLakeCenter = Main.maxTilesX / 2;
                    int crystalLakeLeft = crystalLakeCenter - 600;
                    int crystalLakeRight = crystalLakeCenter + 900;
                    var rectangle = new Rectangle(darkspaceLeft, DarkspaceStart, darkspaceRight - darkspaceLeft, DarkspaceEnd - DarkspaceStart);
                    for(var x = rectangle.Left; x< rectangle.Right; x++)
                    {
                        for(var y = rectangle.Top; y < rectangle.Bottom; y++)
                        {
                            var tile = Main.tile[x, y];
                            tile.ClearEverything();
                            tile.HasTile = true;// 
                            tile.TileType = TileID.Stone;
                        }
                    }


                    VeilGen.CreateDarkspaceNLake(
                        DarkspaceStart,
                        DarkspaceEnd,
                        darkspaceLeft,
                        darkspaceRight,
                        crystalLakeLeft,
                        crystalLakeRight);

                    VeilGen.CreateDarkspaceWalls(DarkspaceStart, DarkspaceEnd);
                }
                return true;
            case "abyss":
                VeilGen.GenerateAbyss();

                return true;
            case "square":
                Point center = Main.LocalPlayer.Center.ToTileCoordinates();
                int width = 170;
                int height = 300;
                Rectangle rect = new Rectangle(center.X - width / 2, center.Y - height / 2, width, height);
                for(int x = rect.Left; x < rect.Right; x++)
                {
                    for(int y = rect.Top; y < rect.Bottom; y++)
                    {
                        Tile tile = Main.tile[x, y];
                        tile.ClearEverything();
                    }
                }
                TileUtilities.UpdateMap(rect, 255);
                return true;
        }

        return false;
    }
}
