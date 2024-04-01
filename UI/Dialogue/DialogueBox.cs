using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;

namespace Stellamod.UI.Dialogue
{
    internal class DialogueBox : UIElement
    {
        public virtual string Texture { get; set; } = "Stellamod/UI/Dialogue/DialogueBox";

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture2D = (Texture2D)ModContent.Request<Texture2D>(Texture);

            //Bottom Center Screen
            Vector2 drawPos = new Vector2(Main.screenWidth / 2f, Main.screenHeight);

            //Move to bottom of sceren
            drawPos -= new Vector2(0, 32);

            Vector2 offset = new Vector2(texture2D.Width / 2, texture2D.Height);
            drawPos -= offset;
            spriteBatch.Draw(texture2D, drawPos, Color.White);
        }
    }
}