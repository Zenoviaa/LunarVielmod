using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
	public class Violar : ModItem
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
			Item.scale = 0.75f;
			Item.rare = ItemRarityID.Green;
			Item.useTime = 100;
			Item.useAnimation = 100;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.autoReuse = true;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/violar");

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 25;
			Item.knockBack = 5f;
			Item.noMelee = true;
			Item.crit = 25;

			// Gun Properties
			Item.shoot = ModContent.ProjectileType<Violarproj>();
			Item.shootSpeed = 4f;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(2f, -2f);
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.StoneBlock, 100);
			recipe.AddTile(TileID.Anvils);
			
			recipe.AddIngredient(ModContent.ItemType<Hlos>(), 1);
			recipe.AddIngredient(ModContent.ItemType<ViolinStick>(), 1);
			recipe.AddIngredient(ModContent.ItemType<Stick>(), 10);
			recipe.AddIngredient(ModContent.ItemType<Fabric>(), 15);
			recipe.AddIngredient(ModContent.ItemType<OvermorrowWood>(), 15);
			recipe.AddIngredient(ModContent.ItemType<FlowerBatch>(), 1);
			recipe.AddIngredient(ModContent.ItemType<MorrowRocks>(), 15);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 15);
			recipe.AddIngredient(ItemID.Silk, 5);

			recipe.Register();
		}
	}
}












