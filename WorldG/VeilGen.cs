using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;

namespace Stellamod.WorldG
{
    internal class VeilGenTester : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.useTime = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 18;
        }

        public override bool? UseItem(Player player)
        {
            GenerateVeinyCave();
            return base.UseItem(player);
        }

        private void GenerateLinearCave()
        {
            Vector2 mouseWorld = Main.MouseWorld;
            int tileX = (int)Main.MouseWorld.X / 16;
            int tileY = (int)Main.MouseWorld.Y / 16;
            int caveWidth = 4;
            int caveSteps = 120;

            //Cave position in tiles
            Vector2 cavePosition = new Vector2(tileX, tileY);

            //Starting cave direction
            Vector2 baseCaveDirection = Vector2.UnitY;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

            //How much the tile runner is gonna carve out
            Vector2 caveStrength = new Vector2(5, 10);

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
                float divisor = 2f;
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
