using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Stellamod.Tiles.Structures;
using Stellamod.Tiles.Abyss.Aurelus;

namespace Stellamod.Items.Placeable
{
    public class AurelusSummoni : ModItem
	{
        public override void SetDefaults()
        {
            // Vanilla has many useful methods like these, use them! This substitutes setting Item.createTile and Item.placeStyle aswell as setting a few values that are common across all placeable items
            Item.createTile = Mod.Find<ModTile>("AurelusSummon").Type;
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
        }
    }
}
