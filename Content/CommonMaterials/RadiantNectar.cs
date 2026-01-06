using Terraria.ModLoader;

namespace Stellamod.Content.CommonMaterials
{
    public class RadiantNectar : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<RadiantNectarRarity>();
        }
    }
}
