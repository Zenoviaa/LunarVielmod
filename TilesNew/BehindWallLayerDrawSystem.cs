using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.TilesNew
{
    internal interface IDrawBehindWall
    {
        void DrawBehindWall(int i, int j, SpriteBatch spriteBatch);
    }


    internal class BehindWallLayerDrawSystem : ModSystem
    {
        public static Vector2 TileAdj => (Lighting.Mode == Terraria.Graphics.Light.LightMode.Retro || Lighting.Mode == Terraria.Graphics.Light.LightMode.Trippy) ? Vector2.Zero : Vector2.One * 12;

        private int TileDrawWidth => Main.screenWidth * 2 / 16;
        private int TileDrawHeight => Main.screenHeight * 2 / 16;
        public override void Load()
        {
            base.Load();
            On_Main.DoDraw_WallsAndBlacks += DrawWalls;
        }
        public override void Unload()
        {
            base.Unload();
            On_Main.DoDraw_WallsAndBlacks -= DrawWalls;
        }

        private void DrawWalls(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self)
        {
            //Draw Behind the walls
            DrawBehindWalls();
            orig(self);
            DrawDecorativeWalls();
        }

        private void DrawBehindWalls()
        {
            Main.tileBatch.Begin();
            int width = TileDrawWidth;
            int height = TileDrawHeight;
            Point bottomLeft = Main.LocalPlayer.Center.ToTileCoordinates() - new Point(width / 2, height / 2);
            int left = bottomLeft.X;
            int right = left + width;
            int bottom = bottomLeft.Y;
            int top = bottom + height;
            for (int x = left; x < right; x++)
            {
                if (x < 0)
                    continue;
                if (x >= Main.maxTilesX)
                    continue;
                for (int y = bottom; y < top; y++)
                {
                    if (y >= Main.maxTilesY)
                        continue;
                    if (y < 0)
                        continue;

                    Tile tile = Main.tile[x, y];
                    if (tile.WallType == WallID.None)
                        continue;
                    ModWall modWall = ModContent.GetModWall(tile.WallType);
                    if (modWall == null)
                        continue;
                    if (modWall is BehindDecorativeWall decorativeWall)
                    {
                        decorativeWall.DrawDecor(x, y, Main.spriteBatch);
                    }
                }
            }
            Main.tileBatch.End();
        }

        private void DrawDecorativeWalls()
        {

            //Draw In Front Walls
            Main.tileBatch.Begin();
            int width = TileDrawWidth;
            int height = TileDrawHeight;
            Point bottomLeft = Main.LocalPlayer.Center.ToTileCoordinates() - new Point(width / 2, height / 2);
            int left = bottomLeft.X;
            int right = left + width;
            int bottom = bottomLeft.Y;
            int top = bottom + height;
            for (int x = left; x < right; x++)
            {
                if (x < 0)
                    continue;
                if (x >= Main.maxTilesX)
                    continue;
                for (int y = bottom; y < top; y++)
                {
                    if (y >= Main.maxTilesY)
                        continue;
                    if (y < 0)
                        continue;

                    Tile tile = Main.tile[x, y];
                    if (tile.WallType == WallID.None)
                        continue;
                    ModWall modWall = ModContent.GetModWall(tile.WallType);
                    if (modWall == null)
                        continue;
                    if (modWall is DecorativeWall decorativeWall)
                    {
                        decorativeWall.DrawDecor(x, y, Main.spriteBatch);
                    }
                }
            }
            Main.tileBatch.End();
        }
    }
}
