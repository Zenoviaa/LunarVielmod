using Stellamod.Common.Bases;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class HeatedVerliaBroochA : BaseBrooch
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
            broochSpawnerPlayer.broochesToSpawn.Add(ModContent.ItemType<BurningGBroochA>());
            broochSpawnerPlayer.broochesToSpawn.Add(ModContent.ItemType<VerliaBroochA>());

            player.GetDamage(DamageClass.Ranged) *= 1.15f;
            player.GetDamage(DamageClass.Throwing) *= 1.15f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<BlankBrooch>(), 1);
            recipe.AddIngredient(ModContent.ItemType<VerliaBroochA>(), 1);
            recipe.AddIngredient(ModContent.ItemType<BurningGBroochA>(), 1);
            recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 25);
            recipe.AddTile(ModContent.TileType<BroochesTable>());
            recipe.Register();
        }
    }


}