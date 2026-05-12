using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

public static class NPCUtilities
{
    /// <summary>
    /// Checks if you are a multiplayer client or if you're singleplayer and sends a spawn NPC packet accordingly
    /// <typeparam name="T"></typeparam>
    /// <param name="position"></param>
    public static void SpawnNPCFromClient<T>(Vector2 position) where T : ModNPC
    {
        int x = (int)position.X;
        int y = (int)position.Y;
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            IEntitySource source = new EntitySource_Misc("BossSpawn");
            NPC npc = NPC.NewNPCDirect(source, x, y,
                ModContent.NPCType<T>());
            npc.netUpdate = true;
        }
        else
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            MultiplayerHelper.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI,
                ModContent.NPCType<T>(), x, y);
        }
    }
}
