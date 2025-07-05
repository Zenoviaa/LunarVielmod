using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Tiles
{
    internal static class TileHelper
    {
        public static Vector2 TileAdj => (Lighting.Mode == Terraria.Graphics.Light.LightMode.Retro || Lighting.Mode == Terraria.Graphics.Light.LightMode.Trippy) ? Vector2.Zero : Vector2.One * 12;
        public static void DrawInvisTile(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 pos2 = (new Vector2(i, j) + TileAdj) * 16;
            pos2 -= Main.screenPosition;
            Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Content/Tiles/InvisibleTile").Value;
            spriteBatch.Draw(texture,
              pos2,
              null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
        public static void DrawInvisTileNoAdj(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 pos2 = new Vector2(i, j) * 16;
            pos2 -= Main.screenPosition;
            Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Content/Tiles/InvisibleTile").Value;
            spriteBatch.Draw(texture,
              pos2,
              null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
    }
}
