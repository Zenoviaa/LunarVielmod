using Stellamod.WorldG.StructureManager;
using Terraria.ModLoader;

namespace Stellamod.Common.ConsoleMenu;

public class UndoCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "undo";
    }
    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        SnapshotSystem system = ModContent.GetInstance<SnapshotSystem>();
        system.Undo();
        return true;
    }
}
