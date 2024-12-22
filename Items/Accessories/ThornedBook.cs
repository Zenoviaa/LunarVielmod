using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class ThornedBook : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Thorned Book of Fiber Cordage");
            /* Tooltip.SetDefault("When enemies hit you deal big amounts of damage back" +
				"\n+10% Melee damage..." +
				"\n-5 defense"); */

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(silver: 12);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MyPlayer>().ThornedBook = true;
            player.GetDamage(DamageClass.Melee) += 0.03f;
            player.statDefense -= 5;
        }
    }
}