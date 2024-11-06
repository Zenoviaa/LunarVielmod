using Stellamod.Items.Materials;
using Stellamod.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Flasks
{
    public class VitalityInsource : BaseInsource
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            InsourcePotionSickness = 2400;
            InsourceCannotUseDuration = 2400;
        }

        public override void TriggerEffect(Player player)
        {
            base.TriggerEffect(player);
            player.statMana += 102;
            player.AddBuff(BuffID.Honey, 2400);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DreadFoil>(), 10);
            recipe.AddIngredient(ModContent.ItemType<TerrorFragments>(), 10);
            recipe.AddIngredient(ItemID.Waterleaf, 5);
            recipe.AddIngredient(ItemID.BottledHoney, 5);
            recipe.AddIngredient(ItemID.Bottle, 1);
            recipe.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe.Register();
        }
    }
}