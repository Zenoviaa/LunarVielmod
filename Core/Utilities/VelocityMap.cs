using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities
{
    public class VelocityMapPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            base.PostUpdate();
            VelocityMap velocityMap = ModContent.GetInstance<VelocityMap>();
            velocityMap.AddVelocity(Player.Center, Player.velocity);
        }
    }

    public class VelocityGlobalNPC : GlobalNPC
    {
        public override void PostAI(NPC npc)
        {
            base.PostAI(npc);
            VelocityMap velocityMap = ModContent.GetInstance<VelocityMap>();
            velocityMap.AddVelocity(npc.Center, npc.velocity);
        }
    }

    public class VelocityGlobalProjectile : GlobalProjectile
    {
        public override void PostAI(Projectile projectile)
        {
            base.PostAI(projectile);
            VelocityMap velocityMap = ModContent.GetInstance<VelocityMap>();
            velocityMap.AddVelocity(projectile.Center, projectile.velocity);
        }
    }
    /// <summary>
    /// Creates a grid of cummulative velocities of projectiles and npcs for applying external forces to various objects
    /// </summary>
    public class VelocityMap : ModSystem
    {
        private int _width;
        private int _height;
        private Vector2[,] _velocityGrid;
        public override void OnModLoad()
        {
            base.OnModLoad();
            ResizeGrid();
        }

        private void ClearGrid()
        {
            for(int x = 0; x < _width; x++)
            {
                for(int y = 0; y < _height; y++ )
                {
                    _velocityGrid[x, y] = Vector2.Zero;
                }
            }
        }

        public void AddVelocity(Vector2 worldPosition, Vector2 velocity)
        {
            //Convert this to screenspace
            Vector2 positionOnScreen = worldPosition - Main.screenPosition;

            //Convert to coordinates
            int x = (int)(positionOnScreen.X / 16);
            int y = (int)(positionOnScreen.Y / 16);

            if (x < 0 || x >= _width)
                return;
            if (y < 0 || y >= _height)
                return;
            _velocityGrid[x, y] += velocity;
        }

        public Vector2 GetVelocity(Vector2 worldPosition)
        {
            //Convert this to screenspace
            Vector2 positionOnScreen = worldPosition - Main.screenPosition;

            //Convert to coordinates
            int x = (int)(positionOnScreen.X / 16);
            int y = (int)(positionOnScreen.Y / 16);

            if (x < 0 || x >= _width)
                return Vector2.Zero;
            if (y < 0 || y >= _height)
                return Vector2.Zero;

            return _velocityGrid[x, y];
        }
        public override void PreUpdateEntities()
        {
            base.PreUpdateEntities();
            ClearGrid();
        }

        private void ResizeGrid()
        {
            //Resize the grid based on the screen width and height
            //16 pixels per
            //So it's just 1 per tile
            _width = Main.screenWidth / 16;
            _height = Main.screenHeight / 16;
            _velocityGrid = new Vector2[_width, _height];
        }
        private bool NeedsToResizeGrid()
        {
            int newWidth = Main.screenWidth / 16;
            int newHeight = Main.screenHeight / 16;
            return newWidth != _width || newHeight != _height;
        }
        public override void PostUpdateNPCs()
        {
            base.PostUpdateNPCs();
            if (NeedsToResizeGrid())
            {
                ResizeGrid();
            }
        }
    }
}
