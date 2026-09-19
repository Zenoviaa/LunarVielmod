using Stellamod.Content.Biomes;
using Terraria;

namespace Stellamod.Common.ConsoleMenu;

public class WaterfallCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "waterfalls";
    }
    
    public override Arguments GetArguments()
    {
        return null;
    }

    public override bool Invoke(params string[] args)
    {
        Main.LocalPlayer.GetModPlayer<BiomePlayer>().justEnteredAbyss = true;
        return true;
    }
}