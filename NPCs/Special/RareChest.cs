using Microsoft.Xna.Framework;
using Stellamod.Content.Items.MoonlightMagic;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Special
{
    public class RareChest : BaseChest
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
            int[] enchantmentTypes = BaseEnchantment.GetNonSpecialTypes();
            npcLoot.Add(ItemDropRule.FewFromOptions(amount: 2, chanceDenominator: 1, enchantmentTypes));
        }
    }
}
