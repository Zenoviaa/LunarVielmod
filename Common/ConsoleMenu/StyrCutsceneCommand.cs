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
        return false;
    }
}
