using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
	public class PoisonedAngel : ModItem
	{
		public override void SetStaticDefaults()
		{
			/* Tooltip.SetDefault("Let them burn in harmony!" +
				"\nSimple weapon forged from Stellean bricks and the heat from plants of the morrow" +
				"\nImpractical but very rewarding..."); */
			// DisplayName.SetDefault("Violiar");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{

			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(Mod, "", "");
			line = new TooltipLine(Mod, "Burniaaa", "(A) Great Damage scaling for explosions!")
			{
				OverrideColor = new Color(108, 271, 99)

			};
			tooltips.Add(line);


			line = new TooltipLine(Mod, "AaaAngel", "(Special) The farther your cursor is, the faster the axe goes!")
			{
				OverrideColor = new Color(235, 52, 97)

			};
			tooltips.Add(line);










		}
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 10;
			Item.rare = ItemRarityID.Green;
			Item.useTime = 41;
			Item.useAnimation = 41;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.DD2_FlameburstTowerShot;

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 23;
			Item.knockBack = 5f;
			Item.noMelee = true;
			Item.crit = 21;

			// Gun Properties
			Item.shoot = ModContent.ProjectileType<PoisonedAngelProj>();
			Item.shootSpeed = 4f;
			Item.value = 10000;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(2f, -2f);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<VirulentPlating>(), 12);
			recipe.AddIngredient(ModContent.ItemType<Cinderscrap>(), 50);
			recipe.AddIngredient(ModContent.ItemType<LostScrap>(), 50);
			recipe.AddIngredient(ModContent.ItemType<SpacialDistortionFragments>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}

	}
}












