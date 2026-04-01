using Microsoft.Xna.Framework;
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
        private Vector2[,] _velocityGrid;
        private Vector2[,] _decayingVelocityGrid;
        private int _width;
        private int _height;
        public override void ClearWorld()
        {
            base.ClearWorld();
            _width = (int)(Main.screenWidth * 1.5f);
            _height = (int)(Main.screenHeight * 1.5f);
            _width /= 16;
            _height /= 16;
            _velocityGrid = new Vector2[_width, _height];
            _decayingVelocityGrid = new Vector2[_width, _height];
        }

        private Point GetTileInScreenSpace(Vector2 worldPosition)
        {
            Point worldTile = worldPosition.ToTileCoordinates();
            Point topLeft = Main.screenPosition.ToTileCoordinates();
            worldTile -= topLeft;
           
            return worldTile;
        }
        private void ClearGrid()
        {
            //We're only going to update the velocities for what's on screen, for optimization concerns
            for(int i = 0; i < _width; i++)
            {
                for(int j = 0; j < _height; j++)
                {
                    _velocityGrid[i, j] = Vector2.Zero;
                    _decayingVelocityGrid[i, j] *= 0.7f;
                }
            }
            /*
            Point startTile = (Main.Camera.Center - new Vector2(_width / 2, _height / 2f)).ToTileCoordinates();
            if (startTile.X < 0)
                startTile.X = 0;
            if (startTile.Y < 0)
                startTile.Y = 0;

            Point endTile = (Main.Camera.Center + new Vector2(_width / 2, _height / 2f)).ToTileCoordinates();
            if (endTile.X >= Main.maxTilesX)
                endTile.X = Main.maxTilesX - 1;
            if(endTile.Y  >= Main.maxTilesY)
                endTile.Y = Main.maxTilesY - 1;
            for (int x = startTile.X; x < endTile.X; x++)
            {
                for (int y = startTile.Y; y < endTile.Y; y++)
                {
                    _velocityGrid[x, y] = Vector2.Zero;
                    _decayingVelocityGrid[x, y] *= 0.9f;
                }
            }*/
        }

        public void AddVelocity(Vector2 worldPosition, Vector2 velocity)
        {
            //Convert this to tile space
            Point tile = GetTileInScreenSpace(worldPosition);
           // Main.NewText(tile);
            if (tile.X < 0 || tile.X >= _width)
                return;
            if (tile.Y < 0 || tile.Y >=_height)
                return;

            _velocityGrid[tile.X, tile.Y] += velocity;
            _decayingVelocityGrid[tile.X, tile.Y] += velocity;
        }

        public Vector2 GetVelocity(Vector2 worldPosition)
        {
            //Convert this to screenspace
            Point tile = GetTileInScreenSpace(worldPosition);


            if (tile.X < 0 || tile.X >= _width)
                return Vector2.Zero;
            if (tile.Y < 0 || tile.Y >= _height)
                return Vector2.Zero;
            return _velocityGrid[tile.X, tile.Y];
        }


        public Vector2 GetDecayingVelocity(int tileX, int tileY)
        {
            if (tileX < 0 || tileX >= _width)
                return Vector2.Zero;
            if (tileY < 0 || tileY >= _height)
                return Vector2.Zero;
            return _decayingVelocityGrid[tileX, tileY];
        }

        public Vector2 GetDecayingVelocity(Vector2 worldPosition, int width, int height)
        {
            //Convert this to screenspace

            Point tile = GetTileInScreenSpace(worldPosition);



            width /= 16;
            height /= 16;

            Vector2 cummulativeVelocity = Vector2.Zero;
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    cummulativeVelocity += GetDecayingVelocity(tile.X + w,tile.Y + h);
                }
            }
            return cummulativeVelocity;
        }
        public override void PreUpdateEntities()
        {
            base.PreUpdateEntities();
            ClearGrid();
        }
    }
}
