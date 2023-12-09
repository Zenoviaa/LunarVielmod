using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Slashers.Voyager;
using Stellamod.Projectiles.Swords.Altride;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Melee
{
	public class AltrideSlicer : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frost Swing");
			/* Tooltip.SetDefault("Shoots one bone bolt to swirl and kill your enemies after attacking!" +
			"\nHitting foes with the melee swing builds damage towards the swing of the weapon"); */
		}
		public override void SetDefaults()
		{
			Item.damage = 40;
			Item.DamageType = DamageClass.Melee;
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 24;
			Item.useAnimation = 24;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 10;
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.value = 100000;
			Item.shoot = ModContent.ProjectileType<AltrideProj>();
			Item.shootSpeed = 10f;
			Item.noUseGraphic = true;
			Item.noMelee = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.MythrilAnvil);

			recipe.AddIngredient(ModContent.ItemType<Stick>(), 10);
			recipe.AddIngredient(ModContent.ItemType<ConvulgingMater>(), 10);
			recipe.AddIngredient(ModContent.ItemType<AlcaricMush>(), 10);
			recipe.AddIngredient(ModContent.ItemType<MooningSlicer>(), 1);
			recipe.AddIngredient(ModContent.ItemType<SpacialDistortionFragments>(), 15);
			recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 9);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 12);

			recipe.Register();
		}
	}
}