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
	public class BurningAngel : ModItem
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
			Item.useTime = 51;
			Item.useAnimation = 51;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.autoReuse = true;
			Item.UseSound = SoundID.DD2_FlameburstTowerShot;

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 8;
			Item.knockBack = 5f;
			Item.noMelee = true;
			Item.crit = 25;

			// Gun Properties
			Item.shoot = ModContent.ProjectileType<BurningAngelProj>();
			Item.shootSpeed = 4f;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(2f, -2f);
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.StoneBlock, 50);
			recipe.AddTile(TileID.Anvils);	
			recipe.AddIngredient(ModContent.ItemType<Hlos>(), 1);
			recipe.AddIngredient(ModContent.ItemType<Fabric>(), 3);
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 15);
			recipe.AddIngredient(ModContent.ItemType<AlcadizMetal>(), 3);
			recipe.AddIngredient(ItemID.Silk, 5);

			recipe.Register();
		}
	}
}












