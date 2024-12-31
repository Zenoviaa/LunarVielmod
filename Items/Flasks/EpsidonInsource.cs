using Stellamod.Items.Materials;
using Stellamod.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Flasks
{
    public class EpsidonInsource : BaseInsource
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            InsourceHealValue = 200;
            InsourceCannotUseDuration = 2400;
            InsourcePotionSickness = 2400;
        }

        public override void TriggerEffect(Player player)
        {
            base.TriggerEffect(player);
            player.AddBuff(BuffID.Honey, 1200);
            player.AddBuff(BuffID.Swiftness, 1200);
            player.AddBuff(BuffID.Ironskin, 1200);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<STARCORE>(), 3);
            recipe.AddIngredient(ModContent.ItemType<MiracleThread>(), 10);
            recipe.AddIngredient(ItemID.Fireblossom, 10);
            recipe.AddIngredient(ItemID.Deathweed, 2);
            recipe.AddIngredient(ItemID.GreaterHealingPotion, 10);
            recipe.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe.Register();
        }
    }
}