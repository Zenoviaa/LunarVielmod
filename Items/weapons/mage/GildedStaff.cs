using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Projectiles;
using Stellamod.Items.Materials;
using Terraria.DataStructures;

namespace Stellamod.Items.weapons.mage
{
    internal class GildedStaff : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gilded Staff");
			Tooltip.SetDefault("Shoots two spinning pieces of spiritual magic at your foes!\nThe fabric is super magical, it turned wood into something like a flamethrower! :>");
		}


		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.mana = 5;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 33;
			Item.useAnimation = 66;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.staff[Item.type] = true;
			Item.noMelee = true;
			Item.knockBack = 2f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = new Terraria.Audio.LegacySoundStyle(2, 8);
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<ShadeBall>();
			Item.shootSpeed = 8f;
			Item.autoReuse = true;
			Item.crit = 22;
		}
		
		

		

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BorealWood, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 5);
		}
	}
}



    





		