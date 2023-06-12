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
	public class MetalDagger : ModItem
	{
		public override void SetStaticDefaults()
		{
			/* Tooltip.SetDefault("Let them burn in harmony!" +
				"\nSimple weapon forged from Stellean bricks and the heat from plants of the morrow" +
				"\nImpractical but very rewarding..."); */
			// DisplayName.SetDefault("Violiar");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
		}
	
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.rare = ItemRarityID.Green;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.DD2_MonkStaffSwing;

			// Weapon Properties
			Item.DamageType = DamageClass.Melee;
			Item.damage = 12;
			Item.knockBack = 20f;
			Item.noMelee = true;
			Item.crit = 6;
			Item.consumable = true;
			Item.maxStack = 9999;

			// Gun Properties
			Item.shoot = ModContent.ProjectileType<MetalDaggerProj>();
			Item.shootSpeed = 12f;
		}
	
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(50);

			recipe.AddIngredient(ModContent.ItemType<AlcadizMetal>(), 1);


			recipe.Register();
		}
	}
}












