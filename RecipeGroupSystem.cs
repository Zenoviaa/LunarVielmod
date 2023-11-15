using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod
{
    public class RecipeGroupSystem : ModSystem
    {
        public override void AddRecipeGroups()
        {
            base.AddRecipeGroups();
            RecipeGroup candleGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.Candle)}", ItemID.Candle, ItemID.PlatinumCandle);
            RecipeGroup.RegisterGroup(nameof(ItemID.Candle), candleGroup);
        }
    }
}
