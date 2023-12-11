using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Stellamod.Items.Accessories
{
    internal class LunarBand : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lunar Band");
            // Tooltip.SetDefault("always increased critical strike chance & increased damage in the night ");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 2500;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!Main.dayTime)
            {
                player.GetDamage(DamageClass.Generic) += 0.13f;
                player.GetCritChance(DamageClass.Generic) += 15;
            }
        }
    }
}