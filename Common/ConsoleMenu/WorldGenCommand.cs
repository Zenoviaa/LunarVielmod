using Stellamod.WorldG;

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
            "abyss"
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
                return true;
        }

        return false;
    }
}
