using Terraria.ModLoader;
using Terraria;

namespace Stellamod.Content.CommonMaterials
{
    public class RadiantNectar : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<RadiantNectarRarity>();
        }
    }
}
