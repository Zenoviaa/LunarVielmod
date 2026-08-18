using Terraria.ModLoader;

namespace Stellamod.Common.ConsoleMenu;

public abstract class ConsoleCommand : ModType
{
    protected override void Register()
    {
        ModTypeLookup<ConsoleCommand>.Register(this);
    }

    public abstract string GetCommandName();
    public abstract Arguments GetArguments();
    public abstract bool Invoke(params string[] args);
}
