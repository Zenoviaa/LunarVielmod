
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class SunGlyph : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sun Glyph");
			// Tooltip.SetDefault("Greatly increases all damage in the daylight.");
		}

		public override void SetDefaults()
		{
			Item.value = Item.sellPrice(gold: 1);
            Item.width = 26;
            Item.height = 34;
            Item.rare = ItemRarityID.Expert;
            Item.value = 1200;
            Item.accessory = true;
            Item.expert = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (Main.dayTime)
            {
                player.GetDamage(DamageClass.Generic) += 0.30f;
            }
        }
	}
}
