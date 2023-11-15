using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
    class IrradiatedBar : ModItem
    {
        public static bool spawnLumiOre = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Irradiated Bar");
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.value = 10000;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Green;
        }
    }
}
