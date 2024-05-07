﻿
using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Accessories
{
    public class ArncharMagazine : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Arnchar Magazine");
            // Tooltip.SetDefault("Increases ranged critical strike chance by 5%");
        } 
        public override void SetDefaults()
        {
            Item.Size = new Vector2(20);
            Item.accessory = true;
            Item.value = Item.sellPrice(silver: 80);
            Item.rare = ItemRarityID.Blue;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance(DamageClass.Ranged) += 10f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<Cinderscrap>(), 50);
            recipe.AddIngredient(ItemType<MoltenScrap>(), 5);
            recipe.AddIngredient(ItemType<ArnchaliteBar>(), 15);
            recipe.AddIngredient(ItemID.MusketBall, 50);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
