using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Special
{
    public class ExtraLootGlobalNPC : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);
            //Friendly npcs can't drop chests lol
            if (npc.friendly)
                return;
            if (npc.dontCountMe)
                return;
            if (npc.damage <= 0)
                return;

            if (Main.rand.NextBool(50))
            {
                //From that
                //1/5 for rare - 20%
                //Another 1/25 for mythical - 4%
                //So uhh
                //I guess that means 1/70 for common
                int num = Main.rand.Next(0, 100);

                var source = npc.GetSource_Death();
                int x = (int)npc.Center.X;
                int y = (int)npc.Center.Y;
                if (num < 4)
                {
                    NPC.NewNPC(source, x, y, ModContent.NPCType<MythicalChest>());
                }
                else if (num < 24)
                {
                    NPC.NewNPC(source, x, y, ModContent.NPCType<RareChest>());
                }
                else
                {
                    NPC.NewNPC(source, x, y, ModContent.NPCType<CommonChest>());
                }
            }
        }
    }
}
