using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Helpers;
using Stellamod.Core.UI;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Map.UI
{
    internal class MapMarker : SimpleUIBackground
    {
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle rectangle = GetDimensions().ToRectangle();

            //Draw Backing
            Color color2 = Color.White;
            Vector2 pos = rectangle.TopLeft();
            Texture2D shadowTexture = ModContent.Request<Texture2D>(Texture + "_Shadow").Value;

            Vector2 shadowPos = pos + new Vector2(3, 20);
            // spriteBatch.Draw(shadowTexture, shadowPos, null, color2, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);


            pos.Y += VectorHelper.Osc(0f, -8f, speed: 3);
            spriteBatch.Draw(TextureAsset.Value, pos, null, color2, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
        }
    }
}
