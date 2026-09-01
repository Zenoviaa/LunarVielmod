using Stellamod.Content.Areas.WondrousDarkspace.TilesWD;
using Terraria;
using Terraria.ModLoader;

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
        Cutscene preFightCutscene = ModContent.GetInstance<EPreFightCutscene>();
        preFightCutscene.Play();
        return true;
    }
}
