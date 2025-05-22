using Stellamod.Items.Accessories;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Thrown.Jugglers;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.NPCs.Global
{
    public class NPCBossEdits : GlobalNPC
	{

        public override void SetDefaults(NPC npc)
        {
			if(npc.type == NPCID.TheDestroyer)
            {
				float lifeMax = npc.lifeMax;
				lifeMax *= 1.8f;
				npc.lifeMax = (int)lifeMax;
			}
			if(npc.type == NPCID.HallowBoss)
            {
				float lifeMax = npc.lifeMax;
				lifeMax *= 1.25f;
				npc.lifeMax = (int)lifeMax;
			}
			if(npc.type == NPCID.DukeFishron)
            {
				float lifeMax = npc.lifeMax;
				lifeMax *= 1.25f;
				npc.lifeMax = (int)lifeMax;
			}
			if(npc.type == NPCID.Plantera)
            {
				float lifeMax = npc.lifeMax;
				lifeMax *= 2f;
				npc.lifeMax = (int)lifeMax;
			}
        }

        // ModifyNPCLoot uses a unique system called the ItemDropDatabase, which has many different rules for many different drop use cases.
        // Here we go through all of them, and how they can be used.
        // There are tons of other examples in vanilla! In a decompiled vanilla build, GameContent/ItemDropRules/ItemDropDatabase adds item drops to every single vanilla NPC, which can be a good resource.

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			
			// We will now use the Guide to explain many of the other types of drop rules.
			if (npc.type == NPCID.Guide)
			{
				// RemoveWhere will remove any drop rule that matches the provided expression.
				// To make your own expressions to remove vanilla drop rules, you'll usually have to study the original source code that adds those rules.
				npcLoot.RemoveWhere(
					// The following expression returns true if the following conditions are met:
					rule => rule is ItemDropWithConditionRule drop // If the rule is an ItemDropWithConditionRule instance
						&& drop.itemId == ItemID.GreenCap // And that instance drops a green cap
						&& drop.condition is Conditions.NamedNPC npcNameCondition // ..And if its condition is that an npc name must match some string
						&& npcNameCondition.neededName == "Andrew" // And the condition's string is "Andrew".
				);

				npcLoot.Add(ItemDropRule.Common(ItemID.GreenCap, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			//Gambits
			if (npc.type == NPCID.EyeofCthulhu)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 1));
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Steali>(), 1, 1, 1));// In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DarkEssence>(), 1, 1, 30)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}
			
			
			if (npc.type == NPCID.BrainofCthulhu)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			if (npc.type == NPCID.SkeletronHead)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}
			
			if (npc.type == NPCID.KingSlime)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			if (npc.type == NPCID.WallofFlesh)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 3, 5));

                LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
                notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CatacombsKey>(), chanceDenominator: 1));
                npcLoot.Add(notExpertRule);// In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

            if (npc.type == NPCID.QueenBee)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}
			if (npc.type == NPCID.Plantera)
            {
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 3, 7));
                List<IItemDropRule> rules = npcLoot.Get();
                for (int i = 0; i < rules.Count; i++)
                {
                    IItemDropRule rule = rules[i];
                    if (rule is CommonDrop commonDrop
						&& commonDrop.itemId == ItemID.TempleKey)
                    {
                        npcLoot.Remove(rule);
                    }
                }
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LilStinger>(), chanceDenominator: 4));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TempleKeyMold>()));
            }

			if (npc.type == NPCID.SkeletronPrime)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 3, 5));
            }

			if (npc.type == NPCID.Retinazer)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 2));

			}

			if (npc.type == NPCID.Spazmatism)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 2));

			}

			if (npc.type == NPCID.TheDestroyer)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 4));

			}

			if (npc.type == NPCID.QueenSlimeBoss)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 2, 3));

			}

			if (npc.type == NPCID.Golem)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 3, 7));

			}

			if (npc.type == NPCID.HallowBoss)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 5, 9));

			}

			if (npc.type == NPCID.MoonLordCore)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 5, 9));

			}

			if (npc.type == NPCID.DukeFishron)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 5, 9));

			}

			if (npc.type == NPCID.CultistBoss)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 5, 9));
            }

            if (npc.type == NPCID.MoonLordCore)
            {
                LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
                notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.LunarOre, chanceDenominator: 1, 80, 120));
                npcLoot.Add(notExpertRule);// In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
            }

            if (npc.type == NPCID.Deerclops)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			// Editing an existing drop rule
			if (npc.type == NPCID.BloodNautilus)
			{
				// Dreadnautilus, known as BloodNautilus in the code, drops SanguineStaff. The drop rate is 100% in Expert mode and 50% in Normal mode. This example will change that rate.
				// The vanilla code responsible for this drop is: ItemDropRule.NormalvsExpert(4269, 2, 1)
				// The NormalvsExpert method creates a DropBasedOnExpertMode rule, and that rule is made up of 2 CommonDrop rules. We'll need to use this information in our casting to properly identify the recipe to edit.

				// There are 2 options. One option is remove the original rule and then add back a similar rule. The other option is to modify the existing rule.
				// It is preferred to modify the existing rule to preserve compatibility with other mods.

				// Adjust the existing rule: Change the Normal mode drop rate from 50% to 33.3%
				foreach (var rule in npcLoot.Get())
				{
					// You must study the vanilla code to know what to objects to cast to.
					if (rule is DropBasedOnExpertMode drop && drop.ruleForNormalMode is CommonDrop normalDropRule && normalDropRule.itemId == ItemID.SanguineStaff)
						normalDropRule.chanceDenominator = 2;
				}


               
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 3, 3)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DarkEssence>(), 1, 1, 30)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.																		  // Remove the rule, then add another rule: Change the Normal mode drop rate from 50% to 16.6%
				/*
				npcLoot.RemoveWhere(
					rule => rule is DropBasedOnExpertMode drop && drop.ruleForNormalMode is CommonDrop normalDropRule && normalDropRule.itemId == ItemID.SanguineStaff
				);
				npcLoot.Add(ItemDropRule.NormalvsExpert(4269, 6, 1));
				*/
			}

			
		}

		// ModifyGlobalLoot allows you to modify loot that every NPC should be able to drop, preferably with a condition.
		// Vanilla uses this for the biome keys, souls of night/light, as well as the holiday drops.
		// Any drop rules in ModifyGlobalLoot should only run once. Everything else should go in ModifyNPCLoot.
		
	}
}