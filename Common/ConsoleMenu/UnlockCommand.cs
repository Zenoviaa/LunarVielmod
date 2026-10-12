using Stellamod.Common.SummonerSystem;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Content.Items.MoonlightMagic;
using Terraria;

namespace Stellamod.Common.ConsoleMenu;

public class UnlockCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "unlock";
    }
    public override Arguments GetArguments()
    {
        Arguments args = new Arguments();
        args.potentialArguments.Add("insource");
        args.potentialArguments.Add("wand");
        return args;
    }
    public override bool Invoke(params string[] args)
    {
        var type = args[1];
        var player = Main.LocalPlayer;
        switch (type)
        {
            case "insource":
                player.GetModPlayer<FlaskPlayer>().GrantAllProgress();
                player.GetModPlayer<BellPlayer>().GrantAllProgress();
                break;
            case "wand":
                player.GetModPlayer<AdvancedMagicPlayer>().GrantAllProgress();
                break;
        }

        return true;
    }
}
