using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable.Cathedral
{
    public class AurelusTempleTile : ModItem
	{
		public override void SetStaticDefaults()
		{
			// Tooltip.SetDefault("Cathedite Block");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
			

			// Some please convert this to lang files, I'm too lazy to do it
			// Sorry Itorius, I feel you

			// DisplayName.AddTranslation(GameCulture.German, "Beispielblock");
			// Tooltip.AddTranslation(GameCulture.German, "Dies ist ein modded Block");
			// DisplayName.AddTranslation(GameCulture.Italian, "Blocco di esempio");
			// Tooltip.AddTranslation(GameCulture.Italian, "Questo è un blocco moddato");
			// DisplayName.AddTranslation(GameCulture.French, "Bloc d'exemple");
			// Tooltip.AddTranslation(GameCulture.French, "C'est un bloc modgé");
			// DisplayName.AddTranslation(GameCulture.Spanish, "Bloque de ejemplo");
			// Tooltip.AddTranslation(GameCulture.Spanish, "Este es un bloque modded");
			// DisplayName.AddTranslation(GameCulture.Russian, "Блок примера");
			// Tooltip.AddTranslation(GameCulture.Russian, "Это модифицированный блок");
			// DisplayName.AddTranslation(GameCulture.Chinese, "例子块");
			// Tooltip.AddTranslation(GameCulture.Chinese, "这是一个修改块");
			// DisplayName.AddTranslation(GameCulture.Portuguese, "Bloco de exemplo");
			// Tooltip.AddTranslation(GameCulture.Portuguese, "Este é um bloco modded");
			// DisplayName.AddTranslation(GameCulture.Polish, "Przykładowy blok");
			// Tooltip.AddTranslation(GameCulture.Polish, "Jest to modded blok");
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
			Item.consumable = true;
			Item.createTile = ModContent.TileType<Tiles.Abyss.Aurelus.AurelusTempleBlock>();
		}
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		
	}
}