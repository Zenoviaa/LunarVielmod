using Terraria;

namespace Stellamod.Common.ConsoleMenu;

public class NoMapCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "nomap";
    }
    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        TileUtilities.UpdateMap(new Rectangle(0, 0, Main.maxTilesX, Main.maxTilesY), 0);
        return true;
    }
}
