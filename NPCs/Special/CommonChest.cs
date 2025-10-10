using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Special
{
    public class CommonChest : BaseChest
    {
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);

            npcLoot.Add(ItemDropRule.Coins(Item.silver * 50, true));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Medal>(), minimumDropped: 3, maximumDropped: 6));
            int[] enchantmentTypes = BaseEnchantment.GetNonSpecialTypes();
            npcLoot.Add(ItemDropRule.FewFromOptions(amount: 1, chanceDenominator: 1, enchantmentTypes));
        }
    }
}
