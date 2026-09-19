namespace Stellamod.Common.ConsoleMenu;

public class AllBossesCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "allbosses";
    }
    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        DownedBossTracker.AllBosses();
        return true;
    }
}