using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Consumables;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Summon;
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

			if (npc.type == NPCID.Antlion)
			{


				npcLoot.Add(ItemDropRule.Common(ItemID.SandBlock, 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Stick>(), 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.

				npcLoot.Add(ItemDropRule.Common(ItemID.SandBoots, 25, 1, 1)); 
				npcLoot.Add(ItemDropRule.Common(ItemID.Sandgun, 40, 1, 1)); 
			}

			if (npc.type == NPCID.Zombie)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Stick>(), 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CondensedDirt>(), 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GrassDirtPowder>(), 30, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			if (npc.type == NPCID.FlyingFish)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FlyingFishBroochA>(), 15, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			if (npc.type == NPCID.GreenSlime)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SlimeBroochA>(), 100, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			if (npc.type == NPCID.BloodZombie)
			{


				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FleshClot>(), 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CondensedDirt>(), 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodLamp>(), 600, 1, 1));
			}
			
			if (npc.type == NPCID.Drippler)
			{

				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodLamp>(), 600, 1, 1));
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FleshClot>(), 3, 1, 5)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			

			}

		}
	}
}