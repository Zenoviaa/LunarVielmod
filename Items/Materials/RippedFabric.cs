
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
    internal class RippedFabric : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Ripped Fabric");
			/* Tooltip.SetDefault("From the knowledge of the Gilded to robes on enemies, wonderous!" +
				   "\nA very magical Materials although.." +
			"\nI feel like the item calls my name..."); */
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.sellPrice(silver: 2);
			Item.rare = ItemRarityID.Blue;
		}


		public override void PostUpdate()
		{
			Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Purple);
			return true;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
			return true;
		}


	}
}
