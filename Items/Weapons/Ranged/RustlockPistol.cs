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
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.HardMode;

namespace Stellamod.Items.Weapons.Ranged
{
	public class RustlockPistol : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Western Pistol"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
        public override void SetDefaults()
		{
			Item.damage = 45;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 56;
			Item.height = 56;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = 5;
			Item.knockBack = 6;
			Item.value = 100000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item11;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.Bullet;
			Item.shopCustomPrice = 23;
			Item.shootSpeed = 15;
			Item.useAmmo = AmmoID.Bullet;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.GoldBar, 1);
            recipe.AddIngredient(ModContent.ItemType<SteampunkBar>(), 10);
            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.Register();
        }
    }
}