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
	internal class GrassDirtPowder : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dirt Powder");
			/* Tooltip.SetDefault("Throw magical dust on them!" +
				"\nA dirty dust... that does damage???!"); */
		}
		public override void SetDefaults()
		{
			Item.damage = 3;
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
			Item.shoot = ModContent.ProjectileType<DirtPowder>();
			Item.autoReuse = true;
			Item.shootSpeed = 6f;
			Item.crit = 51;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Lenabee");
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Daybloom, 2);
			recipe.AddIngredient(ItemID.DirtBlock, 15);
			recipe.AddIngredient(ItemID.Seed, 3);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
			recipe.AddIngredient(ModContent.ItemType<Bagitem>(), 1);
			recipe.AddIngredient(ModContent.ItemType<MorrowVine>(), 1);
			recipe.AddIngredient(ModContent.ItemType<Fabric>(), 3);
		}

	}
}