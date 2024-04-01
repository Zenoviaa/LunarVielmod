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
        public virtual string Texture { get; set; } = "Stellamod/UI/Scripture/DialogueBox";

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture2D = (Texture2D)ModContent.Request<Texture2D>(Texture);
            Vector2 drawPos = new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
            Vector2 size = new Vector2(texture2D.Width, texture2D.Height);
            drawPos -= size / 2;
            spriteBatch.Draw(texture2D, drawPos, Color.White);
        }
    }
}