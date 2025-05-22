using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
    public class EmptyMoonflameLantern : ModItem
	{
		public override void SetStaticDefaults()
		{
			/* Tooltip.SetDefault("Take it to the depths of hell, " +
				"\n Freeze the flame" +
				"\n The dance at the Cathedral awaits at the top of the moonlight"); */
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
			

			
		}
		public override void SetDefaults()
		{
			Item.width = 12;
			Item.height = 12;
			Item.maxStack = 3;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.rare = ItemRarityID.Green;	
		}
	}
}