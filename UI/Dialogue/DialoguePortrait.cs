using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.Dialogue
{
    public class DialoguePortrait : UIElement
    {
        public virtual string Texture { get; set; } = "Stellamod/UI/Dialogue/DialoguePortrait";

        public override void Draw(SpriteBatch spriteBatch)
        {
       
            Texture2D texture2D = (Texture2D)ModContent.Request<Texture2D>(Texture);

            //Bottom Center Screen
            Vector2 drawPos = new Vector2(Main.screenWidth / 2f, Main.screenHeight);

            //Move to bottom of sceren
            drawPos -= new Vector2(0, 58);

            //Move to the left of the box
            drawPos -= new Vector2(701 / 2 / 2, 0);

            Vector2 offset = new Vector2(texture2D.Width, texture2D.Height);
            drawPos -= offset;
            spriteBatch.Draw(texture2D, drawPos, Color.White);
        }
    }
}
