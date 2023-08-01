
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using System;

namespace Stellamod.Items.Materials.HardMode
{

	public class StarlightEssence : ModItem
    {

        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Pure shadows conjured by the darkest of entities."); // The (English) text shown below your item's name
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Lighting.AddLight(Item.Center, Color.LightSkyBlue.ToVector3() * 1.25f * Main.essScale);
            for (int i = 0; i < 1; i++)
            {
                int num7 = 16;
                float num8 = (float)(Math.Cos(Main.GlobalTimeWrappedHourly % 2.4 / 2.4 * MathHelper.TwoPi) / 5 + 0.5);
                SpriteEffects spriteEffects = SpriteEffects.None;
                Texture2D texture = TextureAssets.Item[Item.type].Value;
                var vector2_3 = new Vector2((TextureAssets.Item[Item.type].Value.Width / 2), (TextureAssets.Item[Item.type].Value.Height / 1 / 2));
                var color2 = new Color(255, 134, 247, 150);
                Rectangle r = TextureAssets.Item[Item.type].Value.Frame(1, 1, 0, 0);
                for (int index2 = 0; index2 < num7; ++index2)
                {
                    Color color3 = Item.GetAlpha(color2) * (0.85f - num8);
                    Vector2 position2 = Item.Center + ((index2 / num7 * MathHelper.TwoPi) + rotation).ToRotationVector2() * (4.0f * num8 + 2.0f) - Main.screenPosition - new Vector2(texture.Width + 8, texture.Height) * Item.scale / 2f + vector2_3 * Item.scale;
                    Main.spriteBatch.Draw(TextureAssets.Item[Item.type].Value, position2, new Microsoft.Xna.Framework.Rectangle?(r), color3, rotation, vector2_3, Item.scale * 1.1f, spriteEffects, 0.0f);
                }
            }
            return true;
        }
        public override void SetDefaults()
        {
            Item.width = 25; // The item texture's width
            Item.height = 25; // The item texture's height
         
            Item.maxStack = 999; // The item's max stack value
            Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
		}
	}
}