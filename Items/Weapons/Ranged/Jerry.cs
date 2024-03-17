using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
	public class Jerry : ModItem
	{
		public override void SetStaticDefaults()
		{
			/* Tooltip.SetDefault("Classy!" +
				"\nTotally has no reference to Hollow Knight" +
				"\nA weapon that shoots classy bomb and bullets.. A LOT" +
				"\nDia Gun..."); */
			// DisplayName.SetDefault("Hornet");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{

			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(Mod, "", "");
			line = new TooltipLine(Mod, "Horneta", "Jerry does not like being called names")
			{
				OverrideColor = new Color(108, 271, 99)

			};
			tooltips.Add(line);


		}

		public override void SetDefaults()
		{
			Item.width = 74;
			Item.height = 38;
			Item.scale = 0.9f;
			Item.rare = ItemRarityID.LightRed;
			Item.useTime = 12;
			Item.useAnimation = 12;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.autoReuse = true;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/gun1");

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 52;
			Item.knockBack = 4;
			Item.noMelee = true;

			// Gun Properties
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 8f;
			Item.useAmmo = AmmoID.Bullet; // Restrict the type of ammo the weapon can use, so that the weapon cannot use other ammos
			Item.value = Item.sellPrice(silver: 50);
		}
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		// This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(0f, -8f);
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{

			type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<RobedProjectile>(), ProjectileID.Grenade, ProjectileID.CannonballFriendly });
		}

		/*
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.AddIngredient(ModContent.ItemType<BasicGunParts>(), 1);
			recipe.AddIngredient(ModContent.ItemType<Starrdew>(), 10);
			recipe.AddIngredient(ModContent.ItemType<ArnchaliteBar>(), 5);
			recipe.AddIngredient(ModContent.ItemType<GraftedSoul>(), 10);
			recipe.AddIngredient(ModContent.ItemType<AlcadizScrap>(), 9);
			recipe.Register();
		}*/
	}
}