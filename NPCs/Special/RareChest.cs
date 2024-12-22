using Microsoft.Xna.Framework;
using Stellamod.Items.MoonlightMagic;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Special
{
    internal class RareChest : BaseChest
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            ChestColor = new Color(56, 134, 65);
        }
        public override void AI()
        {
            base.AI();
            if (Timer == 1)
            {

            }
        }

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
