using Stellamod.Common.Bases;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class SandyBearBroochA : BaseBrooch
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
            broochSpawnerPlayer.broochesToSpawn.Add(ModContent.ItemType<SandyBroochA>());
            broochSpawnerPlayer.broochesToSpawn.Add(ModContent.ItemType<BearBroochA>());
            player.maxMinions += 2;
            player.GetDamage(DamageClass.Summon) += 0.2f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<BlankBrooch>(), 1);
            recipe.AddIngredient(ModContent.ItemType<SandyBroochA>(), 1);
            recipe.AddIngredient(ModContent.ItemType<BearBroochA>(), 1);
            recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 5);
            recipe.AddTile(ModContent.TileType<BroochesTable>());
            recipe.Register();
        }
    }
}