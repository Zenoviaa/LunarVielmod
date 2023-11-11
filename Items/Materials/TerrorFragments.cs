using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
    public class TerrorFragments : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Terror Fragment");
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Orange;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Lighting.AddLight(new Vector2(Item.Center.X, Item.Center.Y), 81 * 0.001f, 194 * 0.001f, 58 * 0.001f);
            for (int i = 0; i < 1; i++)
            {
                int num7 = 16;
                float num8 = (float)(Math.Cos(Main.GlobalTimeWrappedHourly % 2.4 / 2.4 * MathHelper.TwoPi) / 5 + 0.5);
                SpriteEffects spriteEffects = SpriteEffects.None;
                Texture2D texture = TextureAssets.Item[Item.type].Value;
                var vector2_3 = new Vector2((TextureAssets.Item[Item.type].Value.Width / 2), (TextureAssets.Item[Item.type].Value.Height / 1 / 2));
                var color2 = new Color(255, 8, 55, 150);
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
    }
}