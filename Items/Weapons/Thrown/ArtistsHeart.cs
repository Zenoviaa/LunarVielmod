
using Microsoft.Xna.Framework;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Paint;
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.Items.Weapons.Thrown
{
    public class ArtistsHeart : ModItem
	{
        public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Cactius"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 40;
            Item.noUseGraphic = true;
            Item.height = 40;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightPurple;
            Item.crit = 30;
            Item.UseSound = SoundID.DD2_GhastlyGlaivePierce;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ArtistsHProj>();
            Item.shootSpeed = 15f;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.CrystalShard, 13);
            recipe.AddIngredient(ModContent.ItemType<KaleidoscopicInk>(), 20);
            recipe.AddIngredient(ModContent.ItemType<ArtisanBar>(), 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}