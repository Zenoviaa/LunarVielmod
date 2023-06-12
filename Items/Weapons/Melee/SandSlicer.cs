using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
	public class SandSlicer : ModItem
	{
		public override void SetStaticDefaults()
		{
			/* Tooltip.SetDefault("Let them burn in harmony!" +
				"\nSimple weapon forged from Stellean bricks and the heat from plants of the morrow" +
				"\nImpractical but very rewarding..."); */
			// DisplayName.SetDefault("Violiar");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
	
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 10;
			Item.rare = ItemRarityID.Green;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.DD2_DarkMageAttack;

			// Weapon Properties
			Item.DamageType = DamageClass.Melee;
			Item.damage = 8;
			Item.knockBack = 10f;
			Item.noMelee = true;
			Item.crit = 26;

			// Gun Properties
			Item.shoot = ModContent.ProjectileType<SandSlacerProj>();
			Item.shootSpeed = 4f;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(2f, -2f);
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.SandBlock, 50);
			recipe.AddIngredient(ItemID.AntlionMandible, 5);
			recipe.AddTile(TileID.Anvils);	
			recipe.AddIngredient(ModContent.ItemType<Hlos>(), 1);
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 3);


			recipe.Register();
		}
	}
}












