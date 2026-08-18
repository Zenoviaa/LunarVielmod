namespace Stellamod.Common.ConsoleMenu;

public class CloudsCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "clouds";
    }

    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        LunarDebugging.clouds = !LunarDebugging.clouds;
        return true;
    }
}
