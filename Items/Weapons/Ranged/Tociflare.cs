using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Gun;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;

namespace Stellamod.Items.Weapons.Ranged
{
	public class Tociflare : ModItem
	{
		public override void SetStaticDefaults()
        {

            // DisplayName.SetDefault("Tociflare"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 27;
            Item.useAnimation = 27;
            Item.useStyle = 5;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item73;
            Item.shoot = ProjectileType<TociBolt4>();
            Item.useAmmo = AmmoID.Gel;
            Item.shootSpeed = 25f;
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<VirulentPlating>(), 30);
            recipe.AddIngredient(ItemType<DreadFoil>(), 10);
            recipe.AddIngredient(ItemType<VerianBar>(), 5);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }
    }
}