using Stellamod.Items.MoonlightMagic;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Special
{
    internal class CommonChest : BaseChest
    {
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            //Ok we gotta add uhh
            //EVERY ENCHANTMENT LOL
            int[] enchantmentTypes = BaseEnchantment.GetTypes();
            npcLoot.Add(ItemDropRule.FewFromOptions(amount: 1, chanceDenominator: 1, enchantmentTypes));
        }
    }
}
