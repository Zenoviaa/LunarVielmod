using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories

{
    public class ArncharSuperCharger : ModItem
    {    
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Toxic Striders");
            // Tooltip.SetDefault("Increases Move Speed by 9%");
        }

        public override void SetDefaults()
        {
            Item.Size = new Vector2(20);
            Item.accessory = true;
            Item.value = Item.sellPrice(silver: 25);
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MyPlayer>().ArchariliteSC = true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<ArnchaliteBar>(), 15);
            recipe.AddRecipeGroup(nameof(ItemID.ShadowScale), 10);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 1);
            recipe.Register();
        }
    }
 }

