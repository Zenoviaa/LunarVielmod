using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static tModPorter.ProgressUpdate;

namespace Stellamod.WorldG
{
    internal class VeilGenTester : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.useTime = 1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 1;
        }

        public override bool? UseItem(Player player)
        {
            GenerateLongCurveCave();
            return base.UseItem(player);
        }
        private void GenerateLongCurveCave()
        {
            Vector2 mouseWorld = Main.MouseWorld;
            int x = (int)Main.MouseWorld.X / 16;
            int y = (int)Main.MouseWorld.Y / 16;
            int caveWidth = 5;
            int caveSteps = 1000;

            //Cave position in tiles
            Vector2 cavePosition = new Vector2(x, y);

            //Starting cave direction
            Vector2 baseCaveDirection = Vector2.UnitX;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

            //How much the tile runner is gonna carve out
            Vector2 caveStrength = new Vector2(12, 14);

            //Chance to open up
            int splitDenominator = 4;
            VeilGen.GenerateLongCurveCave(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
        }
        private void GenerateTreeCaves()
        {
            Vector2 mouseWorld = Main.MouseWorld;
            int x = (int)Main.MouseWorld.X / 16;
            int y = (int)Main.MouseWorld.Y / 16;
            int caveWidth =5;
            int caveSteps =50;

            //Cave position in tiles
            Vector2 cavePosition = new Vector2(x, y);

            //Starting cave direction
            Vector2 baseCaveDirection = Vector2.UnitY;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

            //How much the tile runner is gonna carve out
            Vector2 caveStrength = new Vector2(12, 14);

            //Chance to open up
            int splitDenominator = 4;
            VeilGen.GenerateTreeCaves(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps,
                splitDenominator);
        }

        private void GenerateOpenCaveClearing()
        {
            Vector2 mouseWorld = Main.MouseWorld;
            int tileX = (int)Main.MouseWorld.X / 16;
            int tileY = (int)Main.MouseWorld.Y / 16;
            int caveWidth = 15;
            int caveSteps = 500;

            //Cave position in tiles
            Vector2 cavePosition = new Vector2(tileX, tileY);

            //Starting cave direction
            Vector2 baseCaveDirection = Vector2.UnitX;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

            //How much the tile runner is gonna carve out
            Vector2 caveStrength = new Vector2(20, 25);

            VeilGen.GenerateOpenCaveClearing(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
        }

        private void GenerateLongNoodleCave()
        {
            Vector2 mouseWorld = Main.MouseWorld;
            int tileX = (int)Main.MouseWorld.X / 16;
            int tileY = (int)Main.MouseWorld.Y / 16;
            int caveWidth = 5;
            int caveSteps = 500;

            //Cave position in tiles
            Vector2 cavePosition = new Vector2(tileX, tileY);

            //Starting cave direction
            Vector2 baseCaveDirection = Vector2.UnitX;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

            //How much the tile runner is gonna carve out
            Vector2 caveStrength = new Vector2(4, 5);

            VeilGen.GenerateLongNoodleCave(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
        }


        private void GenerateWiggleCave()
        {
            Vector2 mouseWorld = Main.MouseWorld;
            int tileX = (int)Main.MouseWorld.X / 16;
            int tileY = (int)Main.MouseWorld.Y / 16;
            int caveWidth = 5;
            int caveSteps = 120;

            //Cave position in tiles
            Vector2 cavePosition = new Vector2(tileX, tileY);

            //Starting cave direction
            Vector2 baseCaveDirection = Vector2.UnitY;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

            //How much the tile runner is gonna carve out
            Vector2 caveStrength = new Vector2(4, 5);

            VeilGen.GenerateWiggleCave(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
        }

        private void GenerateLinearCave()
        {
            Vector2 mouseWorld = Main.MouseWorld;
            int tileX = (int)Main.MouseWorld.X / 16;
            int tileY = (int)Main.MouseWorld.Y / 16;
            int caveWidth = 5;
            int caveSteps = 25;

            //Cave position in tiles
            Vector2 cavePosition = new Vector2(tileX, tileY);

            //Starting cave direction
            Vector2 baseCaveDirection = Vector2.UnitY;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

            //How much the tile runner is gonna carve out
            Vector2 caveStrength = new Vector2(9, 10);

            VeilGen.GenerateLinearCave(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
        }

        private void GenerateNoodleCave()
        {
            Vector2 mouseWorld = Main.MouseWorld;
            int tileX = (int)Main.MouseWorld.X / 16;
            int tileY = (int)Main.MouseWorld.Y / 16;
            int caveWidth = 4;
            int caveSteps = 250;

            //Cave position in tiles
            Vector2 cavePosition = new Vector2(tileX, tileY);

            //Starting cave direction
            Vector2 baseCaveDirection = Vector2.UnitY;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

            //How much the tile runner is gonna carve out
            Vector2 caveStrength = new Vector2(10, 20);

            VeilGen.GenerateNoodleCave(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
        }

        private void GenerateWormCave()
        {
            Vector2 mouseWorld = Main.MouseWorld;
            int tileX = (int)Main.MouseWorld.X / 16;
            int tileY = (int)Main.MouseWorld.Y / 16;
            int caveWidth = 4;
            int caveSteps = 64;
            Vector2 cavePosition = new Vector2(tileX, tileY);
            Vector2 caveStrength = new Vector2(5, 10);
            Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(WorldGen.genRand.NextFloatDirection());
            VeilGen.GenerateWormCave(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
        }
        private void GenerateVeinyCave()
        {
            Vector2 mouseWorld = Main.MouseWorld;
            int tileX = (int)Main.MouseWorld.X / 16;
            int tileY = (int)Main.MouseWorld.Y / 16;
            int caveWidth = 12;
            int caveSteps = 250;
            Vector2 cavePosition = new Vector2(tileX, tileY);
            Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);
            Vector2 caveStrength = new Vector2(4, 5);
            VeilGen.GenerateVeinyCaves( cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
        }
    }

    internal static class VeilGen
    {
        public static void GenerateFigure8Cave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;
            Vector2 pullDirection;
            pullDirection.X = -baseCaveDirection.X;
            pullDirection.Y = 1;

            Vector2 targetPosition = caveVelocity + pullDirection;
            Vector2 startPullDirection = pullDirection;
            float sharpness = 3f;
            float counter = 0;
            float target = 100;
            for (int j = 0; j < caveSteps; j++)
            {
                //Homing
                float degreesToRotate = sharpness;
                float length = caveVelocity.Length();
                float targetAngle = (targetPosition - cavePosition).ToRotation();
                Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
                caveVelocity = newVelocity;
                if (counter < 20f)
                {
                    pullDirection = Vector2.Lerp(startPullDirection, Vector2.Zero, counter / 20f);
                    targetPosition = cavePosition + pullDirection;
                    counter++;
                }

                if (counter > target)
                {
                    target = genRand.Next(100, 150);
                    targetPosition.X = -targetPosition.X;
                    startPullDirection = pullDirection;
                    counter = 0;
                }

                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    /*
                    //digging 
                    ShapeData shapeData = new ShapeData();
                    Point point = new Point((int)cavePosition.X, (int)cavePosition.Y);
                    WorldUtils.Gen(point, new Shapes.Circle(3, 3), new Actions.ClearTile());
                    */

                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(4, 5), -1);
                }

                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
                //  caveStrength *= 0.99f;
            }
        }

        public static void GenerateLongCurveCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;
            Vector2 breakStrength = caveStrength;
            Vector2 pullDirection;
            pullDirection.X = -baseCaveDirection.X;
            pullDirection.Y = 1;

            Vector2 startVelocity = caveVelocity;
            Vector2 pullVelocity = caveVelocity;

            float sharpness = 10;
            float counter = 0;
            float target = genRand.Next(50, 200);
            float direction = 1;
            for (int j = 0; j < caveSteps; j++)
            {

                counter++;
                breakStrength *= 0.9995f;
                float degreesToRotate = sharpness;
                float length = caveVelocity.Length();
                float targetAngle = (pullVelocity - startVelocity).ToRotation();
                Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle,
                    MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
                caveVelocity = newVelocity;


                if (counter > target)
                {
                    target = genRand.Next(50, 200);
                    float mult = direction % 2 == 0 ? 1 : 0;
                    pullVelocity = startVelocity.RotatedBy(MathHelper.ToRadians(-180 * mult));
                    direction++;
                    counter = 0;
                }

                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    /*
                    //digging 
                    ShapeData shapeData = new ShapeData();
                    Point point = new Point((int)cavePosition.X, (int)cavePosition.Y);
                    WorldUtils.Gen(point, new Shapes.Circle(3, 3), new Actions.ClearTile());
                    */

                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(breakStrength.X, breakStrength.Y),
                        genRand.Next(4, 5), -1);
                }

                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
                //  caveStrength *= 0.99f;
            }
        }

        public static void GenerateFishCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            FastNoiseLite fastNoiseLite = new FastNoiseLite();
            fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            fastNoiseLite.SetSeed(caveSeed);

            float i = cavePosition.X;
            for (int j = 0; j < caveSteps; j++)
            {
                float divisor = 1f;

                //1. Have Position

                //The default direction

                Vector2 caveDirection = baseCaveDirection;


                //Sample the noise
                float sample = fastNoiseLite.GetNoise(cavePosition.X, j / 50f);
                float caveOffsetAngleAtStep = sample * MathHelper.ToRadians(90);


                //Rotate based on the noise
                caveDirection = caveDirection.RotatedBy(caveOffsetAngleAtStep);

                // Carve out at the current position.
                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    //digging 
                    ShapeData shapeData = new ShapeData();
                    Point point = new Point((int)cavePosition.X, (int)cavePosition.Y);
                    WorldUtils.Gen(point, new Shapes.Circle(3, 3), new Actions.ClearTile());

                    /*WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(4, 5), -1);*/
                }

                // Update the cave position.
                cavePosition += caveDirection * caveWidth * 0.5f;
            }
        }

        public static void GenerateTreeCaves(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength,
            int caveWidth,
            int caveSteps,
            int splitDenominator)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;
            Vector2 pullDirection = genRand.NextVector2Circular(1, 1);
            Vector2 targetPosition = caveVelocity + pullDirection;
            float sharpness = 1;
            int counter = 1;
            for (int j = 0; j < caveSteps; j++)
            {
                //Homing
                float degreesToRotate = sharpness;
                float length = caveVelocity.Length();
                float targetAngle = (targetPosition - caveVelocity).ToRotation();
                Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
                caveVelocity = newVelocity;


                if (genRand.NextBool(3))
                {
                    targetPosition = targetPosition.RotatedByRandom(MathHelper.ToRadians(30));
                }

                if (genRand.NextBool(splitDenominator) && j > 4)
                {
                    int clearingCaveWidth = caveWidth / 2;
                    int clearingCaveSteps = caveSteps;

                    //Cave position in tiles
                    Vector2 clearingPosition = new Vector2((int)cavePosition.X, (int)cavePosition.Y);

                    //Starting cave direction
                    float dir = counter % 2 == 0 ? 1 : -1;
                    counter++;
                    Vector2 clearingCaveDirection = baseCaveDirection.RotatedBy(dir * MathHelper.PiOver2);

                    //How much the tile runner is gonna carve out
                    Vector2 clearingCaveStrength = caveStrength * 0.5f;

                    VeilGen.GenerateTreeCaves(clearingPosition,
                        clearingCaveDirection,
                        clearingCaveStrength,
                        clearingCaveWidth,
                        clearingCaveSteps, 
                        splitDenominator * 640);
                }

                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    /*
                    //digging 
                    ShapeData shapeData = new ShapeData();
                    Point point = new Point((int)cavePosition.X, (int)cavePosition.Y);
                    WorldUtils.Gen(point, new Shapes.Circle(3, 3), new Actions.ClearTile());
                    */
          
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(4, 5), -1);
                }

                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
              //  caveStrength *= 0.99f;
            }
        }

        public static void GenerateStraightCaves(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength,
            int caveWidth,
            int caveSteps)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;
            Vector2 pullDirection = genRand.NextVector2Circular(1, 1);
            Vector2 targetPosition = caveVelocity + pullDirection;
            float sharpness = 1;
            for (int j = 0; j < caveSteps; j++)
            {
                //Homing
                float degreesToRotate = sharpness;
                float length = caveVelocity.Length();
                float targetAngle = (targetPosition - caveVelocity).ToRotation();
                Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
                caveVelocity = newVelocity;


                if (genRand.NextBool(3))
                {
                    pullDirection = genRand.NextVector2Circular(1, 1);
                    targetPosition = -targetPosition;
                }

                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    /*
                    //digging 
                    ShapeData shapeData = new ShapeData();
                
                    */
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(4, 5), -1);
                }

                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
            }
        }

        public static void GenerateHighCaves(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, 
            int caveWidth,
            int caveSteps,
            int clearingDenominator)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;
            Vector2 pullDirection = genRand.NextVector2Circular(1, 1);
            Vector2 targetPosition = caveVelocity + pullDirection;
            float sharpness = 9;
      
            for (int j = 0; j < caveSteps; j++)
            {
                //Homing
                float degreesToRotate = sharpness;
                float length = caveVelocity.Length();
                float targetAngle = (targetPosition - caveVelocity).ToRotation();
                Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
                caveVelocity = newVelocity;


                if (genRand.NextBool(3))
                {
                    pullDirection = genRand.NextVector2Circular(1, 1);
                    targetPosition = -targetPosition;
                }

                if(genRand.NextBool(clearingDenominator) && j > caveSteps / 2)
                {
                    int clearingCaveWidth = 15;
                    int clearingCaveSteps = 500;

                    //Cave position in tiles
                    Vector2 clearingPosition = new Vector2((int)cavePosition.X, (int)cavePosition.Y);

                    //Starting cave direction
                    Vector2 clearingCaveDirection = caveVelocity;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

                    //How much the tile runner is gonna carve out
                    Vector2 clearingCaveStrength = new Vector2(20, 25);

                    VeilGen.GenerateOpenCaveClearing(clearingPosition,
                        clearingCaveDirection,
                        clearingCaveStrength,
                        clearingCaveWidth,
                        clearingCaveSteps);
                }

                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    /*
                    //digging 
                    ShapeData shapeData = new ShapeData();
                
                    */
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(4, 5), -1);

                }

                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
            }
        }


        public static void GenerateOpenCaveClearing(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            FastNoiseLite fastNoiseLite = new FastNoiseLite();
            fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            fastNoiseLite.SetSeed(caveSeed);

            Vector2 caveVelocity = baseCaveDirection;
            Vector2 baseCavePosition = cavePosition;
            for (int j = 0; j < caveSteps; j++)
            {
                if (genRand.NextBool(4))
                {
                    caveVelocity = Main.rand.NextVector2Circular(1, 1);
                }
                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    //digging 
                   // ShapeData shapeData = new ShapeData();
                   // Point point = new Point((int)cavePosition.X, (int)cavePosition.Y);
                   // WorldUtils.Gen(point, new Shapes.Circle(3, 3), new Actions.ClearTile());

                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(8, 9), -1);
                }

                // Update the cave position.
                cavePosition = baseCavePosition + caveVelocity * caveWidth;
            }
        }
       
        public static void GenerateLongNoodleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            FastNoiseLite fastNoiseLite = new FastNoiseLite();
            fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            fastNoiseLite.SetSeed(caveSeed);

            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;
            Vector2 pullDirection = genRand.NextVector2Circular(1, 1);
            Vector2 targetPosition = caveVelocity + pullDirection;
            float sharpness = 9;
            for (int j = 0; j < caveSteps; j++)
            {
                float divisor = 1f;

                //1. Have Position
                //  caveDirection = Vector2.Lerp(caveDirection, pullDirection, 0.05f);


                //Homing
                float degreesToRotate = sharpness;
                float length = caveVelocity.Length();
                float targetAngle = (targetPosition - caveVelocity).ToRotation();
                Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
                caveVelocity = newVelocity;


                if (genRand.NextBool(3))
                {
                    pullDirection = genRand.NextVector2Circular(1, 1);
                    targetPosition = -targetPosition;
                }

                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    //digging 
                    ShapeData shapeData = new ShapeData();
                    Point point = new Point((int)cavePosition.X, (int)cavePosition.Y);
                    WorldUtils.Gen(point, new Shapes.Circle(3, 3), new Actions.ClearTile());

                    /*WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(4, 5), -1);*/
                }

                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
            }
        }


        public static void GenerateVeinyCaves(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            FastNoiseLite fastNoiseLite = new FastNoiseLite();
            fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            fastNoiseLite.SetSeed(caveSeed);

            for (int j = 0; j < caveSteps; j++)
            {
                float divisor = 2f;
                float sample = fastNoiseLite.GetNoise((float)cavePosition.X / divisor, (float)cavePosition.Y / divisor);
                float caveOffsetAngleAtStep = sample * MathHelper.TwoPi * 1.9f;
                Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

                // Carve out at the current position.
                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    //digging 
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y, MathF.Sin(j * 0.05f) * 10 + 
                        genRand.NextFloat(2, 5), 
                        genRand.Next(5, 10), -1);
                }

                // Update the cave position.
                cavePosition += caveDirection * caveWidth * 0.5f;
            }
        }
        public static void GenerateLinearCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps )
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            FastNoiseLite fastNoiseLite = new FastNoiseLite();
            fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            fastNoiseLite.SetSeed(caveSeed);

            for (int j = 0; j < caveSteps; j++)
            {
                float divisor = 50f;
                float sample = fastNoiseLite.GetNoise((float)cavePosition.X / divisor, (float)cavePosition.Y / divisor);
                float caveOffsetAngleAtStep = sample * MathHelper.TwoPi * 1.9f;
                Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

                // Carve out at the current position.
                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    //digging 
                    WorldGen.TileRunner(
                        (int)cavePosition.X,
                        (int)cavePosition.Y,
                        genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next((int)caveStrength.X, (int)caveStrength.Y),
                        type: -1);
                }

                // Update the cave position.
                cavePosition += caveDirection * caveWidth * 0.5f;
            }
        }
        public static void GenerateWiggleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            FastNoiseLite fastNoiseLite = new FastNoiseLite();
            fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            fastNoiseLite.SetSeed(caveSeed);

            for (int j = 0; j < caveSteps; j++)
            {
                float divisor = 2f;
                float sample = fastNoiseLite.GetNoise((float)cavePosition.X / divisor, (float)cavePosition.Y / divisor);
                sample = MathF.Sin(sample * 8);
                float caveOffsetAngleAtStep = sample * MathHelper.TwoPi * 1.9f;
                Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

                // Carve out at the current position.
                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15 && sample > 0)
                {
                    //digging 
                    WorldGen.TileRunner(
                        (int)cavePosition.X,
                        (int)cavePosition.Y,
                        strength: genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(5, 10),
                        type: -1);
                }

                // Update the cave position.
                cavePosition += caveDirection * caveWidth * 0.5f;
            }
        }

        public static void GenerateNoodleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            FastNoiseLite fastNoiseLite = new FastNoiseLite();
            fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            fastNoiseLite.SetSeed(caveSeed);

            for (int j = 0; j < caveSteps; j++)
            {
                float divisor = 2f ;
                float sample = fastNoiseLite.GetNoise((float)cavePosition.X / divisor, (float)cavePosition.Y / divisor);
                sample = MathF.Sin(sample * 4);
                float caveOffsetAngleAtStep = sample * MathHelper.TwoPi * 1.9f;
                Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

                // Carve out at the current position.
                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15 && sample > 0)
                {
                    //digging 
                    WorldGen.TileRunner(
                        (int)cavePosition.X,
                        (int)cavePosition.Y,
                        strength: genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(5, 10),
                        type: -1);
                }

                // Update the cave position.
                cavePosition += caveDirection * caveWidth * 0.5f;
            }
        }

        public static void GenerateWormCave(Vector2 cavePosition, 
            Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            FastNoiseLite fastNoiseLite = new FastNoiseLite();
            fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            fastNoiseLite.SetSeed(caveSeed);

            //Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);
            //Vector2 cavePosition = new Vector2(Main.maxTilesX / 2, (int)Main.worldSurface);

            for (int j = 0; j < caveSteps; j++)
            {
                float divisor = 1f;
                float sample = fastNoiseLite.GetNoise((float)cavePosition.X / divisor, (float)cavePosition.Y / divisor);
               
                float angleOffset = sample * MathHelper.Pi;
                Vector2 caveDirection = baseCaveDirection.RotatedBy(angleOffset);

                // Carve out at the current position.
                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15 && sample > 0f)
                {
                    //digging 
                    WorldGen.TileRunner(
                        (int)cavePosition.X,
                        (int)cavePosition.Y,
                        strength: genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(5, 10),
                        type: -1);
                }

                // Update the cave position.
                cavePosition += caveDirection * caveWidth * 0.5f;
            }
        }

        /*
        public static float SampleNoise(Texture2D texture, float x, float y, float divisor)
        {
            texture.get
        }
    

        private void NoodleCaves()
        {
            var genRand = WorldGen.genRand;
            for (int i = 0; i < 6; i++)
            {
                int caveWidth = 12; // Width
                int caveSteps = 250; // How many carves
                int caveSeed = WorldGen.genRand.Next();
                Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);
                Vector2 cavePosition = new Vector2(Main.maxTilesX / 2, (int)Main.worldSurface);

                for (int j = 0; j < caveSteps; j++)
                {
                    float caveOffsetAngleAtStep = WorldMath.PerlinNoise2D(i / 50f, j / 50f, 1, caveSeed) * MathHelper.TwoPi * 1.9f;
                    Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

                    // Carve out at the current position.
                    if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                    {
                        //digging 
                        WorldGen.TileRunner(
                            (int)cavePosition.X,
                            (int)cavePosition.Y,
                            strength: genRand.NextFloat(10f, 20f),
                            genRand.Next(5, 10),
                            type: -1);
                    }

                    // Update the cave position.
                    cavePosition += caveDirection * caveWidth * 0.5f;
                }
            }
        }

      

        private void VeinyCaves()
        {

            var genRand = WorldGen.genRand;
            for (int i = 0; i < 6; i++)
            {
                int caveWidth = 12; // Width
                int caveSteps = 250; // How many carves

                int caveSeed = WorldGen.genRand.Next();
                Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);
                Vector2 cavePosition = new Vector2(Main.maxTilesX / 2, (int)Main.worldSurface);

                for (int j = 0; j < caveSteps; j++)
                {
                    float caveOffsetAngleAtStep = WorldMath.PerlinNoise2D(1 / 50f, j / 50f, 4, caveSeed) * MathHelper.Pi * 1.9f;
                    Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

                    // Carve out at the current position.
                    if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                    {
                        //digging 
                        WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y, MathF.Sin(j * 0.05f) * 10 + genRand.NextFloat(2, 5), genRand.Next(5, 10), -1);
                    }

                    // Update the cave position.
                    cavePosition += caveDirection * caveWidth * 0.5f;
                }
            }
        }
        */
    }
}
