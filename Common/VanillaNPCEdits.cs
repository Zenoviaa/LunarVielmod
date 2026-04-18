using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common;

public class VanillaNPCEdits : GlobalNPC
{
    public override bool PreAI(NPC npc)
    {
        if (npc.type == NPCID.OldMan)
        {
            //Screw you
            npc.active = false;
            return false;
        }

        return base.PreAI(npc);
    }
}
