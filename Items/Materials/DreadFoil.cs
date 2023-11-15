using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Materials
{
    public class DreadFoil : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dread Foil");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
        }
    }
}