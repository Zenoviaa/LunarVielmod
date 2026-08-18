using Stellamod.Content.Areas.Illuria.BossesIL.EStyr;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Common.ConsoleMenu;

public class StyrCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "styr";
    }

    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        Vector2 tempSpawnPoint = Main.LocalPlayer.Center;
        tempSpawnPoint.Y -= 32;
        if (MultiplayerHelper.IsHost)
        {
            int npcIndex = NPC.NewNPC(new EntitySource_Misc("cutscene"), (int)tempSpawnPoint.X, (int)tempSpawnPoint.Y, ModContent.NPCType<E>(), ai1: 3);
        }
        else
        {
            MultiplayerHelper.SpawnNPCFromClient((byte)Main.LocalPlayer.whoAmI, ModContent.NPCType<E>(), (int)tempSpawnPoint.X, (int)tempSpawnPoint.Y, ai1: 3);
        }

        return true;
    }
}
