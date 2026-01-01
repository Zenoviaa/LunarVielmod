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
        public override void ClearWorld()
        {
            base.ClearWorld();
            _velocityGrid = new Vector2[Main.maxTilesX, Main.maxTilesY];
            _decayingVelocityGrid = new Vector2[Main.maxTilesX, Main.maxTilesY];
        }

        private void ClearGrid()
        {
            //We're only going to update the velocities for what's on screen, for optimization concerns
            Point startTile = (Main.Camera.Center - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2f)).ToTileCoordinates();
            if (startTile.X < 0)
                startTile.X = 0;
            if (startTile.Y < 0)
                startTile.Y = 0;

            Point endTile = startTile + new Point(Main.screenWidth/16, Main.screenHeight/16);
            if (endTile.X >= Main.maxTilesX)
                endTile.X = Main.maxTilesX - 1;
            if(endTile.Y  >= Main.maxTilesY)
                endTile.Y = Main.maxTilesY - 1;
            for (int x = startTile.X; x < endTile.X; x++)
            {
                for (int y = startTile.Y; y < endTile.Y; y++)
                {
                    _velocityGrid[x, y] = Vector2.Zero;
                    _decayingVelocityGrid[x, y] *= 0.94f;
                }
            }
        }

        public void AddVelocity(Vector2 worldPosition, Vector2 velocity)
        {
            //Convert this to tile space
            Point tile = worldPosition.ToTileCoordinates();
            if (tile.X < 0 || tile.X >= Main.maxTilesX)
                return;
            if (tile.Y < 0 || tile.Y >= Main.maxTilesY)
                return;
            _velocityGrid[tile.X, tile.Y] += velocity;
            _decayingVelocityGrid[tile.X, tile.Y] += velocity;
        }

        public Vector2 GetVelocity(Vector2 worldPosition)
        {
            //Convert this to screenspace
            Point tile = worldPosition.ToTileCoordinates();
            if (tile.X < 0 || tile.X >= Main.maxTilesX)
                return Vector2.Zero;
            if (tile.Y < 0 || tile.Y >= Main.maxTilesY)
                return Vector2.Zero;
            return _velocityGrid[tile.X, tile.Y];
        }
        public Vector2 GetVelocity(int tileX, int tileY)
        {

            if (tileX < 0 || tileX >= Main.maxTilesX)
                return Vector2.Zero;
            if (tileY < 0 || tileY >= Main.maxTilesY)
                return Vector2.Zero;

            return _velocityGrid[tileX, tileY];
        }

        public Vector2 GetDecayingVelocity(Vector2 worldPosition)
        {
            //Convert this to screenspace
            Point tile = worldPosition.ToTileCoordinates();
            if (tile.X < 0 || tile.X >= Main.maxTilesX)
                return Vector2.Zero;
            if (tile.Y < 0 || tile.Y >= Main.maxTilesY)
                return Vector2.Zero;
            return _decayingVelocityGrid[tile.X, tile.Y];
        }

        public Vector2 GetDecayingVelocity(int tileX, int tileY)
        {
            if (tileX < 0 || tileX >= Main.maxTilesX)
                return Vector2.Zero;
            if (tileY < 0 || tileY >= Main.maxTilesY)
                return Vector2.Zero;
            return _decayingVelocityGrid[tileX, tileY];
        }

        public Vector2 GetDecayingVelocity(Vector2 worldPosition, int width, int height)
        {
            //Convert this to screenspace

            Point tile = worldPosition.ToTileCoordinates();
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
