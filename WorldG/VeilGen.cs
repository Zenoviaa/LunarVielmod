using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using Stellamod.Tiles.Abyss;
using Stellamod.Tiles.Veil;
using Stellamod.TilesNew.RainforestTiles;
using Stellamod.WorldG.StructureManager;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.WorldBuilding;

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
            GenerateColosseum();
            return base.UseItem(player);
        }

        private void GenerateColosseum()
        {
            var genRand = WorldGen.genRand;
            Vector2 mouseWorld = Main.MouseWorld;
            int tileX = (int)Main.MouseWorld.X / 16;
            int tileY = (int)Main.MouseWorld.Y / 16;
            Vector2 colosseumPosition = new Vector2(tileX, tileY);
            Point colosseumPoint = colosseumPosition.ToPoint();
            Vector2 caveStrength = new Vector2(12, 15);
            int fallSteps = 150;
            int caveWidth = 5;
            VeilGen.GenerateDuneHole((colosseumPoint + new Point(51, 0)).ToVector2(), Vector2.UnitY, caveStrength * 2f, Vector2.Zero, caveWidth,
              caveSteps: fallSteps,
              tileToPlace: TileID.Sandstone,
              addTile: true);
            VeilGen.GenerateDuneHoleEdges((colosseumPoint + new Point(51, 0)).ToVector2(), Vector2.UnitY, caveStrength * 2f, Vector2.Zero, caveWidth,
                caveSteps: fallSteps,
                tileToPlace: TileID.Sandstone,
                addTile: true);
            VeilGen.GenerateDuneHoleEdges((colosseumPoint + new Point(51, 0)).ToVector2(), Vector2.UnitY, caveStrength, Vector2.Zero, caveWidth,
                caveSteps: fallSteps,
                tileToPlace: -1,
                addTile: false);
            VeilGen.GenerateColosseum(colosseumPosition.ToPoint());

        }

        private void GenerateCavernToAbyss()
        {
            var genRand = WorldGen.genRand;
            Vector2 mouseWorld = Main.MouseWorld;
            int tileX = (int)Main.MouseWorld.X / 16;
            int tileY = (int)Main.MouseWorld.Y / 16;
            Vector2 cavePosition = new Vector2(tileX, tileY);
            Vector2 caveVelocity = Vector2.UnitY;
            Vector2 caveStrength = new Vector2(15, 20);
            Vector2 pullDirection = -Vector2.UnitX * 0.2f;
            int caveWidth = 7;
            int caveSteps = 100;
            VeilGen.GenerateFallingIceCavern(cavePosition, caveVelocity, pullDirection, caveStrength, caveWidth, caveSteps);
        }

        private void GenerateSmallFallingIceCavern()
        {

            var genRand = WorldGen.genRand;
            Vector2 mouseWorld = Main.MouseWorld;
            int tileX = (int)Main.MouseWorld.X / 16;
            int tileY = (int)Main.MouseWorld.Y / 16;
            Vector2 cavePosition = new Vector2(tileX, tileY);
            Vector2 caveVelocity = Vector2.UnitX;
            if (genRand.NextBool(2))
            {
                caveVelocity = -Vector2.UnitX;
            }
            Vector2 caveStrength = new Vector2(5, 10);
            Vector2 pullDirection = Vector2.UnitY;
            int caveWidth = 7;
            int caveSteps = 25;
            VeilGen.GenerateFallingIceCavern(cavePosition, caveVelocity, pullDirection, caveStrength, caveWidth, caveSteps);
        }

        private void GenerateFallingIceCavern()
        {

            var genRand = WorldGen.genRand;
            Vector2 mouseWorld = Main.MouseWorld;
            int tileX = (int)Main.MouseWorld.X / 16;
            int tileY = (int)Main.MouseWorld.Y / 16;
            Vector2 cavePosition = new Vector2(tileX, tileY);
            Vector2 caveVelocity = Vector2.UnitX;
            Vector2 caveStrength = new Vector2(20, 30);
            Vector2 pullDirection = Vector2.UnitY;
            int caveWidth = 7;
            int caveSteps = 100;
            VeilGen.GenerateFallingIceCavern(cavePosition, caveVelocity, pullDirection, caveStrength, caveWidth, caveSteps);
        }


        private void GenerateIceCavern()
        {
            var genRand = WorldGen.genRand;
            Vector2 mouseWorld = Main.MouseWorld;
            int tileX = (int)Main.MouseWorld.X / 16;
            int tileY = (int)Main.MouseWorld.Y / 16;
            Vector2 cavePosition = new Vector2(tileX, tileY);
            Vector2 caveVelocity = Vector2.UnitX;
            Vector2 caveStrength = new Vector2(20, 30);
            int caveWidth = 7;
            int caveSteps = 100;
            VeilGen.GenerateIceCavern(cavePosition, caveVelocity, caveStrength, caveWidth, caveSteps);
        }

        private void GenerateMarble()
        {
            var genRand = WorldGen.genRand;
            Vector2 mouseWorld = Main.MouseWorld;
            int tileX = (int)Main.MouseWorld.X / 16;
            int tileY = (int)Main.MouseWorld.Y / 16;
            Point granitePoint = new Point(tileX, tileY);

            int maxRadius = 64;
            int radius = genRand.Next(24, 64);
            float sizeMultiplier = (float)radius / (float)maxRadius;
            WorldUtils.Gen(granitePoint, new Shapes.Circle(radius, radius),
                new Actions.SetTile(TileID.Marble));
            for (int n = 0; n < 150; n++)
            {
                int r = genRand.Next(radius - 30, radius + 16);
                Point tileRunnePoint = granitePoint + genRand.NextVector2CircularEdge(r, r).ToPoint();
                Vector2 strength = new Vector2(3, 4);
                WorldGen.TileRunner(tileRunnePoint.X, tileRunnePoint.Y,
                    genRand.NextFloat(strength.X, strength.Y),
                    genRand.Next(4, 5), -1);
            }
            for (int n = 0; n < 450; n++)
            {
                int r = genRand.Next(radius - 30, radius + 16);
                Point tileRunnePoint = granitePoint + genRand.NextVector2CircularEdge(r, r).ToPoint();
                Vector2 strength = new Vector2(8, 10);
                WorldGen.TileRunner(tileRunnePoint.X, tileRunnePoint.Y,
                    genRand.NextFloat(strength.X, strength.Y),
                    genRand.Next(4, 5), TileID.Marble);
            }

            Vector2 cavePosition = new Vector2(tileX, tileY) - new Vector2(radius, radius / 4);

            //Starting cave direction
            Vector2 baseCaveDirection = Vector2.UnitX;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

            //How much the tile runner is gonna carve out
            Vector2 caveStrength = new Vector2(12, 14);

            //Chance to open up
            int caveWidth = 5;
            int caveSteps = (int)(50f * sizeMultiplier);
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;

            ushort[] wallTypes = new ushort[]
            {
                WallID.MarbleUnsafe,
                WallID.MarbleBlock
            };

            for (int w = 0; w < 800; w++)
            {
                Point shadowOrbPoint = granitePoint + genRand.NextVector2Circular(radius, radius).ToPoint();
                ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
                WorldUtils.Gen(shadowOrbPoint, new Shapes.Circle(4, 4), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceWall(wallType),
                    new Actions.Smooth(true)
                }));
            }


            for (int j = 0; j < caveSteps; j++)
            {

                Vector2 newVelocity = caveVelocity;
                newVelocity.Y += MathF.Sin((float)j * 2f) * 8;
                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
                    WorldUtils.Gen(cavePosition.ToPoint(), new Shapes.Circle(6, 6), Actions.Chain(new GenAction[]
                    {
                        new Actions.PlaceWall(wallType),
                        new Actions.Smooth(true)
                    }));
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(4, 20), -1);
                }

                // Update the cave position.
                cavePosition += newVelocity * caveWidth * 0.5f;
                //  caveStrength *= 0.99f;
            }
        }

        private void GenerateGranite()
        {
            var genRand = WorldGen.genRand;
            Vector2 mouseWorld = Main.MouseWorld;
            int tileX = (int)Main.MouseWorld.X / 16;
            int tileY = (int)Main.MouseWorld.Y / 16;
            Point granitePoint = new Point(tileX, tileY);

            int radius = genRand.Next(24, 64);
            WorldUtils.Gen(granitePoint, new Shapes.Circle(radius, radius),
                new Actions.SetTile(TileID.Granite));
            for (int n = 0; n < 150; n++)
            {
                int r = genRand.Next(radius - 30, radius + 16);
                Point tileRunnePoint = granitePoint + genRand.NextVector2CircularEdge(r, r).ToPoint();
                Vector2 strength = new Vector2(3, 4);
                WorldGen.TileRunner(tileRunnePoint.X, tileRunnePoint.Y,
                    genRand.NextFloat(strength.X, strength.Y),
                    genRand.Next(4, 5), -1);
            }
            for (int n = 0; n < 450; n++)
            {
                int r = genRand.Next(radius - 30, radius + 16);
                Point tileRunnePoint = granitePoint + genRand.NextVector2CircularEdge(r, r).ToPoint();
                Vector2 strength = new Vector2(8, 10);
                WorldGen.TileRunner(tileRunnePoint.X, tileRunnePoint.Y,
                    genRand.NextFloat(strength.X, strength.Y),
                    genRand.Next(4, 5), TileID.Granite);
            }

            Vector2 cavePosition = new Vector2(tileX, tileY) - new Vector2(radius / 4, radius);

            //Starting cave direction
            Vector2 baseCaveDirection = Vector2.UnitY;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

            //How much the tile runner is gonna carve out
            Vector2 caveStrength = new Vector2(12, 14);

            //Chance to open up
            int caveWidth = 5;
            int caveSteps = 50;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;

            ushort[] wallTypes = new ushort[]
            {
                WallID.GraniteUnsafe,
                WallID.GraniteBlock
            };

            for (int w = 0; w < 800; w++)
            {
                Point shadowOrbPoint = granitePoint + genRand.NextVector2Circular(radius, radius).ToPoint();
                ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
                WorldUtils.Gen(shadowOrbPoint, new Shapes.Circle(4, 4), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceWall(wallType),
                    new Actions.Smooth(true)
                }));
            }


            for (int j = 0; j < caveSteps; j++)
            {

                Vector2 newVelocity = caveVelocity;
                newVelocity.X += MathF.Sin((float)j) * 8;
                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
                    WorldUtils.Gen(cavePosition.ToPoint(), new Shapes.Circle(6, 6), Actions.Chain(new GenAction[]
                    {
                        new Actions.PlaceWall(wallType),
                        new Actions.Smooth(true)
                    }));
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(4, 5), -1);
                }

                // Update the cave position.
                cavePosition += newVelocity * caveWidth * 0.5f;
                //  caveStrength *= 0.99f;
            }
        }

        private void GenerateEvil()
        {
            var genRand = WorldGen.genRand;
            Vector2 mouseWorld = Main.MouseWorld;
            int mx = (int)Main.MouseWorld.X / 16;
            int my = (int)Main.MouseWorld.Y / 16;
            Point tilePoint = new Point(mx, my);
            Point evilPoint = tilePoint;

            int radius = 96;
            WorldGen.crimson = false;
            ushort blockType = WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone;
            ushort wallType = WorldGen.crimson ? WallID.CrimsonUnsafe1 : WallID.CorruptionUnsafe1;

            WorldUtils.Gen(evilPoint, new Shapes.Circle(radius, radius), new Actions.SetTile(blockType));
            WorldUtils.Gen(evilPoint, new Shapes.Circle(radius - 20, radius - 20), new Actions.ClearTile());
            WorldUtils.Gen(evilPoint, new Shapes.Circle(radius - 40, radius - 40), new Actions.SetTile(blockType));

            ushort[] corruptWallTypes = new ushort[]
            {
                        WallID.CorruptionUnsafe1,
                        WallID.CorruptionUnsafe2,
                        WallID.EbonstoneUnsafe
            };

            ushort[] crimsonWallTypes = new ushort[]
            {
                        WallID.CrimsonUnsafe1,
                        WallID.CrimsonUnsafe2,
                        WallID.CrimstoneUnsafe
            };

            int decorativeBlock = WorldGen.crimson ? TileID.FleshBlock : TileID.LesionBlock;
            int lampType = WorldGen.crimson ? 14 : 33;
            int lanternType = WorldGen.crimson ? 23 : 39;
            for (int w = 0; w < 800; w++)
            {
                Point shadowOrbPoint = evilPoint + genRand.NextVector2Circular(80, 80).ToPoint();

                ushort wallType2 = WorldGen.crimson ?
                    crimsonWallTypes[genRand.Next(0, crimsonWallTypes.Length)] :
                    corruptWallTypes[genRand.Next(0, corruptWallTypes.Length)];
                WorldUtils.Gen(shadowOrbPoint, new Shapes.Circle(4, 4), Actions.Chain(new GenAction[]
                {
                            new Actions.PlaceWall(wallType2),
                            new Actions.Smooth(true)
                }));
            }

            for (int w = 0; w < 150; w++)
            {
                int radius2 = genRand.Next(50, 100);
                Point shadowOrbPoint = evilPoint + genRand.NextVector2CircularEdge(radius2, radius2).ToPoint();
                ushort wallType2 = WorldGen.crimson ? WallID.Flesh : WallID.LesionBlock;
                WorldUtils.Gen(shadowOrbPoint, new Shapes.Circle(1, 1), Actions.Chain(new GenAction[]
                {
                            new Actions.PlaceWall(wallType2),
                            new Actions.Smooth(true)
                }));
            }


            float pokey = 12;
            for (int n = 0; n < pokey; n++)
            {
                float progress = (float)n / pokey;
                float rot = progress * MathHelper.TwoPi;
                Vector2 velocity = rot.ToRotationVector2() * 66;
                Point cavePoint = evilPoint + velocity.ToPoint();
                Vector2 strength = new Vector2(3, 4);

                Vector2 moveVelocity = -velocity.SafeNormalize(Vector2.Zero);
                VeilGen.GenerateSimpleCave(cavePoint.ToVector2(), moveVelocity,
                    strength, moveVelocity, 2, caveSteps: 30);
            }

            for (int n = 0; n < 800; n++)
            {
                float progress = (float)n / 800f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 velocity = rot.ToRotationVector2() * genRand.NextFloat(50, 80);
                Point cavePoint = evilPoint + velocity.ToPoint();
                Vector2 strength = new Vector2(3, 4);

                WorldGen.TileRunner((int)cavePoint.X, (int)cavePoint.Y,
                    genRand.NextFloat(strength.X, strength.Y),
                    genRand.Next(4, 5), -1);
            }

            for (int n = 0; n < 800; n++)
            {
                float progress = (float)n / 800f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 velocity = rot.ToRotationVector2() * genRand.NextFloat(50, 80);
                Point cavePoint = evilPoint + velocity.ToPoint();
                Vector2 strength = new Vector2(3, 4);


                WorldGen.TileRunner((int)cavePoint.X, (int)cavePoint.Y,
                    genRand.NextFloat(strength.X, strength.Y),
                    genRand.Next(4, 5), decorativeBlock);
            }

            for (int n = 0; n < 800; n++)
            {
                float progress = (float)n / 800f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 velocity = rot.ToRotationVector2() * genRand.NextFloat(60, 100);
                Point cavePoint = evilPoint + velocity.ToPoint();
                Vector2 strength = new Vector2(3, 4);

                WorldGen.TileRunner((int)cavePoint.X, (int)cavePoint.Y,
                    genRand.NextFloat(strength.X, strength.Y),
                    genRand.Next(4, 5), decorativeBlock);
            }

            for (int n = 0; n < 10; n++)
            {
                float progress = (float)n / 10f;
                float rot = progress * MathHelper.TwoPi;
                rot += MathHelper.ToRadians(30);
                Vector2 velocity = rot.ToRotationVector2() * 10;
                Point shadowOrbPoint = evilPoint + velocity.ToPoint();
                WorldGen.AddShadowOrb(shadowOrbPoint.X, shadowOrbPoint.Y);
            }

            for (int n = 0; n < 10; n++)
            {
                float progress = (float)n / 10f;
                float rot = progress * MathHelper.TwoPi;
                rot += MathHelper.ToRadians(60);
                Vector2 velocity = rot.ToRotationVector2() * 30;
                Point shadowOrbPoint = evilPoint + velocity.ToPoint();
                WorldGen.AddShadowOrb(shadowOrbPoint.X, shadowOrbPoint.Y);
            }

            for (int n = 0; n < 10; n++)
            {
                float progress = (float)n / 10f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 velocity = rot.ToRotationVector2() * 50;
                Point shadowOrbPoint = evilPoint + velocity.ToPoint();
                WorldGen.AddShadowOrb(shadowOrbPoint.X, shadowOrbPoint.Y);
            }

            for (int n = 0; n < 1600; n++)
            {
                float range = genRand.NextFloat(30, 100);
                Point fPoint = evilPoint + genRand.NextVector2CircularEdge(range, range).ToPoint();

                WorldGen.Place1xX(fPoint.X, fPoint.Y, TileID.Lamps, style: lampType);
            }
            for (int n = 0; n < 800; n++)
            {
                float range = genRand.NextFloat(30, 100);
                Point fPoint = evilPoint + genRand.NextVector2CircularEdge(range, range).ToPoint();
                WorldGen.Place1x2Top(fPoint.X, fPoint.Y, TileID.HangingLanterns, style: lanternType);
            }

            //Make Extra
            Vector2 caveStrength = new Vector2(10, 12);
            Vector2 pullDirection = -Vector2.UnitY;
            int caveWidth = 5;
            int steps = 150;

            VeilGen.GenerateSimpleCaveWall((evilPoint + new Point(-16, -32)).ToVector2(), -Vector2.UnitX, caveStrength * 2f, pullDirection, caveWidth, caveSteps: steps, tileToPlace: wallType);
            VeilGen.GenerateSimpleCave((evilPoint + new Point(-16, -32)).ToVector2(), -Vector2.UnitX, caveStrength * 2f, pullDirection, caveWidth, caveSteps: steps, tileToPlace: blockType);
            VeilGen.GenerateSimpleCave((evilPoint + new Point(-16, -32)).ToVector2(), -Vector2.UnitX, caveStrength, pullDirection, caveWidth, caveSteps: steps, tileToPlace: -1);

            int fallSteps = 40;
            VeilGen.GenerateSimpleCave((evilPoint + new Point(0, 48)).ToVector2(), Vector2.UnitY, caveStrength * 2f, Vector2.UnitY, caveWidth,
                caveSteps: fallSteps,
                tileToPlace: blockType);
            VeilGen.GenerateSimpleCave((evilPoint + new Point(0, 48)).ToVector2(), Vector2.UnitY, caveStrength, Vector2.UnitY, caveWidth,
                caveSteps: fallSteps,
                tileToPlace: -1);
            VeilGen.GenerateSimpleCave((evilPoint + new Point(-128, 100)).ToVector2(), Vector2.UnitX, caveStrength * 2f, Vector2.UnitX, caveWidth,
                caveSteps: fallSteps * 2,
                tileToPlace: blockType,
                addTile: true);
            VeilGen.GenerateSimpleCave((evilPoint + new Point(-128, 100)).ToVector2(), Vector2.UnitX, caveStrength, Vector2.UnitX, caveWidth,
                caveSteps: fallSteps * 2,
                tileToPlace: -1);

            for (int n = 0; n < 6400; n++)
            {
                int x = genRand.Next(evilPoint.X - 128, evilPoint.X + 128);
                int y = genRand.Next(evilPoint.Y + 90, evilPoint.Y + 150);
                int style = WorldGen.crimson ? 1 : 0;
                WorldGen.Place3x2(x, y, 26, style);
            }

            for (int x = evilPoint.X - 128; x < evilPoint.X + 128; x++)
            {
                int y = evilPoint.Y + 100;
                Point wallPoint = new Point(x, y);
                ushort wallType2 = WorldGen.crimson ? WallID.CrimstoneUnsafe : WallID.EbonstoneUnsafe;
                WorldUtils.Gen(wallPoint, new Shapes.Circle(8, 8), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceWall(wallType2),
                    new Actions.Smooth(true)
                }));
            }


            //Crimsonfy/Ebonfy surroundings
            int corruptRadius = 500;
            for (int x = evilPoint.X - corruptRadius; x < evilPoint.X + corruptRadius; x++)
            {
                for (int y = evilPoint.Y - corruptRadius; y < evilPoint.Y + corruptRadius; y++)
                {
                    if (!WorldGen.SolidTile(x, y))
                        continue;
                    Tile tile = Main.tile[x, y];
                    if (tile.TileType == TileID.Grass)
                    {
                        ushort grassType = WorldGen.crimson ? TileID.CrimsonGrass : TileID.CorruptGrass;
                        WorldGen.PlaceTile(x, y, grassType, forced: true);
                    }
                    if (tile.TileType == TileID.Stone)
                    {
                        WorldGen.PlaceTile(x, y, blockType, forced: true);
                    }
                }
            }
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
                    tileToPlaceOn.X -= rect.Width / 2;
                    tileToPlaceOn.Y -= 28;
                    int[] chestIndices = Structurizer.ReadStruct(tileToPlaceOn, structure);
                    Structurizer.ProtectStructure(tileToPlaceOn, structure);
                }
                else
                {
                    string structure = "Struct/AshotiTemple/TempleMiddle";
                    Rectangle rect = Structurizer.ReadRectangle(structure);
                    tileToPlaceOn.X -= rect.Width / 2;
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
            VeilGen.GenerateSquiggleCave(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
        }
        private void GenerateTreeCaves()
        {
            Vector2 mouseWorld = Main.MouseWorld;
            int x = (int)Main.MouseWorld.X / 16;
            int y = (int)Main.MouseWorld.Y / 16;
            int caveWidth = 5;
            int caveSteps = 50;

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
            VeilGen.GenerateVeinyCaves(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
        }
    }

    internal static class VeilGen
    {
        public static Vector2 TileAdj => (Lighting.Mode == Terraria.Graphics.Light.LightMode.Retro || Lighting.Mode == Terraria.Graphics.Light.LightMode.Trippy) ? Vector2.Zero : Vector2.One * 12;
        public static bool IsAir(int x, int y, int w)
        {
            for (int k = 0; k < w; k++)
            {
                Tile tile = Framing.GetTileSafely(x + k, y);
                if (tile.HasTile && Main.tileSolid[tile.TileType])
                    return false;
            }

            return true;
        }

        public static bool IsRainforestTreeGround(int x, int y, int w)
        {
            for (int k = 0; k < w; k++)
            {
                Tile tile = Framing.GetTileSafely(x + k, y);
                if (!(tile.HasTile && tile.Slope == SlopeType.Solid && !tile.IsHalfBlock && (tile.TileType == ModContent.TileType<RainforestGrass>())))
                    return false;

                Tile tile2 = Framing.GetTileSafely(x + k, y - 1);
                if (tile2.HasTile && Main.tileSolid[tile2.TileType])
                    return false;
            }

            return true;
        }
        public static void PlaceMultitile(Point16 position, int type, int style = 0)
        {
            var data = TileObjectData.GetTileData(type, style); //magic numbers and uneccisary params begone!

            if (position.X + data.Width > Main.maxTilesX || position.X < 0)
                return; //make sure we dont spawn outside of the world!

            if (position.Y + data.Height > Main.maxTilesY || position.Y < 0)
                return;

            int xVariants = 0;
            int yVariants = 0;

            if (data.StyleHorizontal)
                xVariants = Main.rand.Next(data.RandomStyleRange);
            else
                yVariants = Main.rand.Next(data.RandomStyleRange);

            for (int x = 0; x < data.Width; x++) //generate each column
            {
                for (int y = 0; y < data.Height; y++) //generate each row
                {
                    Tile tile = Framing.GetTileSafely(position.X + x, position.Y + y); //get the targeted tile
                    tile.TileType = (ushort)type; //set the type of the tile to our multitile

                    int yHeight = 0;
                    for (int k = 0; k < data.CoordinateHeights.Length; k++)
                    {
                        yHeight += data.CoordinateHeights[k] + data.CoordinatePadding;
                    }

                    tile.TileFrameX = (short)((x + data.Width * xVariants) * (data.CoordinateWidth + data.CoordinatePadding)); //set the X frame appropriately
                    tile.TileFrameY = (short)(y * (data.CoordinateHeights[y > 0 ? y - 1 : y] + data.CoordinatePadding) + yVariants * yHeight); //set the Y frame appropriately
                    tile.HasTile = true; //activate the tile
                }
            }
        }
        public static void PlaceRaintrees(int treex, int treey, int height)
        {
            treey -= 1;

            if (treey - height < 1)
                return;

            for (int x = -1; x < 3; x++)
            {
                for (int y = 0; y < (height + 2); y++)
                {
                    WorldGen.KillTile(treex + x, treey - y);
                }
            }

            PlaceMultitile(new Point16(treex, treey - 1), ModContent.TileType<RainforestTreeBase>());

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    WorldGen.PlaceTile(treex + x, treey - (y + 2), ModContent.TileType<RainforestTree>(), true, true);
                }
            }

            for (int x = -1; x < 3; x++)
            {
                for (int y = 0; y < (height + 2); y++)
                {
                    WorldGen.TileFrame(treex + x, treey + y);
                }
            }
        }

        public static void GenerateIceSpike(Vector2 cavePosition, double width, Vector2D endOffset, ushort tileId = TileID.IceBlock)
        {
            WorldUtils.Gen(cavePosition.ToPoint(), new Shapes.Tail(width, endOffset), Actions.Chain(new GenAction[]
            {
                    new Actions.SetTile(tileId),
            }));
        }

        public static void GenerateFallingIceCavern(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 pullDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
        {
            var genRand = WorldGen.genRand;
            Vector2 caveVelocity = baseCaveDirection;
            ushort[] wallTypes = new ushort[]
            {
                WallID.SnowWallUnsafe,
                WallID.IceUnsafe
            };

            Vector2 pullVelocity = pullDirection;
            Vector2 startVelocity = baseCaveDirection;
            float sharpness = 1f;
            int ignoreTile = ModContent.TileType<AbyssalDirt>();
            for (int s = 0; s < caveSteps; s++)
            {
                float radiansOffset = MathF.Sin(s * 0.5f) * MathHelper.ToRadians(45);
                float degreesToRotate = sharpness;
                float length = caveVelocity.Length();
                float targetAngle = (pullVelocity - startVelocity).ToRotation();
                Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle,
                    MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
                caveVelocity = newVelocity;

                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(7, 25), -1, ignoreTileType: ignoreTile);
                }

                //Place Walls
                for (int w = 0; w < 5; w++)
                {
                    ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
                    if (genRand.NextBool(2))
                    {
                        wallType = WallID.IceUnsafe;
                    }

                    Vector2 wallVelocity = genRand.NextVector2Circular(32, 32);
                    Vector2 wallPosition = cavePosition + wallVelocity;
                    WorldUtils.Gen(wallPosition.ToPoint(), new Shapes.Circle(4, 4), Actions.Chain(new GenAction[]
                    {
                        new Actions.PlaceWall(wallType),
                        new Actions.Smooth(true)
                    }));
                }


                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
            }
        }

        public static void GenerateIceCavern(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
        {
            var genRand = WorldGen.genRand;
            Vector2 caveVelocity = baseCaveDirection;
            ushort[] wallTypes = new ushort[]
            {
                WallID.SnowWallUnsafe,
                WallID.IceUnsafe
            };

            int ignoreTile = ModContent.TileType<AbyssalDirt>();
            for (int s = 0; s < caveSteps; s++)
            {
                float radiansOffset = MathF.Sin(s * 0.5f) * MathHelper.ToRadians(45);
                Vector2 shiftedVelocity = baseCaveDirection.RotatedBy(radiansOffset);
                caveVelocity = shiftedVelocity;

                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(7, 25), -1, ignoreTileType: ignoreTile);
                }

                //Make Stalagtites
                if (genRand.NextBool(2))
                {
                    Vector2D endOffset = new Vector2D(
                        genRand.Next(-10, 10),
                        genRand.Next(-20, -3));
                    Vector2 spikePosition = cavePosition;
                    spikePosition += new Vector2(0, -10);
                    GenerateIceSpike(spikePosition, width: 25, endOffset);
                }

                //Make Stalagmites
                if (genRand.NextBool(4))
                {
                    Vector2D endOffset = new Vector2D(
                        genRand.Next(-10, 10),
                        genRand.Next(3, 7));
                    Vector2 spikePosition = cavePosition;
                    spikePosition += new Vector2(0, 15);
                    GenerateIceSpike(spikePosition, width: 15, endOffset);
                }

                //Place Walls
                for (int w = 0; w < 5; w++)
                {
                    ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
                    if (genRand.NextBool(2))
                    {
                        wallType = WallID.IceUnsafe;
                    }

                    Vector2 wallVelocity = genRand.NextVector2Circular(32, 32);
                    Vector2 wallPosition = cavePosition + wallVelocity;
                    WorldUtils.Gen(wallPosition.ToPoint(), new Shapes.Circle(4, 4), Actions.Chain(new GenAction[]
                    {
                        new Actions.PlaceWall(wallType),
                        new Actions.Smooth(true)
                    }));
                }


                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
            }
        }

        public static void PlaceMarble(Point granitePoint, Vector2 radiusSize, int caveWidth = 5)
        {
            var genRand = WorldGen.genRand;
            int maxRadius = (int)radiusSize.Y;
            int radius = genRand.Next((int)radiusSize.X, (int)radiusSize.Y);
            float sizeMultiplier = (float)radius / (float)maxRadius;
            WorldUtils.Gen(granitePoint, new Shapes.Circle(radius, radius),
                new Actions.SetTile(TileID.Marble));
            for (int n = 0; n < 150; n++)
            {
                int r = genRand.Next(radius - 30, radius + 16);
                Point tileRunnePoint = granitePoint + genRand.NextVector2CircularEdge(r, r).ToPoint();
                Vector2 strength = new Vector2(3, 4);
                WorldGen.TileRunner(tileRunnePoint.X, tileRunnePoint.Y,
                    genRand.NextFloat(strength.X, strength.Y),
                    genRand.Next(4, 5), -1);
            }
            for (int n = 0; n < 450; n++)
            {
                int r = genRand.Next(radius - 30, radius + 16);
                Point tileRunnePoint = granitePoint + genRand.NextVector2CircularEdge(r, r).ToPoint();
                Vector2 strength = new Vector2(8, 10);
                WorldGen.TileRunner(tileRunnePoint.X, tileRunnePoint.Y,
                    genRand.NextFloat(strength.X, strength.Y),
                    genRand.Next(4, 5), TileID.Marble);
            }

            Vector2 cavePosition = new Vector2(granitePoint.X, granitePoint.Y) - new Vector2(radius, radius / 4);

            //Starting cave direction
            Vector2 baseCaveDirection = Vector2.UnitX;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

            //How much the tile runner is gonna carve out
            Vector2 caveStrength = new Vector2(12, 14);

            //Chance to open up
            int caveSteps = (int)(50f * sizeMultiplier);
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;

            ushort[] wallTypes = new ushort[]
            {
                WallID.MarbleUnsafe,
                WallID.MarbleBlock
            };

            for (int w = 0; w < 800; w++)
            {
                Point shadowOrbPoint = granitePoint + genRand.NextVector2Circular(radius, radius).ToPoint();
                ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
                WorldUtils.Gen(shadowOrbPoint, new Shapes.Circle(4, 4), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceWall(wallType),
                    new Actions.Smooth(true)
                }));
            }


            for (int j = 0; j < caveSteps; j++)
            {

                Vector2 newVelocity = caveVelocity;
                newVelocity.Y += MathF.Sin((float)j * 2f) * 8;
                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
                    WorldUtils.Gen(cavePosition.ToPoint(), new Shapes.Circle(6, 6), Actions.Chain(new GenAction[]
                    {
                        new Actions.PlaceWall(wallType),
                        new Actions.Smooth(true)
                    }));
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(4, 20), -1);
                }

                // Update the cave position.
                cavePosition += newVelocity * caveWidth * 0.5f;
                //  caveStrength *= 0.99f;
            }
        }

        public static void PlaceGranite(Point granitePoint, Vector2 radiusSize, int caveWidth = 5)
        {
            var genRand = WorldGen.genRand;


            int radius = genRand.Next((int)radiusSize.X, (int)radiusSize.Y);
            float sizeMultiplier = (float)radius / (float)radiusSize.Y;
            WorldUtils.Gen(granitePoint, new Shapes.Circle(radius, radius),
                new Actions.SetTile(TileID.Granite));
            for (int n = 0; n < 150; n++)
            {
                int r = genRand.Next(radius - 30, radius + 16);
                Point tileRunnePoint = granitePoint + genRand.NextVector2CircularEdge(r, r).ToPoint();
                Vector2 strength = new Vector2(3, 4);
                WorldGen.TileRunner(tileRunnePoint.X, tileRunnePoint.Y,
                    genRand.NextFloat(strength.X, strength.Y),
                    genRand.Next(4, 5), -1);
            }
            for (int n = 0; n < 450; n++)
            {
                int r = genRand.Next(radius - 30, radius + 16);
                Point tileRunnePoint = granitePoint + genRand.NextVector2CircularEdge(r, r).ToPoint();
                Vector2 strength = new Vector2(8, 10);
                WorldGen.TileRunner(tileRunnePoint.X, tileRunnePoint.Y,
                    genRand.NextFloat(strength.X, strength.Y),
                    genRand.Next(4, 5), TileID.Granite);
            }

            Vector2 cavePosition = new Vector2(granitePoint.X, granitePoint.Y) - new Vector2(radius / 4, radius);

            //Starting cave direction
            Vector2 baseCaveDirection = Vector2.UnitY;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

            //How much the tile runner is gonna carve out
            Vector2 caveStrength = new Vector2(12, 14);

            //Chance to open up
            int caveSteps = (int)(50f * sizeMultiplier);
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;

            ushort[] wallTypes = new ushort[]
            {
                WallID.GraniteUnsafe,
                WallID.GraniteBlock
            };

            for (int w = 0; w < 800; w++)
            {
                Point shadowOrbPoint = granitePoint + genRand.NextVector2Circular(radius, radius).ToPoint();
                ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
                WorldUtils.Gen(shadowOrbPoint, new Shapes.Circle(4, 4), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceWall(wallType),
                    new Actions.Smooth(true)
                }));
            }


            for (int j = 0; j < caveSteps; j++)
            {

                Vector2 newVelocity = caveVelocity;
                newVelocity.X += MathF.Sin((float)j) * 8;
                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
                    WorldUtils.Gen(cavePosition.ToPoint(), new Shapes.Circle(6, 6), Actions.Chain(new GenAction[]
                    {
                        new Actions.PlaceWall(wallType),
                        new Actions.Smooth(true)
                    }));
                    if (genRand.NextBool(3))
                    {
                        WorldUtils.Gen(cavePosition.ToPoint(), new Shapes.Circle(4, 4),
                            new Actions.SetLiquid(type: LiquidID.Water));
                    }

                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(caveStrength.X, caveStrength.Y),
                        genRand.Next(4, 5), -1);
                }

                // Update the cave position.
                cavePosition += newVelocity * caveWidth * 0.5f;
                //  caveStrength *= 0.99f;
            }
        }

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

        public static void GenerateSimpleCaveWall(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, ushort tileToPlace)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;
            Vector2 breakStrength = caveStrength;

            Vector2 startVelocity = caveVelocity;
            Vector2 pullVelocity = pullDirection;

            float sharpness = 1;
            float counter = 0;
            bool shouldBreak = false;
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


                if (shouldBreak)
                {
                    break;
                }

                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    Point wallPoint = cavePosition.ToPoint();
                    WorldUtils.Gen(wallPoint, new Shapes.Circle(8, 8), Actions.Chain(new GenAction[]
                    {
                        new Actions.PlaceWall(tileToPlace),
                        new Actions.Smooth(true)
                    }));
                }

                float tilePercent = VeilGen.TilePercentNoAir(cavePosition.ToPoint(), new Rectangle((int)cavePosition.X, (int)cavePosition.Y, 20, 20), TileID.Dirt, TileID.Stone);
                if (tilePercent < 0.5f && j > caveSteps / 2)
                {
                    shouldBreak = true;
                }
                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
                //  caveStrength *= 0.99f;
            }
        }

        public static void GenerateDuneHole(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1, bool addTile = false)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;
            Vector2 breakStrength = caveStrength;

            Vector2 startVelocity = caveVelocity;
            Vector2 pullVelocity = pullDirection;

            float sharpness = 1;
            float counter = 0;
            ushort t = (ushort)tileToPlace;
            for (int j = 0; j < caveSteps; j++)
            {
                counter++;
                breakStrength *= 0.9995f;


                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    WorldUtils.Gen(new Point((int)cavePosition.X, (int)cavePosition.Y),
                                        new Shapes.Circle(8, 8), new Actions.ClearTile());
                }

                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
                //  caveStrength *= 0.99f;
            }
        }
        public static void GenerateDuneHoleEdges(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1, bool addTile = false)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;
            Vector2 breakStrength = caveStrength;

            Vector2 startVelocity = caveVelocity;
            Vector2 pullVelocity = pullDirection;

            float sharpness = 1;
            float counter = 0;
            ushort t = (ushort)tileToPlace;
            for (int j = 0; j < caveSteps; j++)
            {

                counter++;
                breakStrength *= 0.9995f;


                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    if (j > 6)
                    {


                        WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                                              genRand.NextFloat(breakStrength.X, breakStrength.Y),
                                              genRand.Next(4, 5), tileToPlace, addTile);
                    }


                }

                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
                //  caveStrength *= 0.99f;
            }
        }
        public static void GenerateDuneCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1, bool addTile = false)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;
            Vector2 breakStrength = caveStrength;

            Vector2 startVelocity = caveVelocity;
            Vector2 pullVelocity = pullDirection;

            float sharpness = 1;
            float counter = 0;
            ushort t = (ushort)tileToPlace;
            for (int j = 0; j < caveSteps; j++)
            {
                if (t == TileID.Sandstone && j < 16)
                    continue;
                counter++;
                breakStrength *= 0.9995f;


                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {

                    if (tileToPlace == -1)
                    {
                        WorldUtils.Gen(new Point((int)cavePosition.X, (int)cavePosition.Y),
                            new Shapes.Circle(8, 8), new Actions.ClearTile());
                    }
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(breakStrength.X, breakStrength.Y),
                        genRand.Next(4, 5), tileToPlace, addTile);
                }

                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
                //  caveStrength *= 0.99f;
            }
        }
        public static void GenerateSimpleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1, bool addTile = false)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;
            Vector2 breakStrength = caveStrength;

            Vector2 startVelocity = caveVelocity;
            Vector2 pullVelocity = pullDirection;

            float sharpness = 1;
            float counter = 0;
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

                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(breakStrength.X, breakStrength.Y),
                        genRand.Next(4, 5), tileToPlace, addTile);
                }

                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
                //  caveStrength *= 0.99f;
            }
        }
        public static void GenerateDuneCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;
            Vector2 breakStrength = caveStrength;

            Vector2 startVelocity = caveVelocity;
            Vector2 pullVelocity = pullDirection;

            float sharpness = 1;
            float counter = 0;
            bool shouldBreak = false;
            for (int j = 0; j < caveSteps; j++)
            {

                counter++;
                breakStrength *= 0.9995f;

                float tilePercent = VeilGen.TilePercentNoAir(cavePosition.ToPoint(), new Rectangle((int)cavePosition.X, (int)cavePosition.Y, 20, 20), TileID.Dirt, TileID.Stone);

                if (shouldBreak)
                    break;

                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(breakStrength.X, breakStrength.Y),
                        genRand.Next(4, 5), tileToPlace);
                }

                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;


                if (tilePercent < 0.5f && j > caveSteps / 2)
                {
                    shouldBreak = true;
                }
            }
        }

        public static void GenerateStraightCaveWall(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, ushort tileToPlace)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;
            Vector2 breakStrength = caveStrength;

            Vector2 startVelocity = caveVelocity;
            Vector2 pullVelocity = pullDirection;

            float sharpness = 1;
            float counter = 0;
            bool shouldBreak = false;
            for (int j = 0; j < caveSteps; j++)
            {

                counter++;
                breakStrength *= 0.9995f;


                if (shouldBreak)
                {
                    break;
                }

                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    Point wallPoint = cavePosition.ToPoint();
                    WorldUtils.Gen(wallPoint, new Shapes.Circle(8, 8), Actions.Chain(new GenAction[]
                    {
                        new Actions.PlaceWall(tileToPlace),
                        new Actions.Smooth(true)
                    }));
                }

                float tilePercent = VeilGen.TilePercentNoAir(cavePosition.ToPoint(), new Rectangle((int)cavePosition.X, (int)cavePosition.Y, 20, 20), TileID.Dirt, TileID.Stone);
                if (tilePercent < 0.5f && j > caveSteps / 2)
                {
                    shouldBreak = true;
                }
                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
                //  caveStrength *= 0.99f;
            }
        }

        public static void GenerateStraightCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;
            Vector2 breakStrength = caveStrength;

            Vector2 startVelocity = caveVelocity;
            Vector2 pullVelocity = pullDirection;

            float sharpness = 1;
            float counter = 0;
            bool shouldBreak = false;
            for (int j = 0; j < caveSteps; j++)
            {

                counter++;
                breakStrength *= 0.9995f;
                float tilePercent = VeilGen.TilePercentNoAir(cavePosition.ToPoint(), new Rectangle((int)cavePosition.X, (int)cavePosition.Y, 20, 20), TileID.Dirt, TileID.Stone);

                if (shouldBreak)
                    break;

                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(breakStrength.X, breakStrength.Y),
                        genRand.Next(4, 5), tileToPlace);
                }

                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;


                if (tilePercent < 0.5f && j > caveSteps / 2)
                {
                    shouldBreak = true;
                }
            }
        }

        public static void GenerateSimpleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;
            Vector2 breakStrength = caveStrength;

            Vector2 startVelocity = caveVelocity;
            Vector2 pullVelocity = pullDirection;

            float sharpness = 1;
            float counter = 0;
            bool shouldBreak = false;
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

                float tilePercent = VeilGen.TilePercentNoAir(cavePosition.ToPoint(), new Rectangle((int)cavePosition.X, (int)cavePosition.Y, 20, 20), TileID.Dirt, TileID.Stone);

                if (shouldBreak)
                    break;

                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(breakStrength.X, breakStrength.Y),
                        genRand.Next(4, 5), tileToPlace);
                }

                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;


                if (tilePercent < 0.5f && j > caveSteps / 2)
                {
                    shouldBreak = true;
                }
            }
        }

        public static void GenerateSimpleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps)
        {
            var genRand = WorldGen.genRand;
            int caveSeed = genRand.Next();

            //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
            float i = cavePosition.X;
            Vector2 caveVelocity = baseCaveDirection;
            Vector2 breakStrength = caveStrength;

            Vector2 startVelocity = caveVelocity;
            Vector2 pullVelocity = pullDirection;

            float sharpness = 1;
            float counter = 0;
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

                if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                {
                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                        genRand.NextFloat(breakStrength.X, breakStrength.Y),
                        genRand.Next(4, 5), -1);
                }

                // Update the cave position.
                cavePosition += caveVelocity * caveWidth * 0.5f;
                //  caveStrength *= 0.99f;
            }
        }
        public static void GenerateSquiggleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
        {
            var genRand = WorldGen.genRand;

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

                if (genRand.NextBool(clearingDenominator) && j > caveSteps / 2)
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
        public static void GenerateLinearCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
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
        public static float TilePercentNoAir(Point tilePoint, Rectangle size, params ushort[] tileIDs)
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
                        if (tile.HasTile)
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
                        if (!WorldGen.SolidTile(x, y))
                        {
                            count++;
                        }

                        if (tile.HasTile && tile.TileType == tileID)
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
        public static void GenerateColosseum(Point tilePoint)
        {
            var genRand = WorldGen.genRand;
            string GetMiniStructurePath()
            {
                int num = genRand.Next(1, 3);
                string baseStructurePath = $"Struct/Colosseum/SquareHouse{num}";
                return baseStructurePath;
            }

            string GetStructurePath()
            {
                int num = genRand.Next(1, 5);
                string baseStructurePath = $"Struct/Colosseum/House{num}";
                return baseStructurePath;
            }

            int[] tileBlend = new int[]
            {
                TileID.RubyGemspark
            };

            void Arena(Point tilePoint)
            {
                var structure = "Struct/Colosseum/TheColosseum";
                Rectangle rectangle = Structurizer.ReadRectangle(structure);
                rectangle.Location = tilePoint;
                Structurizer.ReadStruct(tilePoint, structure, tileBlend);
                Structurizer.ProtectStructure(tilePoint, structure);
                for (int beamX = rectangle.Location.X;
                 beamX < rectangle.Location.X + rectangle.Width; beamX += 8)
                {
                    //Place beams
                    int beamY = rectangle.Location.Y;
                    Tile tile = Main.tile[beamX, beamY];
                    if (!tile.HasTile)
                        continue;

                    int solidCount = 0;
                    while (solidCount < 5)
                    {
                        tile = Main.tile[beamX, beamY];
                        if (!tile.HasTile)
                        {
                            WorldGen.PlaceTile(beamX, beamY, TileID.SandstoneColumn);
                        }
                        else
                        {
                            solidCount++;
                        }
                        beamY++;
                    }
                }
            }
            void PlaceAir(Point tilePoint)
            {
                string structure = "Struct/Colosseum/Elevator";
                Rectangle rectangle = Structurizer.ReadRectangle(structure);
                rectangle.Location = tilePoint;
                var chestIndices = Structurizer.ReadStruct(tilePoint, structure, tileBlend);
                Structurizer.ProtectStructure(tilePoint, structure);
            }

            void PlaceBigStructure(Point tilePoint)
            {
                string structure = GetStructurePath();
                Rectangle rectangle = Structurizer.ReadRectangle(structure);
                rectangle.Location = tilePoint;
                var chestIndices = Structurizer.ReadStruct(tilePoint, structure, tileBlend);
                if (chestIndices.Length != 0)
                {
                    foreach (int chestIndex in chestIndices)
                    {
                        if (chestIndex == -1)
                            continue;
                        Chest chest = Main.chest[chestIndex];
                        var itemsToAdd = new List<(int type, int stack)>();

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

                for (int beamX = rectangle.Location.X;
                    beamX < rectangle.Location.X + rectangle.Width; beamX += 8)
                {
                    //Place beams
                    int beamY = rectangle.Location.Y;
                    Tile tile = Main.tile[beamX, beamY];
                    if (!tile.HasTile)
                        continue;
                    int solidCount = 0;
                    while (solidCount < 5)
                    {
                        tile = Main.tile[beamX, beamY];
                        if (!tile.HasTile)
                        {
                            WorldGen.PlaceTile(beamX, beamY, TileID.SandstoneColumn);
                        }
                        else
                        {
                            solidCount++;
                        }
                        beamY++;
                    }
                }
                Structurizer.ProtectStructure(tilePoint, structure);
            }
            void PlaceSmallStructure(Point tilePoint)
            {
                string structure = GetMiniStructurePath();
                Rectangle rectangle = Structurizer.ReadRectangle(structure);
                rectangle.Location = tilePoint;
                var chestIndices = Structurizer.ReadStruct(tilePoint, structure, tileBlend);
                if (chestIndices.Length != 0)
                {
                    foreach (int chestIndex in chestIndices)
                    {
                        if (chestIndex == -1)
                            continue;
                        Chest chest = Main.chest[chestIndex];
                        var itemsToAdd = new List<(int type, int stack)>();

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

                for (int beamX = rectangle.Location.X;
                    beamX < rectangle.Location.X + rectangle.Width; beamX += 8)
                {
                    //Place beams
                    int beamY = rectangle.Location.Y;
                    Tile tile = Main.tile[beamX, beamY];
                    if (!tile.HasTile)
                        continue;
                    int solidCount = 0;
                    while (solidCount < 5)
                    {
                        tile = Main.tile[beamX, beamY];
                        if (!tile.HasTile)
                        {
                            WorldGen.PlaceTile(beamX, beamY, TileID.SandstoneColumn);
                        }
                        else
                        {
                            solidCount++;
                        }
                        beamY++;
                    }
                }
            }
            PlaceAir(tilePoint + new Point(48, 100));
            PlaceAir(tilePoint + new Point(50, 100));
            //Layer 1
            int rightSideOffset = 61;
            int upOffset = 18;
            PlaceBigStructure(tilePoint);
            PlaceBigStructure(tilePoint + new Point(24, 0));
            PlaceBigStructure(tilePoint + new Point(24 + 32, 0));
            PlaceBigStructure(tilePoint + new Point(24 + 32 + 24, 0));

            tilePoint.Y -= upOffset;
            PlaceBigStructure(tilePoint);
            PlaceBigStructure(tilePoint + new Point(24, 0));
            PlaceBigStructure(tilePoint + new Point(24 + 32, 0));
            PlaceBigStructure(tilePoint + new Point(24 + 32 + 24, 0));


            tilePoint.Y -= upOffset;
            PlaceBigStructure(tilePoint + new Point(4, 0));
            PlaceBigStructure(tilePoint + new Point(24 + 4, 0));
            PlaceBigStructure(tilePoint + new Point(24 + 32 - 4, 0));
            PlaceBigStructure(tilePoint + new Point(24 + 32 + 24 - 4, 0));

            tilePoint.Y -= upOffset;
            PlaceSmallStructure(tilePoint + new Point(34, 0));
            PlaceSmallStructure(tilePoint + new Point(52, 0));

            tilePoint.Y -= upOffset;
            PlaceSmallStructure(tilePoint + new Point(16, 1));
            PlaceSmallStructure(tilePoint + new Point(34, 1));
            PlaceSmallStructure(tilePoint + new Point(52, 1));
            PlaceSmallStructure(tilePoint + new Point(70, 1));

            tilePoint.Y -= upOffset;
            Arena(tilePoint + new Point(-21, -1));

            /*
            //Layer 6
          
            */
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

            for (int t = 0; t < tunnelLength; t++)
            {
                string structure = GetStructurePath();
                Rectangle rectangle = Structurizer.ReadRectangle(structure);
                rectangle.Location = tilePoint;
                if (TilePercent(tilePoint, rectangle, TileID.Dirt, TileID.Stone) < 0.7f)
                {
                    break;
                }

                int[] chestIndices = Structurizer.ReadStruct(tilePoint, structure, null);
                if (chestIndices.Length != 0)
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
                                    itemsToAdd.Add((ItemID.LavaCharm, 1));
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
                            switch (genRand.Next(0, 6))
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
                                case 5:
                                    itemsToAdd.Add((ItemID.ObsidianSkinPotion, genRand.Next(1, 3)));
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

                if (tileDirection.X != 0)
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
                }
                else if (genRand.NextBool(2) && tileDirection != new Point(1, 0))
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
                float divisor = 2f;
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
    }
}
