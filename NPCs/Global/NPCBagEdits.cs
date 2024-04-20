using Stellamod.Items.Consumables;
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
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TempleKeyMold>()));
            }
        }
    }
}
