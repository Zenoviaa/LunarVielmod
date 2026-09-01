using Stellamod.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.WorldG.StructureManager.Snapshot;

namespace Stellamod.Core.Utilities;

public static class NPCUtilities
{
    public static void SetDomainArenaY(NPC npc, ref float arenaY)
    {
        if(arenaY == 0)
        {
            npc.TargetClosest();
            arenaY = Main.player[npc.target].Top.Y;
            npc.netUpdate = true;
        }

    }
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

    /// <summary>
    /// Receives a request to change an NPC's ai slots from a client
    /// </summary>
    /// <param name="npcWhoAmI"></param>
    /// <param name="ai0"></param>
    /// <param name="ai1"></param>
    /// <param name="ai2"></param>
    /// <param name="ai3"></param>
    public static void HandleNPCAIChange(int npcWhoAmI, float ai0 = -1, float ai1 = -1, float ai2 = -1, float ai3 = -1)
    {
        NPC npc = Main.npc[npcWhoAmI];
        npc.ai[0] = ai0 != -1 ? ai0 : npc.ai[0];
        npc.ai[1] = ai1 != -1 ? ai1 : npc.ai[1];
        npc.ai[2] = ai2 != -1 ? ai2 : npc.ai[2];
        npc.ai[3] = ai3 != -1 ? ai3 : npc.ai[3];

        //The server is the authority for how npcs function, so after it receives the change it should just net update to sync across all clients
        if (Main.netMode == NetmodeID.Server)
            npc.netUpdate = true;
    }

    public static void ChangeNPCAIFromClient(int npcWhoAmI, float ai0 = -1, float ai1 = -1, float ai2 = -1, float ai3 = -1)
    {
        HandleNPCAIChange(npcWhoAmI, ai0, ai1, ai2, ai3);
        if (Main.netMode == NetmodeID.SinglePlayer)
            return;

        Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.ChangeNPCAI,
            (int)npcWhoAmI,
            (float)ai0,
            (float)ai1,
            (float)ai2,
            (float)ai3).Send(-1);
    }
}
