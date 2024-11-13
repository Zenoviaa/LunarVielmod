using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class IgniterTooltipDrop : GlobalItem
    {
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (line.Mod == "Stellamod" && line.Name.Contains("Powder_"))
            {
                line.BaseScale *= 0.8f;
                line.X += 30;
                line.Y += 6;
            }

            return base.PreDrawTooltipLine(item, line, ref yOffset);
        }



        public override void PostDrawTooltipLine(Item item, DrawableTooltipLine line)
        {
            base.PostDrawTooltipLine(item, line);


            if (line.Mod == "Stellamod" && line.Name.Contains("Powder_"))
            {

                int startIndex = line.Name.IndexOf("_") + 1;
                int endIndex = line.Name.LastIndexOf("_");
                string textureName = line.Name.Substring(startIndex, endIndex - startIndex);
                Texture2D texture = ModContent.Request<Texture2D>(textureName).Value;

                SpriteBatch spriteBatch = Main.spriteBatch;
                Vector2 textPosition = new(line.X, line.Y);
                Vector2 drawPos = textPosition + new Vector2(0, texture.Size().Y / 3.5f) - new Vector2(15, 6);
                spriteBatch.Draw(texture, drawPos, null, Color.White, 0f, texture.Size() * 0.5f, 0.8f, SpriteEffects.None, 0f);

            }
        }
    }
}
