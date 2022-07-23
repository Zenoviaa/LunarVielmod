using Stellamod.Items.Consumables;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Global
{
	public class NPCPreHEdits : GlobalNPC
	{
		// ModifyNPCLoot uses a unique system called the ItemDropDatabase, which has many different rules for many different drop use cases.
		// Here we go through all of them, and how they can be used.
		// There are tons of other examples in vanilla! In a decompiled vanilla build, GameContent/ItemDropRules/ItemDropDatabase adds item drops to every single vanilla NPC, which can be a good resource.

		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{

			// We will now use the Guide to explain many of the other types of drop rules.
			

			if (npc.type == NPCID.GoblinSummoner)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 3, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}




		}
	}
}