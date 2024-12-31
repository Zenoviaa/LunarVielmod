using Stellamod.Common.Bases;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class KilliaBroochA : BaseBrooch
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
            broochSpawnerPlayer.broochesToSpawn.Add(ModContent.ItemType<VixedBroochA>());
            broochSpawnerPlayer.broochesToSpawn.Add(ModContent.ItemType<RoseBroochA>());
            broochSpawnerPlayer.broochesToSpawn.Add(ModContent.ItemType<DreadBroochA>());
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<VixedBroochA>(), 1);
            recipe.AddIngredient(ModContent.ItemType<DreadBroochA>(), 1);
            recipe.AddIngredient(ModContent.ItemType<RoseBroochA>(), 1);
            recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 25);
            recipe.AddTile(ModContent.TileType<BroochesTable>());
            recipe.Register();
        }
    }
}