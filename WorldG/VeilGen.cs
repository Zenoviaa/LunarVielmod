using Microsoft.Xna.Framework;
using Stellamod.Items.Ores;
using Stellamod.Tiles.Veil;
using Stellamod.Tiles;
using Stellamod.WorldG.StructureManager;
using System;
using System.Collections.Generic;
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
            GenerateAshotiTemple();
            return base.UseItem(player);
        }
        private void GenerateAshotiTemple()
        {
            int radius = 80;
            int desertCenterX = (GenVars.desertHiveLeft + GenVars.desertHiveRight) / 2;
            int desertCenterY = GenVars.desertHiveLow - 200;
            Point arenaPoint = new Point(desertCenterX, desertCenterY);

            //Building the arena
            WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius, radius), new Actions.SetTile(TileID.LihzahrdBrick));
            WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius - 2, radius - 2), new Actions.SetTile((ushort)ModContent.TileType<ChiseledStone>()));
            WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius - 4, radius - 4), new Actions.SetTile((ushort)ModContent.TileType<NoxianBlock>()));
            WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius - 6, radius - 6), new Actions.ClearTile());
            WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius / 2, radius / 2), new Actions.SetLiquid(type: LiquidID.Lava));


            //Decorate arena with walls
            for (int w = 0; w < 80; w++)
            {
                float progressOnCircle = (float)w / 80f;
                float rot = progressOnCircle * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * radius;
                Point pointToWall = arenaPoint + vel.ToPoint();
                WorldUtils.Gen(pointToWall, new Shapes.Circle(4, 4), new Actions.PlaceWall(type: WallID.LihzahrdBrickUnsafe));
            }

            //Make Middle of the Temple
            int middleLength = 7;
            for (int m = 0; m < middleLength; m++)
            {
                Point offset = new Point(0, m * -43);
                Point tileToPlaceOn = arenaPoint + offset;

                if (m == middleLength - 1)
                {
                    string structure = "Struct/AshotiTemple/TempleEntrance";
                    Rectangle rect = Structurizer.ReadRectangle(structure);
                    tileToPlaceOn.X -= rect.Width/2;
                    tileToPlaceOn.Y -= 28;
                    int[] chestIndices = Structurizer.ReadStruct(tileToPlaceOn, structure);
                    Structurizer.ProtectStructure(tileToPlaceOn, structure);
                }
                else
                {
                    string structure = "Struct/AshotiTemple/TempleMiddle";
                    Rectangle rect = Structurizer.ReadRectangle(structure);
                    tileToPlaceOn.X -= rect.Width/2;
                    int[] chestIndices = Structurizer.ReadStruct(tileToPlaceOn, structure);
                    Structurizer.ProtectStructure(tileToPlaceOn, structure);
                }
            }
        }
        private void GenerateMineshaftTunnel()
        {
            Vector2 mouseWorld = Main.MouseWorld;
            int x = (int)Main.MouseWorld.X / 16;
            int y = (int)Main.MouseWorld.Y / 16;
            Point tilePoint = new Point(x, y);
            Point tileDirection = new Point(0, -1);
            int tunnel = Main.rand.Next(7, 25);
            VeilGen.GenerateMineshaftTunnel(tilePoint, tileDirection, tunnel);
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

        public static void GenerateVirulentCave(Vector2 cavePosition,
            Vector2 seedPosition,
            Vector2 baseCaveDirection,
            Vector2 caveStrength,
            int caveWidth,
            int caveSteps)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();
            Vector2 caveVelocity = baseCaveDirection;
            float sharpness = 1f;
            for (int j = 0; j < caveSteps; j++)
            {
                float degreesToRotate = sharpness;
                float length = caveVelocity.Length();
                float targetAngle = (seedPosition - cavePosition).ToRotation();
                Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
                caveVelocity = newVelocity;
                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(4, 5), -1);
                }

                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
            }
        }


        public static void GenerateJungleTreeCaves(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength,
            int caveWidth,
            int caveSteps,
            int splitSteps,
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
                    targetPosition = targetPosition.RotatedByRandom(MathHelper.ToRadians(15));
                }

                if (genRand.NextBool(splitDenominator) && j > 4)
                {
                    int clearingCaveWidth = caveWidth / 2;
                    int clearingCaveSteps = splitSteps;

                    //Cave position in tiles
                    Vector2 clearingPosition = new Vector2((int)cavePosition.X, (int)cavePosition.Y);

                    //Starting cave direction
                    float dir = counter % 2 == 0 ? 1 : -1;
                    counter++;
                    Vector2 clearingCaveDirection = baseCaveDirection.RotatedBy(dir * MathHelper.PiOver2);

                    //How much the tile runner is gonna carve out
                    Vector2 clearingCaveStrength = caveStrength * 0.5f;

                    VeilGen.GenerateJungleTreeCaves(clearingPosition,
                        clearingCaveDirection,
                        clearingCaveStrength,
                        clearingCaveWidth,
                        clearingCaveSteps,
                        genRand.Next(splitSteps / 2, splitSteps),
                        splitDenominator * 640);
                }

                /*
                Point cavePoint = cavePosition.ToPoint();
                Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
                WorldUtils.Gen(cavePoint, new Shapes.Rectangle(20, 10), new Actions.TileScanner(TileID.Mud, TileID.Stone).Output(dictionary));
                int mudCount = dictionary[TileID.Mud];
                int stoneCount = dictionary[TileID.Stone];
                if(stoneCount > mudCount)
                {
                    return;
                }
                */
                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(4, 5), -1);
                }



                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
                //  caveStrength *= 0.99f;
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

        public static float TilePercent(Point tilePoint, Rectangle size, params ushort[] tileIDs)
        {
            int count = 0;
            int width = size.Width;
            int height = size.Height;
            for (int x = tilePoint.X; x < tilePoint.X + width; x++)
            {
                if (x < 0)
                    continue;
                if (x >= Main.maxTilesX)
                    continue;
                    
                for (int y = tilePoint.Y; y > tilePoint.Y - height; y--)
                {

                    if (y < 0)
                        continue;
                    if (y >= Main.maxTilesY)
                        continue;

                    Tile tile = Main.tile[x, y];
                    for (int t = 0; t < tileIDs.Length; t++)
                    {
                        int tileID = tileIDs[t];
                        if(!WorldGen.SolidTile(x, y))
                        {
                            count++;
                        }

                        if(tile.HasTile && tile.TileType == tileID)
                        {
                            count++;
                        }
                    }
                }
            }

            int tileM = width * height;
            float tilePercent = (float)count / (float)tileM;
            return tilePercent;
        }

        public static void GenerateMineshaftTunnel(Point tilePoint, Point tileDirection, int tunnelLength)
        {
            var genRand = WorldGen.genRand;
            string GetStructurePath()
            {
                int num = genRand.Next(1, 15);
                string baseStructurePath = $"Struct/Catacombs/CaRoom{num}";
                return baseStructurePath;
            }

            int[] tileBlend = new int[]
            {

            };

            for(int t = 0; t < tunnelLength; t++)
            {
                string structure = GetStructurePath();
                Rectangle rectangle = Structurizer.ReadRectangle(structure);
                rectangle.Location = tilePoint;
                if(TilePercent(tilePoint, rectangle, TileID.Dirt, TileID.Stone) < 0.5f)
                {
                    break;
                }

                int[] chestIndices = Structurizer.ReadStruct(tilePoint, structure, null);
                if(chestIndices.Length != 0)
                {
                    foreach (int chestIndex in chestIndices)
                    {
                        if (chestIndex == -1)
                            continue;
                        Chest chest = Main.chest[chestIndex];
                        var itemsToAdd = new List<(int type, int stack)>();
                        if (genRand.NextBool(2))
                        {
                            switch (genRand.Next(6))
                            {
                                case 0:
                                    itemsToAdd.Add((ItemID.MagicMirror, 1));
                                    break;
                                case 1:
                                    itemsToAdd.Add((ItemID.HermesBoots, 1));
                                    break;
                                case 2:
                                    itemsToAdd.Add((ItemID.FlareGun, 1));
                                    itemsToAdd.Add((ItemID.Flare, genRand.Next(20, 30)));
                                    break;
                                case 3:
                                    itemsToAdd.Add((ItemID.Mace, 1));
                                    break;
                                case 4:
                                    if (genRand.NextBool(7))
                                    {
                                        itemsToAdd.Add((ItemID.LavaCharm, 1));
                                    }
                                    break;
                                case 5:
                                    itemsToAdd.Add((ItemID.Aglet, 1));
                                    break;
                            }
                        }

                        itemsToAdd.Add((ModContent.ItemType<GrailBar>(), genRand.Next(3, 5)));
                        if (genRand.NextBool(3))
                        {
                            switch (genRand.Next(0, 2))
                            {
                                case 0:
                                    itemsToAdd.Add((ItemID.Bomb, genRand.Next(3, 7)));
                                    break;
                                case 1:
                                    itemsToAdd.Add((ItemID.Dynamite, genRand.Next(1, 3)));
                                    break;
                            }
                        }

                        if (genRand.NextBool(3))
                        {
                            switch (genRand.Next(0, 2))
                            {
                                case 0:
                                    itemsToAdd.Add((ItemID.Torch, genRand.Next(3, 7)));
                                    break;
                                case 1:
                                    itemsToAdd.Add((ItemID.SpelunkerGlowstick, genRand.Next(5, 10)));
                                    break;
                            }
                        }

                        if (genRand.NextBool(3))
                        {
                            switch (genRand.Next(0, 2))
                            {
                                case 0:
                                    itemsToAdd.Add((ItemID.LesserHealingPotion, genRand.Next(2, 4)));
                                    break;
                                case 1:
                                    itemsToAdd.Add((ItemID.LesserManaPotion, genRand.Next(1, 3)));
                                    break;
                            }
                        }

                        if (genRand.NextBool(3))
                        {
                            switch (genRand.Next(0, 5))
                            {
                                case 0:
                                    itemsToAdd.Add((ItemID.SpelunkerPotion, genRand.Next(2, 4)));
                                    break;
                                case 1:
                                    itemsToAdd.Add((ItemID.PotionOfReturn, genRand.Next(1, 3)));
                                    break;
                                case 2:
                                    itemsToAdd.Add((ItemID.HunterPotion, genRand.Next(1, 3)));
                                    break;
                                case 3:
                                    itemsToAdd.Add((ItemID.MiningPotion, genRand.Next(1, 3)));
                                    break;
                                case 4:
                                    itemsToAdd.Add((ItemID.TrapsightPotion, genRand.Next(1, 3)));
                                    break;
                            }
                        }
                        for (int n = 0; n < 4; n++)
                        {
                            if (genRand.NextBool(4))
                            {
                                switch (genRand.Next(0, 7))
                                {
                                    case 0:
                                        itemsToAdd.Add((ItemID.Amethyst, genRand.Next(3, 10)));
                                        break;
                                    case 1:
                                        itemsToAdd.Add((ItemID.Emerald, genRand.Next(3, 10)));
                                        break;
                                    case 2:
                                        itemsToAdd.Add((ItemID.Sapphire, genRand.Next(3, 10)));
                                        break;
                                    case 3:
                                        itemsToAdd.Add((ItemID.Topaz, genRand.Next(3, 10)));
                                        break;
                                    case 4:
                                        itemsToAdd.Add((ItemID.Ruby, genRand.Next(3, 10)));
                                        break;
                                    case 5:
                                        itemsToAdd.Add((ItemID.Diamond, genRand.Next(3, 10)));
                                        break;
                                    case 6:
                                        itemsToAdd.Add((ItemID.Amber, genRand.Next(3, 10)));
                                        break;
                                }
                            }
                        }

                        for (int n = 0; n < 4; n++)
                        {
                            if (genRand.NextBool(4))
                            {
                                switch (genRand.Next(0, 8))
                                {
                                    case 0:
                                        itemsToAdd.Add((ItemID.CopperOre, genRand.Next(3, 10)));
                                        break;
                                    case 1:
                                        itemsToAdd.Add((ItemID.TinOre, genRand.Next(3, 10)));
                                        break;
                                    case 2:
                                        itemsToAdd.Add((ItemID.IronOre, genRand.Next(3, 10)));
                                        break;
                                    case 3:
                                        itemsToAdd.Add((ItemID.LeadOre, genRand.Next(3, 10)));
                                        break;
                                    case 4:
                                        itemsToAdd.Add((ItemID.SilverOre, genRand.Next(3, 10)));
                                        break;
                                    case 5:
                                        itemsToAdd.Add((ItemID.TungstenOre, genRand.Next(3, 10)));
                                        break;
                                    case 6:
                                        itemsToAdd.Add((ItemID.GoldOre, genRand.Next(3, 10)));
                                        break;
                                    case 7:
                                        itemsToAdd.Add((ItemID.PlatinumOre, genRand.Next(3, 10)));
                                        break;
                                }
                            }
                        }

                        if (genRand.NextBool(1))
                        {
                            switch (genRand.Next(3))
                            {
                                case 0:
                                    itemsToAdd.Add((ItemID.CopperCoin, genRand.Next(45, 100)));
                                    break;
                                case 1:
                                    itemsToAdd.Add((ItemID.SilverCoin, genRand.Next(45, 100)));
                                    break;
                                case 2:
                                    itemsToAdd.Add((ItemID.GoldCoin, genRand.Next(1, 3)));
                                    break;
                            }
                        }

                        if (genRand.NextBool(100))
                        {
                            itemsToAdd.Add((ItemID.MiningHelmet, 1));
                            itemsToAdd.Add((ItemID.MiningPants, 1));
                            itemsToAdd.Add((ItemID.MiningShirt, 1));
                        }

                        int chestItemIndex = 0;
                        foreach (var itemToAdd in itemsToAdd)
                        {
                            Item item = new Item();
                            item.SetDefaults(itemToAdd.type);
                            item.stack = itemToAdd.stack;
                            chest.item[chestItemIndex] = item;
                            chestItemIndex++;
                            if (chestItemIndex >= 40)
                                break; // Make sure not to exceed the capacity of the chest
                        }
                    }

                }


                Structurizer.ProtectStructure(tilePoint, structure);

                if(tileDirection.X != 0)
                {
                    tilePoint.X += tileDirection.X * rectangle.Width;
                } 
                else if (tileDirection.Y != 0)
                {
                    tilePoint.Y += tileDirection.Y * (rectangle.Height + 1);
                }

                if (genRand.NextBool(4) && tileDirection != new Point(0, -1))
                {
                    GenerateMineshaftTunnel(tilePoint, new Point(0, -1), tunnelLength / 2);
                } else if (genRand.NextBool(2) && tileDirection != new Point(1, 0))
                {
                    GenerateMineshaftTunnel(tilePoint, new Point(1, 0), tunnelLength / 2);
                }
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
