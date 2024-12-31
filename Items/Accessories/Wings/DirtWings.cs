using Stellamod.Items.Materials;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
	public class DirtWings : ModItem
	{
		public override void SetStaticDefaults()
		{
			// These wings use the same values as the solar wings
			// Fly time: 180 ticks = 3 seconds
			// Fly speed: 9
			// Acceleration multiplier: 2.5
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(4, 11f, 1.9f);
		}

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 20;
			Item.value = 1;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
			ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			ascentWhenFalling = 0.85f; // Falling glide speed
			ascentWhenRising = 0.15f; // Rising speed
			maxCanAscendMultiplier = 1f;
			maxAscentMultiplier = 3f;
			constantAscend = 0.135f;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.DirtBlock, 999)
				.AddIngredient(ItemID.SoulofFlight, 20)
				.AddTile(TileID.MythrilAnvil)
				.SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1)) // Places this recipe before any wing so every wing stays together in the crafting menu.
				.Register();
		}
	}
}
