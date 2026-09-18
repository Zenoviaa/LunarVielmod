using Terraria;

namespace Stellamod.Common.ConsoleMenu;

public class MapCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "map";
    }
    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        TileUtilities.UpdateMap(new Rectangle(0, 0, Main.maxTilesX, Main.maxTilesY), 255);
        return true;
    }
}
