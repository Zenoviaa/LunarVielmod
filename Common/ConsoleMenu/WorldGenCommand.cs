using Stellamod.Content.Biomes;
using Stellamod.WorldG;
using Terraria;

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
            "square"
        };

        return arguments0;
    }

    public override bool Invoke(params string[] args)
    {
        if (args.Length <= 0)
            return false;
        switch (args[1])
        {
            case "abyss":
                VeilGen.GenerateAbyss();
                Main.LocalPlayer.GetModPlayer<BiomePlayer>().justEnteredAbyss = true;
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
