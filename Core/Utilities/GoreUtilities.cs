using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

public static class GoreUtilities
{
    public static void CreateDeathGores(ModNPC modNPC, int numGores)
    {
        if (Main.netMode == NetmodeID.Server)
            return;


        for (int i = 0; i < numGores; i++)
        {
            int gore = modNPC.Mod.Find<ModGore>($"{modNPC.Name}_Gore_{i}").Type;
            NPC npc = modNPC.NPC;
            // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
            Vector2 pos = npc.Center + Main.rand.NextVector2Circular(32, 32);
            Vector2 vel = npc.velocity + (pos - npc.Center).SafeNormalize(Vector2.Zero) * 5;
            Gore.NewGore(npc.GetSource_Death(), pos, vel, gore, 1f);
        }
    }
}
