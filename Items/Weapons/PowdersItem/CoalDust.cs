using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Powders;
using Stellamod.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.PowdersItem
{
	internal class CoalDust : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Coal Powder");
			Tooltip.SetDefault("Throw magical dust on them!" +
				"\nForged from the gems you collect!");
		}
		public override void SetDefaults()
		{
			Item.damage = 4;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<CoalPowder>();
			Item.autoReuse = true;
			Item.shootSpeed = 12f;
			Item.crit = 43;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Lenabee");
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.StoneBlock, 50);
			recipe.AddIngredient(ItemID.DirtBlock, 15);
			recipe.AddIngredient(ItemID.Bomb, 3);
			recipe.AddTile(ModContent.TileType<AlcaologyTable>());
			recipe.Register();
			recipe.AddIngredient(ModContent.ItemType<Bagitem>(), 5);
			recipe.AddIngredient(ModContent.ItemType<MorrowVine>(), 5);
			recipe.AddIngredient(ModContent.ItemType<Fabric>(), 15);
			recipe.AddIngredient(ItemID.Silk, 5);
		}

	}
}