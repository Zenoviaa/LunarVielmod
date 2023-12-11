using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod
{
    public class RecipeGroupSystem : ModSystem
    {
        private void RegisterRecipeGroup(string recipeGroupName, params int[] items)
        {
            const string Any_Text = "LegacyMisc.37";
            RecipeGroup recipeGroup = new RecipeGroup(() =>
             $"{Language.GetTextValue(Any_Text)} {Lang.GetItemNameValue(items[0])}", items);
            RecipeGroup.RegisterGroup(recipeGroupName, recipeGroup);
        }

        public override void AddRecipeGroups()
        {
            base.AddRecipeGroups();
            RegisterRecipeGroup(nameof(ItemID.Candle), ItemID.Candle, ItemID.PlatinumCandle);
            RegisterRecipeGroup(nameof(ItemID.DemoniteBar), ItemID.DemoniteBar, ItemID.CrimtaneBar);
            RegisterRecipeGroup(nameof(ItemID.IronBar), ItemID.IronBar, ItemID.LeadBar);
            RegisterRecipeGroup(nameof(ItemID.GoldBar), ItemID.GoldBar, ItemID.PlatinumBar);
            RegisterRecipeGroup(nameof(ItemID.ShadowScale), ItemID.ShadowScale, ItemID.TissueSample);
        }
    }
}
