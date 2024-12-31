using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Thrown.Jugglers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Global
{
    internal class NPCBagEdits : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if(item.type == ItemID.WallOfFleshBossBag)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CatacombsKey>()));
            }

            if(item.type == ItemID.MoonLordBossBag)
            {
                itemLoot.Add(ItemDropRule.Common(ItemID.LunarOre, 1, 80, 120));
            }

            //This code checks for and removes a specific item
            if(item.type == ItemID.PlanteraBossBag)
            {
                List<IItemDropRule> rules = itemLoot.Get();
                for (int i = 0; i < rules.Count; i++)
                {
                    IItemDropRule rule = rules[i];
                    if (rule is CommonDrop commonDrop && commonDrop.itemId == ItemID.TempleKey)
                    {
                        itemLoot.Remove(rule);
                    }
                }

                LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
                notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<LilStinger>(), chanceDenominator: 4));
                itemLoot.Add(notExpertRule);
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TempleKeyMold>()));
            }
        }
    }
}
