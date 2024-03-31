using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.Scripture
{
    public class ScripturePopup : UIElement
    {
        public virtual string Texture { get; set; } = "Stellamod/UI/Scripture/ExampleScripture";
   
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
