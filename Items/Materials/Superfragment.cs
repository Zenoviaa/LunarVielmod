
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
    public class Superfragment : ModItem
	{
		public override void SetStaticDefaults()
		{
			// Some please convert this to lang files, I'm too lazy to do it
			// Sorry Itorius, I feel you
			// DisplayName.AddTranslation(GameCulture.Polish, "Przykładowy blok");
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(1, 60));

			// Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
			ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
		}
		public override void SetDefaults()
		{
			Item.width = 12;
			Item.height = 12;
			Item.maxStack = Item.CommonMaxStack;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 10;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.rare = ItemRarityID.LightRed;
			Item.buyPrice(0, 0, 95, 0);
			Item.value = 2500;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}
}