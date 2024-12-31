using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials.Tech
{
    public class ArmorDrive : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Armor Drive");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
        }

    }
}