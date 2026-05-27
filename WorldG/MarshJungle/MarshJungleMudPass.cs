using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;


namespace Stellamod.WorldG.MarshJungle
{
    /// <summary>
    /// Creates the mud layout for the marshy jungle, our jungle is a lot more uniform in how it spawns, so we need to redo the vanilla jungle generation
    /// </summary>
    public class MarshJungleMudPass : GenPass
    {
        public MarshJungleMudPass()
            : base("Marsh Jungle Mud", 449.3721923828125)
        {

        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Growing Jungle Mud";
            int width = 1850;

            //Get the bounds of the jungle
            int halfWidth = width / 2;
            GenVars.jungleMinX = GenVars.jungleOriginX - halfWidth;
            GenVars.jungleMaxX = GenVars.jungleOriginX + halfWidth;

            var genRand = WorldGen.genRand;

            int minY = (int)Main.worldSurface - 100;

            int darkspaceMaxY = (Main.UnderworldLayer - (Main.maxTilesY / 6));
            darkspaceMaxY -= 400;
            int darkspaceMinY = darkspaceMaxY - 12;

            int minMaxY = darkspaceMinY - 700;
            int maxMaxY = darkspaceMinY;

            for (int x = GenVars.jungleMinX; x < GenVars.jungleMaxX; x ++)
            {
                int jungleRange = GenVars.jungleMaxX - GenVars.jungleMinX;
                float xRatio = (float)(x - GenVars.jungleMinX) / (float)jungleRange;
                float bump = EasingFunction.QuadraticBump(xRatio);

                double strength = (double)MathHelper.Lerp(8, 16, bump);
                int steps = (int)MathHelper.Lerp(1, 8, bump);
                int maxY = (int)MathHelper.Lerp(minMaxY, maxMaxY, bump);

                for (int y = minY; y < maxY; y ++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile)
                    {
                        //tile.ClearTile();
                        tile.TileType = TileID.Mud;
                    }
                    //WorldGen.TileRunner(i, j, innerStrength, innerSteps, TileID.Mud, false);
                }
            }
        }
    }
}