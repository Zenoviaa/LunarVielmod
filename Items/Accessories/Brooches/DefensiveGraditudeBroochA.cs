using Stellamod.Common.Bases;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class DefensiveGraditudeBroochA : BaseBrooch
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 49;
            Item.height = 34;
            Item.value = Item.sellPrice(gold: 15);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
            BroochType = BroochType.Radiant;
        }

        public override void UpdateBrooch(Player player)
        {
            base.UpdateBrooch(player);
            BroochSpawnerPlayer broochSpawnerPlayer = player.GetModPlayer<BroochSpawnerPlayer>();
            broochSpawnerPlayer.broochesToSpawn.Add(ModContent.ItemType<SpragaldBroochA>());
            broochSpawnerPlayer.broochesToSpawn.Add(ModContent.ItemType<MorrowedJelliesBroochA>());
            player.GetDamage(DamageClass.Generic) *= 0.70f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<BlankBrooch>(), 1);
            recipe.AddIngredient(ModContent.ItemType<MorrowedJelliesBroochA>(), 1);
            recipe.AddIngredient(ModContent.ItemType<SpragaldBroochA>(), 1);
            recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 15);
            recipe.AddTile(ModContent.TileType<BroochesTable>());
            recipe.Register();
        }
    }
}