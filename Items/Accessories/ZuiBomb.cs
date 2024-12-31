using Microsoft.Xna.Framework;
using Stellamod.Items.Accessories.Players;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
	 // Load the spritesheet you create as a shield for the player when it is equipped.
	public class ZuiBomb : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Steali");
			/* Tooltip.SetDefault("A small fast dash that provides invincibility as you dash" +
				"\nIncreased regeneration" +
				"\nYou may not attack while this is in use" +
				"\nHollow Knight inspiried!"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(platinum: 3);
			Item.rare = ModContent.RarityType<Helpers.GoldenSpecialRarity>();
			Item.accessory = true;



		}
	



		public override void UpdateAccessory(Player player, bool hideVisual)
		{

			player.GetModPlayer<MyPlayer>().RadiantBombCooldown--;
			player.GetModPlayer<MyPlayer>().RadiantBomb = true;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<RadiantBomb>()] == 0)
			{
				Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
					ModContent.ProjectileType<RadiantBomb>(), 10, 4, player.whoAmI);
			}
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}

}