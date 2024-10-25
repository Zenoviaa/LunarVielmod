using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private Dictionary<Point, IDrawBehindWall> _drawIndex;
        public static Vector2 TileAdj => (Lighting.Mode == Terraria.Graphics.Light.LightMode.Retro || Lighting.Mode == Terraria.Graphics.Light.LightMode.Trippy) ? Vector2.Zero : Vector2.One * 12;
        public override void Load()
        {
            base.Load();
            _drawIndex = new Dictionary<Point, IDrawBehindWall>();
            On_Main.DoDraw_WallsAndBlacks += DrawBehindWalls;
        }
        public override void Unload()
        {
            base.Unload();
            On_Main.DoDraw_WallsAndBlacks -= DrawBehindWalls;
        }

        private bool IsOnScreen(Point point)
        {
            Point a = point;
            Point drawPos = a;
            drawPos.X *= 16;
            drawPos.Y *= 16;

            Rectangle screenRect = new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2);
            screenRect.Location = new Point((int)Main.screenPosition.X, (int)Main.screenPosition.Y);
            return screenRect.Contains(drawPos);
        }

        public void AddDraw(int i, int j, IDrawBehindWall drawBehindWall)
        {
            Point point = new Point(i, j);
            if (_drawIndex.ContainsKey(point))
                return;
            _drawIndex.Add(point, drawBehindWall);
        }

        private void DrawBehindWalls(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self)
        {
            //Draw Behind the walls

            Main.tileBatch.Begin();
            //Filter out things that aren't on screen anymore
            _drawIndex = _drawIndex.Where(x => IsOnScreen(x.Key)).ToDictionary();
            foreach(var kvp in _drawIndex)
            {
                var func = kvp.Value;
                var point = kvp.Key;
                func.DrawBehindWall(point.X, point.Y, Main.spriteBatch);
            }
            Main.tileBatch.End();
            orig(self);
        }
    }
}
