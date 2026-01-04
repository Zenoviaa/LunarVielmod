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
            int width = Main.maxTilesX / 6;

            //Get the bounds of the jungle
            int halfWidth = width / 2;
            GenVars.jungleMinX = GenVars.jungleOriginX - halfWidth;
            GenVars.jungleMaxX = GenVars.jungleOriginX + halfWidth;

            var genRand = WorldGen.genRand;

            int minY = (int)Main.worldSurface - 100;

            int darkspaceMaxY = (Main.UnderworldLayer - (Main.maxTilesY / 6));
            int darkspaceMinY = darkspaceMaxY - 12;

            int minMaxY = darkspaceMinY - 700;
            int maxMaxY = darkspaceMinY;

            int increment = 4;
            for (int x = GenVars.jungleMinX; x < GenVars.jungleMaxX; x += increment)
            {
                int jungleRange = GenVars.jungleMaxX - GenVars.jungleMinX;
                float xRatio = (float)(x - GenVars.jungleMinX) / (float)jungleRange;
                float bump = EasingFunction.QuadraticBump(xRatio);

                double strength = (double)MathHelper.Lerp(8, 16, bump);
                int steps = (int)MathHelper.Lerp(1, 8, bump);
                int maxY = (int)MathHelper.Lerp(minMaxY, maxMaxY, bump);

                for (int y = minY; y < maxY; y += increment)
                {
                    GenVars.mudWall = true;

                    int i = x;
                    int j = y;
                    int innerSteps = steps;
                    double innerStrength = strength;

                    //Just create some variation in the shape
                    //We want a large ovular shape but we also want it to not be so repetitive
                    i += genRand.Next(-10, 10);
                    j += genRand.Next(-10, 10);
                    innerSteps += genRand.Next(-3, 3);
                    innerStrength += genRand.NextDouble() * 4f;

                    WorldGen.TileRunner(i, j, innerStrength, innerSteps, TileID.Mud, false);
                    GenVars.mudWall = false;
                }
            }
        }
    }
}