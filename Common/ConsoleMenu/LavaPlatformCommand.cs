using Stellamod.Common.DungeonGeneration;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.ConsoleMenu;
public class LavaPlatformCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "lavaplatform";
    }
    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        NPC.NewNPC(Main.LocalPlayer.GetSource_FromThis(), (int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y, ModContent.NPCType<BigMoltenPlatform>());
        return true;
    }
}
public class GenDungeonCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "gendungeon";
    }
    public override Arguments GetArguments()
    {
        return null;
    }

    public override bool Invoke(params string[] args)
    {
        DungeonGenerationPreviewer.rooms = Dungeonizer.TestGeneration();
        return true;
    }
}
