
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Shields
{
	[AutoloadEquip(EquipType.Shield)] // Load the spritesheet you create as a shield for the player when it is equipped.
	public class WoodenShield : ModItem
	{
		public override void SetStaticDefaults()
		{
			/* Tooltip.SetDefault("Super cool right??? A wooden shield?" +
				"\nWhat are you gonna block, a bow and arrow?" +
				"\nLiterally no blocking features smh" +
				"\n +1 Defense and life regen increased!"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(10);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Wood, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.lifeRegen += 1;
			player.statDefense += 1;



		}
	}
}