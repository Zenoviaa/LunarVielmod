using Stellamod.Core.StructureSelector;
using Terraria.ModLoader;

namespace Stellamod.Common.ConsoleMenu;

public class StructuresCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "structures";
    }
    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        StructureSelectorUISystem uiSystem = ModContent.GetInstance<StructureSelectorUISystem>();
        uiSystem.ToggleUI();
        return true;
    }
}
