using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.AlcadChests;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Armors.Alcalite;
using Stellamod.Items.Armors.Stone;
using Stellamod.Items.Armors.Windmillion;
using Stellamod.Items.Consumables;
using Stellamod.Items.Flasks;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Items.Placeable;
using Stellamod.Items.Quest.Merena;
using Stellamod.Items.Special;
using Stellamod.Items.Special.MinerLogs;
using Stellamod.Items.Tools;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Melee.Greatswords;
using Stellamod.Items.Weapons.Melee.Spears;
using Stellamod.Items.Weapons.Melee.Yoyos;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Ranged.Crossbows;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.Items.Weapons.Whips;
using Stellamod.Tiles;
using Stellamod.Tiles.Abyss;
using Stellamod.Tiles.Acid;
using Stellamod.Tiles.Illuria;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;


namespace Stellamod.WorldG
{


    public class StellaWorld : ModSystem
    {

        private int _index;
        public static bool SoulStorm;
        private void Add(List<GenPass> tasks)
        {

        }
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int biomeSpots = tasks.FindIndex(genpass => genpass.Name.Equals("Marble"));
            if (biomeSpots != -1)
            {
                tasks.Insert(biomeSpots + 1, new PassLegacy("World Gen Abysm", WorldGenAbysm));

            }
            int SnowGen = tasks.FindIndex(genpass => genpass.Name.Equals("Generate Ice Biome"));
            if (SnowGen != -1)
            {

                tasks.Insert(SnowGen + 1, new PassLegacy("Smooth Ice", WorldGenSmoothIceBiome));
                //   Add(new PassLegacy("World Gen Alcad", WorldGenAlcadSpot));

            }

            int OreGen = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            if (OreGen != -1)
            {

                tasks.Insert(OreGen + 1, new PassLegacy("World Gen Other stones", WorldGenDarkstone));
                tasks.Insert(OreGen + 2, new PassLegacy("World Gen Ice Ores", WorldGenFrileOre));
                tasks.Insert(OreGen + 3, new PassLegacy("World Gen Starry Ores", WorldGenArncharOre));
                tasks.Insert(OreGen + 4, new PassLegacy("World Gen Flame Ores", WorldGenFlameOre));
                tasks.Insert(OreGen + 5, new PassLegacy("World Gen Flame Ores", WorldGenVeriplantBlobs));
                tasks.Insert(OreGen + 6, new PassLegacy("World Gen Cinderspark", WorldGenCinderspark));
                tasks.Insert(OreGen + 7, new PassLegacy("World Gen Cinderspark", WorldGenArncharOre2));
                tasks.Insert(OreGen + 8, new PassLegacy("World Gen Ice Ores", WorldGenFrileOre));
                tasks.Insert(OreGen + 9, new PassLegacy("World Gen Royal Capital Spot", WorldGenRoyalCapitalSpot));
                tasks.Insert(OreGen + 10, new PassLegacy("World Gen Royal Alcad", WorldGenAlcadSpot));
                tasks.Insert(OreGen + 11, new PassLegacy("World Gen Virulent", WorldGenVirulent));
                //   Add(new PassLegacy("World Gen Alcad", WorldGenAlcadSpot));

            }

            int CathedralGen3 = tasks.FindIndex(genpass => genpass.Name.Equals("Buried Chests"));
            if (CathedralGen3 != -1)
            {
                tasks.Insert(CathedralGen3 + 1, new PassLegacy("World Gen Ambience", WorldGenAmbience));
            }

            //Generate all of our structures before floating islands.
            //This should prevent them from ending up in weird spots
            int CathedralGen2 = tasks.FindIndex(genpass => genpass.Name.Equals("Full Desert"));
            _index = CathedralGen2 + 1;

            void Add(GenPass pass)
            {
                tasks.Insert(_index, pass);
                _index++;
            }

            if (CathedralGen2 != -1)
            {

            }
            int structuresGen = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            _index = structuresGen + 1;
            if (structuresGen != -1)
            {
                Add(new PassLegacy("World Gen Royal Castle", WorldGenRoyalCapital));
                Add(new PassLegacy("World Gen Illuria", WorldGenIlluria));
                Add(new PassLegacy("World Gen Veldris", WorldGenVeldris));
                Add(new PassLegacy("World Gen Veil Underground", WorldGenVU));

                Add(new PassLegacy("World Gen Abandoned Mineshafts", WorldGenAbandonedMineshafts));
                Add(new PassLegacy("World Gen AureTemple", WorldGenAurelusTemple));
                Add(new PassLegacy("World Gen Fable", WorldGenFabiliaRuin));
                Add(new PassLegacy("World Gen Morrowed Structures", WorldGenMorrowedStructures));
                Add(new PassLegacy("World Gen More skies", WorldGenBig));
                Add(new PassLegacy("World Gen More skies", WorldGenMed));
                Add(new PassLegacy("World Gen Virulent Structures", WorldGenVirulentStructures));
                Add(new PassLegacy("World Gen Govheil Castle", WorldGenGovheilCastle));
                Add(new PassLegacy("World Gen Stone Castle", WorldGenStoneCastle));

                Add(new PassLegacy("World Gen Cathedral", WorldGenCathedral));
                Add(new PassLegacy("World Gen Underworld rework", WorldGenUnderworldSpice));
                Add(new PassLegacy("World Gen Catacombs Fire", WorldGenCatacombsFlames));
                Add(new PassLegacy("World Gen Catacombs Trap", WorldGenCatacombsTrap));
                Add(new PassLegacy("World Gen Catacombs Water 1", WorldGenCatacombsWater));
                Add(new PassLegacy("World Gen Catacombs Water 2", WorldGenCatacombsWater2));
                Add(new PassLegacy("World Gen Sylia", WorldGenSylia));
                Add(new PassLegacy("World Gen Rallad", WorldGenRallad));
                Add(new PassLegacy("World Gen Xix Village", WorldGenXixVillage));
                Add(new PassLegacy("World Gen Windmills Village", WorldGenWindmills));
                Add(new PassLegacy("World Gen Mechanic spot", WorldGenMechShop));
                Add(new PassLegacy("World Gen Gia's House", WorldGenGiaHouse));
                Add(new PassLegacy("World Gen Bridget", WorldGenBridget));
                Add(new PassLegacy("World Gen Bridget", WorldGenFabledTrees));


                Add(new PassLegacy("World Gen Worshiping Towers", WorldGenWorshipingTowers));
                Add(new PassLegacy("World Gen GothiviaTower", WorldGenTG));
                Add(new PassLegacy("World Gen SigfriedTower", WorldGenTS));
                Add(new PassLegacy("World Gen AzurerinTower", WorldGenTA));
                Add(new PassLegacy("World Gen CozmireTower", WorldGenTC));
                Add(new PassLegacy("World Gen CozmireTower", WorldGenTL));
                Add(new PassLegacy("World Gen Dread Monoliths", WorldGenDreadMonoliths));
                Add(new PassLegacy("World Gen Sunstalker", WorldGenStalker));
                Add(new PassLegacy("World Gen Manor", WorldGenManor));
            }

        }

        private void WorldGenSmoothIceBiome(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Smoothing The Ice Surface";
            int levels = 1000;

            Point snowBounds = SnowBounds();
            SmoothenSurface(snowBounds.X, snowBounds.Y, levels);
        }

        private Point AbyssBounds()
        {
            int min = Main.maxTilesX - 1;
            int max = 0;
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile)
                    {
                        if (tile.TileType == ModContent.TileType<AbyssalDirt>())
                        {
                            min = Math.Min(min, x);
                            max = Math.Max(max, x);
                        }
                    }
                }
            }
            return new Point(min, max);
        }
        private float FillPercent(Rectangle rect)
        {
            int totalSpace = rect.Width * rect.Height;
            int filledTiles = 0;
            for (int x = rect.Left; x < rect.Right; x++)
            {
                for (int y = rect.Top; y < rect.Bottom; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (!tile.HasTile)
                        continue;
                    if (!WorldGen.SolidTile(x, y))
                        continue;
                    filledTiles++;
                }
            }
            float pct = filledTiles / (float)totalSpace;
            return pct;
        }

        private void WorldGenFabledTrees(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "The Veiled people planting trees!";
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.4f) * 6E-02); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next(0, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == ModContent.TileType<CatagrassBlock>())
                {
                    WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Fable.FableTreeSapling>());
                }
            }
        }

        private void WorldGenAmbience(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Golden Ambience ruining the world";
            int[] placeableOnDirt = new int[]
            {
                ModContent.TileType<Tiles.Ambient.OwlTrunck1>(),
                ModContent.TileType<Tiles.Ambient.OwlTrunck2>(),
                ModContent.TileType<Tiles.Ambient.OwlTrunck3>(),
                ModContent.TileType<Tiles.Ambient.TreeOver1>(),
                ModContent.TileType<Tiles.Ambient.TreeOver2>(),
                ModContent.TileType<Tiles.Ambient.TreeOver3>()
            };

            //Stone and clayblocks
            int[] placeableOnStone = new int[]
            {
                ModContent.TileType<Tiles.Ambient.BigRock1>(),
                ModContent.TileType<Tiles.Ambient.BigRock2>(),
                ModContent.TileType<Tiles.Ambient.BigRock3>(),
                ModContent.TileType<Tiles.Ambient.BigRock4>(),
                ModContent.TileType<Tiles.Ambient.Stalagmite1>(),
                ModContent.TileType<Tiles.Ambient.Stalagmite2>(),
                ModContent.TileType<Tiles.Ambient.Stalagmite3>(),
                ModContent.TileType<Tiles.Ambient.Stalagmite4>(),
                ModContent.TileType<Tiles.Ambient.Mushroom1>(),
                ModContent.TileType<Tiles.Ambient.Mushroom2>(),
                ModContent.TileType<Tiles.Ambient.Mushroom3>()
            };

            int[] placeableOnMud = new int[]
            {
                ModContent.TileType<Tiles.Structures.LogS>()
            };


            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.UnderworldLayer; y++)
                {
                    int yBelow = y + 1;
                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile)
                        continue;
                    if (!WorldGen.SolidTile(x, yBelow))
                        continue;
                    int denom = 24;
                    int[] pool = null;
                    Tile tileBelow = Main.tile[x, yBelow];
                    switch (tileBelow.TileType)
                    {
                        case TileID.Dirt:
                        case TileID.Grass:
                            pool = placeableOnDirt;
                            break;
                        case TileID.ClayBlock:
                        case TileID.Stone:
                            pool = placeableOnStone;
                            denom = 6;
                            break;
                        case TileID.Mud:
                        case TileID.JungleGrass:
                            pool = placeableOnMud;
                            break;
                    }

                    if (pool == null)
                        continue;
                    if (!WorldGen.genRand.NextBool(denom))
                        continue;
                    int type = pool[WorldGen.genRand.Next(0, pool.Length)];
                    WorldGen.PlaceObject(x, y, type);
                }
            }

        }
        Point pointVeri;
        Point pointAlcadthingy;
        private void WorldGenFabiliaRuin(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Buring the landscape with Cinder and Fable";


            int[] tileBlend = new int[]
            {
                TileID.RubyGemspark
            };

            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 10000000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int smx = WorldGen.genRand.Next(((Main.maxTilesX) / 2) + 50, (Main.maxTilesX / 2) + 200); // from 50 since there's a unaccessible area at the world's borders
                                                                                                          // 50% of choosing the last 6th of the world
                                                                                                          //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                int smy = ((int)(Main.worldSurface - 250));

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
                {
                    smy++;
                }

                // If we went under the world's surface, try again
                if (smy > Main.worldSurface - 20)
                {
                    continue;
                }

                Tile tile = Main.tile[smx, smy];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == TileID.Sand
                    || tile.TileType == TileID.Dirt
                    || tile.TileType == TileID.Grass
                    || tile.TileType == TileID.Stone
                    || tile.TileType == TileID.Sandstone))
                {
                    continue;
                }



                for (int da = 0; da < 1; da++)
                {
                    Point Loc = new Point(smx + 10, smy + 340);
                    NPCs.Town.AlcadSpawnSystem.FableTile = Loc;

                    //This code just places
                    int width = 253;
                    int height = 50;
                    ShapeData shapeData = new ShapeData();
                    Point dirtLoc = Loc;
                    dirtLoc.Y -= 338;
                    WorldUtils.Gen(dirtLoc, new Shapes.Rectangle(width, height), new Actions.Blank().Output(shapeData));
                    WorldUtils.Gen(dirtLoc, new ModShapes.All(shapeData), new Actions.SetTile(TileID.Dirt, true));
                    WorldUtils.Gen(dirtLoc, new ModShapes.All(shapeData), new Actions.Smooth());

                    StructureLoader.ReadStruct(Loc, "Struct/Morrow/FableBiomeNew", tileBlend);
                    Point Loc2 = new Point(smx + 10, smy + 380);
                    WorldGen.digTunnel(Loc2.X - 10, Loc2.Y + 10, 1, 0, 1, 10, false);

                    Point Loc22 = new Point(smx + 10, smy - 33);
                    StructureLoader.ReadStruct(Loc22, "Struct/Morrow/Morrowtop");

                    pointVeri = new Point(smx + 10, smy + 500);
                    Point Loc4 = new Point(smx + 233, smy + 45);
                    Point Loc5 = new Point(smx + 10, smy + 45);

                    StructureLoader.ProtectStructure(Loc, "Struct/Morrow/FableBiomeNew");
                    StructureLoader.ProtectStructure(Loc22, "Struct/Morrow/Morrowtop");
                    placed = true;
                }
            }
        }



        private void WorldGenManor(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Ereshkigal secretly hiding Sigfried";


            int[] tileBlend = new int[]
            {
                TileID.RubyGemspark
            };

            int[] tileBlend2 = new int[]
            {
                TileID.Stone
            };

            bool placed = false;
            int attempts = 0;
            Point manorPoint = new Point();
            while (!placed && attempts++ < 10000000)
            {


                int smx = Main.maxTilesX / 2;
                int smy = (Main.UnderworldLayer - (Main.maxTilesY / 20));
                Point Loc = new Point(smx, smy);
                manorPoint = Loc;
                //StructureLoader.ReadStruct(Loc, "Struct/Underground/Manor", tileBlend);
                string path = "Struct/Underground/Manor";//
                NPCs.Town.AlcadSpawnSystem.OrdinTile = Loc;
                int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path, tileBlend);
                StructureLoader.ProtectStructure(Loc, path);
                foreach (int chestIndex in ChestIndexs)
                {
                    var chest = Main.chest[chestIndex];
                    // etc

                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                    var itemsToAdd = new List<(int type, int stack)>();

                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                        Tuple.Create(ModContent.ItemType<VerianBar>(), 0.5)


                    // Choose no item with a high weight of 7.
                    );
                    if (specialItem != ItemID.None)
                    {
                        itemsToAdd.Add((specialItem, 1));
                    }
                    // Using a switch statement and a random choice to add sets of items.
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            itemsToAdd.Add((ModContent.ItemType<CinderedCard>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner5>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                            itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                            break;
                        case 1:
                            itemsToAdd.Add((ModContent.ItemType<Volcant>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianBar>(), Main.rand.Next(1, 10)));
                            itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner5>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                            itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                            break;
                        case 2:
                            itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                            itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                            break;
                        case 3:
                            itemsToAdd.Add((ModContent.ItemType<CinderNeedle>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                            itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner5>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                            break;
                    }

                    // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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
                placed = true;
            }



            Point ishtarLocation = manorPoint + new Point( - 500, -100);
            string path2 = "Struct/Underground/Ishtar";//
            attempts = 0;
            /*
            while(!StructureLoader.TryPlaceAndProtectStructure(ishtarLocation, path2))
            {
                ishtarLocation.X--;
                attempts++;
                if (attempts >= 1000)
                    break;
            }*/

            int[] ChestIndexs2 = StructureLoader.ReadStruct(ishtarLocation, path2, tileBlend2);
            NPCs.Town.AlcadSpawnSystem.IshPinTile = ishtarLocation;
            NPCs.Town.AlcadSpawnSystem.EreshTile = ishtarLocation;
            NPCs.Town.AlcadSpawnSystem.PULSETile = ishtarLocation;

            StructureLoader.ProtectStructure(ishtarLocation, path2);
            foreach (int chestIndex in ChestIndexs2)
            {
                var chest = Main.chest[chestIndex];
                // etc

                // itemsToAdd will hold type and stack data for each item we want to add to the chest
                var itemsToAdd = new List<(int type, int stack)>();

                // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                    Tuple.Create(ModContent.ItemType<IshtarCandle>(), 0.5)


                // Choose no item with a high weight of 7.
                );
                if (specialItem != ItemID.None)
                {
                    itemsToAdd.Add((specialItem, 1));
                }
                // Using a switch statement and a random choice to add sets of items.
                switch (Main.rand.Next(5))
                {
                    case 0:
                        itemsToAdd.Add((ModContent.ItemType<IshtarCard>(), Main.rand.Next(1, 1)));
                        itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                        itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                        itemsToAdd.Add((ItemID.RagePotion, Main.rand.Next(1, 3)));
                        break;
                    case 1:
                        itemsToAdd.Add((ModContent.ItemType<ImperfectionStaff>(), Main.rand.Next(1, 1)));
                        itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                        itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                        itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                        break;
                    case 2:
                        itemsToAdd.Add((ModContent.ItemType<RazzleDazzle>(), Main.rand.Next(1, 1)));
                        itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                        itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                        itemsToAdd.Add((ItemID.FlipperPotion, Main.rand.Next(1, 3)));
                        break;
                    case 3:
                        itemsToAdd.Add((ModContent.ItemType<PoisonPistol>(), Main.rand.Next(1, 1)));
                        itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                        itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                        itemsToAdd.Add((ItemID.NightOwlPotion, Main.rand.Next(1, 3)));

                        break;
                    case 4:
                        itemsToAdd.Add((ModContent.ItemType<EreshkinPowder>(), Main.rand.Next(1, 1)));
                        itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                        itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                        itemsToAdd.Add((ItemID.NightOwlPotion, Main.rand.Next(1, 3)));
                        break;




                }

                // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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





        private void WorldGenStoneCastle(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Creating life near spawn :)";

            int[] tileBlend = new int[]
            {
                TileID.RubyGemspark
            };

            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 10000000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int smx = WorldGen.genRand.Next((Main.maxTilesX / 2) - 200, (Main.maxTilesX / 2) - 150); // from 50 since there's a unaccessible area at the world's borders
                                                                                                         // 50% of choosing the last 6th of the world
                                                                                                         // Choose which side of the world to be on randomly
                ///if (WorldGen.genRand.NextBool())
                ///{
                ///	towerX = Main.maxTilesX - towerX;
                ///}

                //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                int smy = ((int)(Main.worldSurface - 250));

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
                {
                    smy++;
                }

                // If we went under the world's surface, try again
                if (smy > Main.worldSurface - 20)
                {
                    continue;
                }
                Tile tile = Main.tile[smx, smy];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == TileID.Sand
                    || tile.TileType == TileID.Dirt
                    || tile.TileType == ModContent.TileType<VeriplantGrass>()
                    || tile.TileType == TileID.Grass
                    || tile.TileType == TileID.Stone
                    || tile.TileType == TileID.Sandstone))
                {
                    continue;
                }


                // place the Rogue
                //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                //Main.npc[num].homeTileX = -1;
                //	Main.npc[num].homeTileY = -1;
                //	Main.npc[num].direction = 1;
                //	Main.npc[num].homeless = true;



                for (int da = 0; da < 1; da++)
                {
                    Point Loc = new Point(smx, smy + 450);
                    string path = "Struct/Underground/StoneGolem";//
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path, tileBlend);
                    StructureLoader.ProtectStructure(Loc, path);
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                            Tuple.Create(ModContent.ItemType<StoniaBroochA>(), 0.5)


                        // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(5))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<Gutinier>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.ShinyRedBalloon, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;
                            case 2:
                                itemsToAdd.Add((ItemID.EndlessQuiver, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;
                            case 3:
                                itemsToAdd.Add((ItemID.SlimeStaff, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));

                                break;
                            case 4:
                                itemsToAdd.Add((ModContent.ItemType<StoniaHat>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<StoniaBoots>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<StoniaChestplate>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;




                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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

                placed = true;


            }

        }


        private void WorldGenXixVillage(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Witches spreading love all inside you!";



            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 10000000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int smx = WorldGen.genRand.Next((Main.maxTilesX / 2) - 300, (Main.maxTilesX / 2) - 150); // from 50 since there's a unaccessible area at the world's borders
                                                                                                         // 50% of choosing the last 6th of the world
                                                                                                         // Choose which side of the world to be on randomly
                ///if (WorldGen.genRand.NextBool())
                ///{
                ///	towerX = Main.maxTilesX - towerX;
                ///}

                //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                int smy = ((int)(Main.worldSurface - 200));

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
                {
                    smy++;
                }

                // If we went under the world's surface, try again
                if (smy > Main.worldSurface - 20)
                {
                    continue;
                }
                Tile tile = Main.tile[smx, smy];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == TileID.Sand
                    || tile.TileType == TileID.Dirt
                    || tile.TileType == ModContent.TileType<VeriplantGrass>()
                    || tile.TileType == TileID.Grass
                    || tile.TileType == TileID.Stone
                    || tile.TileType == TileID.Sandstone))
                {
                    continue;
                }


                // place the Rogue
                //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                //Main.npc[num].homeTileX = -1;
                //	Main.npc[num].homeTileY = -1;
                //	Main.npc[num].direction = 1;
                //	Main.npc[num].homeless = true;



                for (int da = 0; da < 1; da++)
                {
                    Point Loc = new Point(smx, smy + 18);
                    Point Loc22 = new Point(smx, smy + 58);
                    string path = "Struct/Overworld/XixVillage";
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path);
                    StructureLoader.ProtectStructure(Loc, path);
                    NPCs.Town.AlcadSpawnSystem.LittleWitchTownTile = Loc;
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                            Tuple.Create(ModContent.ItemType<Ivythorn>(), 0.5)


                        // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(4))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<PerfectionStaff>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.CordageGuide, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;
                            case 2:
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 50)));
                                itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;
                            case 3:
                                itemsToAdd.Add((ItemID.SlimeStaff, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));

                                break;





                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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

                for (int da = 0; da < 1; da++)
                {
                    Point Loc2 = new Point(smx, smy + 19);
                    WorldUtils.Gen(Loc2, new Shapes.Rectangle(125, 20), new Actions.SetTile(TileID.Dirt));



                }
                placed = true;


            }

        }


        private void WorldGenGraving(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "You aren't escaping the Kill Pillars";


            int smx = 0;
            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 10000000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                ; // from 50 since there's a unaccessible area at the world's borders
                switch (Main.rand.Next(2))
                {


                    case 0:
                        {
                            smx = WorldGen.genRand.Next(1000, (Main.maxTilesX / 2) - 350);
                        }




                        break;

                    case 1:
                        {
                            smx = WorldGen.genRand.Next((Main.maxTilesX / 2) + 350, (Main.maxTilesX) - 1000);
                        }




                        break;

                }                                                                                        // 50% of choosing the last 6th of the world
                                                                                                         // Choose which side of the world to be on randomly
                ///if (WorldGen.genRand.NextBool())
                ///{
                ///	towerX = Main.maxTilesX - towerX;
                ///}

                //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                int smy = ((int)(Main.worldSurface - 200));

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
                {
                    smy++;
                }

                // If we went under the world's surface, try again
                if (smy > Main.worldSurface - 20)
                {
                    continue;
                }
                Tile tile = Main.tile[smx, smy];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == TileID.Dirt
                    || tile.TileType == ModContent.TileType<VeriplantGrass>()
                    || tile.TileType == TileID.Grass
                    || tile.TileType == TileID.Stone))
                {
                    continue;
                }


                // place the Rogue
                //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                //Main.npc[num].homeTileX = -1;
                //	Main.npc[num].homeTileY = -1;
                //	Main.npc[num].direction = 1;
                //	Main.npc[num].homeless = true;



                for (int da = 0; da < 1; da++)
                {
                    Point Loc = new Point(smx, smy + 3);

                    string path = "Struct/Overworld/Graving";

                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path);
                    StructureLoader.ProtectStructure(Loc, path);
                    NPCs.Town.AlcadSpawnSystem.DaedenTile = Loc;
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                            Tuple.Create(ModContent.ItemType<Ivythorn>(), 0.5)


                        // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(4))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<PerfectionStaff>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.CordageGuide, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;
                            case 2:
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 50)));
                                itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;
                            case 3:
                                itemsToAdd.Add((ItemID.SlimeStaff, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));

                                break;





                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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

                for (int da = 0; da < 1; da++)
                {
                    Point Loc2 = new Point(smx, smy + 3);
                    WorldUtils.Gen(Loc2, new Shapes.Rectangle(75, 20), new Actions.SetTile(TileID.Grass));



                }
                placed = true;


            }

        }


        private void WorldGenWindmills(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Adding life to the world!";

            for (int k = 0; k < 2; k++)
            {

                bool placed = false;
                int attempts = 0;
                while (!placed && attempts++ < 10000000)
                {
                    // Select a place in the first 6th of the world, avoiding the oceans
                    int smx = WorldGen.genRand.Next(400, (Main.maxTilesX / 3)); // from 50 since there's a unaccessible area at the world's borders
                                                                                // 50% of choosing the last 6th of the world
                                                                                // Choose which side of the world to be on randomly
                    ///if (WorldGen.genRand.NextBool())
                    ///{
                    ///	towerX = Main.maxTilesX - towerX;
                    ///}

                    //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                    int smy = ((int)(Main.worldSurface - 250));

                    // We go down until we hit a solid tile or go under the world's surface
                    while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
                    {
                        smy++;
                    }

                    // If we went under the world's surface, try again
                    if (smy > Main.worldSurface - 20)
                    {
                        continue;
                    }
                    Tile tile = Main.tile[smx, smy];
                    // If the type of the tile we are placing the tower on doesn't match what we want, try again
                    if (!(tile.TileType == TileID.Dirt
                        || tile.TileType == TileID.Stone
                        || tile.TileType == TileID.Grass))
                    {
                        continue;
                    }


                    // place the Rogue
                    //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                    //Main.npc[num].homeTileX = -1;
                    //	Main.npc[num].homeTileY = -1;
                    //	Main.npc[num].direction = 1;
                    //	Main.npc[num].homeless = true;



                    for (int da = 0; da < 1; da++)
                    {
                        Point Loc = new Point(smx, smy + 1);

                        int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Overworld/Windmill");
                        foreach (int chestIndex in ChestIndexs)
                        {
                            var chest = Main.chest[chestIndex];
                            // etc

                            // itemsToAdd will hold type and stack data for each item we want to add to the chest
                            var itemsToAdd = new List<(int type, int stack)>();

                            // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                            int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                Tuple.Create(ModContent.ItemType<Ivythorn>(), 0.5)


                            // Choose no item with a high weight of 7.
                            );
                            if (specialItem != ItemID.None)
                            {
                                itemsToAdd.Add((specialItem, 1));
                            }
                            // Using a switch statement and a random choice to add sets of items.
                            switch (Main.rand.Next(4))
                            {
                                case 0:
                                    itemsToAdd.Add((ModContent.ItemType<WindmillShuriken>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                    break;
                                case 1:
                                    itemsToAdd.Add((ModContent.ItemType<WindmillionRobe>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<WindmillionHat>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<WindmillionBoots>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;
                                case 2:
                                    itemsToAdd.Add((ModContent.ItemType<WindedQuiver>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;
                                case 3:
                                    itemsToAdd.Add((ItemID.BabyBirdStaff, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));

                                    break;




                            }

                            // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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

                    for (int da = 0; da < 1; da++)
                    {
                        Point Loc2 = new Point(smx, smy + 2);
                        WorldUtils.Gen(Loc2, new Shapes.Rectangle(34, 10), new Actions.SetTile(TileID.Grass));



                    }
                    placed = true;


                }




            }

            for (int k = 0; k < 2; k++)
            {

                bool placed = false;
                int attempts = 0;
                while (!placed && attempts++ < 10000000)
                {
                    // Select a place in the first 6th of the world, avoiding the oceans
                    int smx = WorldGen.genRand.Next((Main.maxTilesX) - (Main.maxTilesX / 3), (Main.maxTilesX) - 200); // from 50 since there's a unaccessible area at the world's borders
                                                                                                                      // 50% of choosing the last 6th of the world
                                                                                                                      // Choose which side of the world to be on randomly
                    ///if (WorldGen.genRand.NextBool())
                    ///{
                    ///	towerX = Main.maxTilesX - towerX;
                    ///}

                    //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                    int smy = ((int)(Main.worldSurface - 250));

                    // We go down until we hit a solid tile or go under the world's surface
                    while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
                    {
                        smy++;
                    }

                    // If we went under the world's surface, try again
                    if (smy > Main.worldSurface - 20)
                    {
                        continue;
                    }
                    Tile tile = Main.tile[smx, smy];
                    // If the type of the tile we are placing the tower on doesn't match what we want, try again
                    if (!(tile.TileType == TileID.Dirt
                        || tile.TileType == TileID.Stone
                        || tile.TileType == ModContent.TileType<VeriplantGrass>()
                        || tile.TileType == TileID.Grass))
                    {
                        continue;
                    }


                    // place the Rogue
                    //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                    //Main.npc[num].homeTileX = -1;
                    //	Main.npc[num].homeTileY = -1;
                    //	Main.npc[num].direction = 1;
                    //	Main.npc[num].homeless = true;



                    for (int da = 0; da < 1; da++)
                    {
                        Point Loc = new Point(smx, smy + 1);


                        int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Overworld/Windmill");
                        foreach (int chestIndex in ChestIndexs)
                        {
                            var chest = Main.chest[chestIndex];
                            // etc

                            // itemsToAdd will hold type and stack data for each item we want to add to the chest
                            var itemsToAdd = new List<(int type, int stack)>();

                            // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                            int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                Tuple.Create(ModContent.ItemType<Ivythorn>(), 0.5)


                            // Choose no item with a high weight of 7.
                            );
                            if (specialItem != ItemID.None)
                            {
                                itemsToAdd.Add((specialItem, 1));
                            }
                            // Using a switch statement and a random choice to add sets of items.
                            switch (Main.rand.Next(4))
                            {
                                case 0:
                                    itemsToAdd.Add((ModContent.ItemType<WindmillShuriken>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                    break;
                                case 1:
                                    itemsToAdd.Add((ModContent.ItemType<WindmillionRobe>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<WindmillionHat>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<WindmillionBoots>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;
                                case 2:
                                    itemsToAdd.Add((ModContent.ItemType<WindedQuiver>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;
                                case 3:
                                    itemsToAdd.Add((ItemID.BabyBirdStaff, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));

                                    break;






                            }

                            // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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


                    for (int da = 0; da < 1; da++)
                    {
                        Point Loc2 = new Point(smx, smy + 2);
                        WorldUtils.Gen(Loc2, new Shapes.Rectangle(34, 10), new Actions.SetTile(TileID.Grass));



                    }






                    placed = true;


                }

            }

        }


        private void WorldGenMed(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Building Gintze houses";


            for (int k = 0; k < 3; k++)
            {
                bool placed = false;
                int attempts = 0;
                while (!placed && attempts++ < 1000000)
                {
                    // Select a place in the first 6th of the world, avoiding the oceans
                    int smx = WorldGen.genRand.Next(750, Main.maxTilesX); // from 50 since there's a unaccessible area at the world's borders
                                                                          // 50% of choosing the last 6th of the world
                                                                          // Choose which side of the world to be on randomly
                    ///if (WorldGen.genRand.NextBool())
                    ///{
                    ///	towerX = Main.maxTilesX - towerX;
                    ///}

                    //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                    int smy = ((int)(Main.worldSurface - 200));

                    // We go down until we hit a solid tile or go under the world's surface
                    while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
                    {
                        smy++;
                    }

                    // If we went under the world's surface, try again
                    if (smy > Main.worldSurface - 20)
                    {
                        continue;
                    }
                    Tile tile = Main.tile[smx, smy];
                    // If the type of the tile we are placing the tower on doesn't match what we want, try again
                    if (!(tile.TileType == TileID.Dirt
                        || tile.TileType == ModContent.TileType<Tiles.Acid.AcidialDirt>()

                        || tile.TileType == TileID.Mud))

                    {
                        continue;
                    }


                    // place the Rogue
                    //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                    //Main.npc[num].homeTileX = -1;
                    //	Main.npc[num].homeTileY = -1;
                    //	Main.npc[num].direction = 1;
                    //	Main.npc[num].homeless = true;



                    for (int da = 0; da < 1; da++)
                    {
                        Point Loc = new Point(smx, smy - Main.rand.Next(125, 150));
                        if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Overworld/Overworld2"))
                            continue;
                        int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Overworld/Overworld2");
                        foreach (int chestIndex in ChestIndexs)
                        {
                            var chest = Main.chest[chestIndex];
                            // etc

                            // itemsToAdd will hold type and stack data for each item we want to add to the chest
                            var itemsToAdd = new List<(int type, int stack)>();

                            // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                            int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5)


                            // Choose no item with a high weight of 7.
                            );
                            if (specialItem != ItemID.None)
                            {
                                itemsToAdd.Add((specialItem, 1));
                            }
                            // Using a switch statement and a random choice to add sets of items.
                            switch (Main.rand.Next(9))
                            {
                                case 0:
                                    itemsToAdd.Add((ModContent.ItemType<Gutinier>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                    break;
                                case 1:
                                    itemsToAdd.Add((ItemID.WandofFrosting, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;
                                case 2:
                                    itemsToAdd.Add((ItemID.EndlessQuiver, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;
                                case 3:
                                    itemsToAdd.Add((ItemID.SlimeStaff, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));

                                    break;
                                case 4:
                                    itemsToAdd.Add((ItemID.Diamond, Main.rand.Next(1, 20)));
                                    itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;

                                case 5:
                                    itemsToAdd.Add((ItemID.CloudinaBottle, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;

                                case 6:
                                    itemsToAdd.Add((ItemID.ShinyRedBalloon, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;

                                case 7:
                                    itemsToAdd.Add((ItemID.BandofRegeneration, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;

                                case 8:
                                    itemsToAdd.Add((ItemID.BandofStarpower, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;
                            }

                            // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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

                    placed = true;
                }
            }

        }


        private void WorldGenBig(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Building Gintze houses";


            for (int k = 0; k < 5; k++)
            {
                bool placed = false;
                int attempts = 0;
                while (!placed && attempts++ < 1000000)
                {
                    // Select a place in the first 6th of the world, avoiding the oceans
                    int smx = WorldGen.genRand.Next(1000, (Main.maxTilesX) - 500); // from 50 since there's a unaccessible area at the world's borders
                                                                                   // 50% of choosing the last 6th of the world
                                                                                   // Choose which side of the world to be on randomly
                    ///if (WorldGen.genRand.NextBool())
                    ///{
                    ///	towerX = Main.maxTilesX - towerX;
                    ///}

                    //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                    int smy = ((int)(Main.worldSurface - 200));

                    // We go down until we hit a solid tile or go under the world's surface
                    while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
                    {
                        smy++;
                    }

                    // If we went under the world's surface, try again
                    if (smy > Main.worldSurface - 20)
                    {
                        continue;
                    }
                    Tile tile = Main.tile[smx, smy];
                    // If the type of the tile we are placing the tower on doesn't match what we want, try again
                    if (!(tile.TileType == TileID.Dirt
                        || tile.TileType == TileID.Sand
                        || tile.TileType == TileID.Mud))

                    {
                        continue;
                    }


                    // place the Rogue
                    //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                    //Main.npc[num].homeTileX = -1;
                    //	Main.npc[num].homeTileY = -1;
                    //	Main.npc[num].direction = 1;
                    //	Main.npc[num].homeless = true;



                    for (int da = 0; da < 1; da++)
                    {
                        Point Loc = new Point(smx, smy - Main.rand.Next(125, 150));
                        if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Overworld/Overworld3"))
                            continue;

                        int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Overworld/Overworld3");
                        foreach (int chestIndex in ChestIndexs)
                        {
                            var chest = Main.chest[chestIndex];
                            // etc

                            // itemsToAdd will hold type and stack data for each item we want to add to the chest
                            var itemsToAdd = new List<(int type, int stack)>();

                            // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                            int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5)


                            // Choose no item with a high weight of 7.
                            );
                            if (specialItem != ItemID.None)
                            {
                                itemsToAdd.Add((specialItem, 1));
                            }
                            // Using a switch statement and a random choice to add sets of items.
                            switch (Main.rand.Next(11))
                            {
                                case 0:
                                    itemsToAdd.Add((ModContent.ItemType<Gutinier>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                    break;
                                case 1:
                                    itemsToAdd.Add((ItemID.WandofFrosting, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;
                                case 2:
                                    itemsToAdd.Add((ItemID.EndlessQuiver, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;
                                case 3:
                                    itemsToAdd.Add((ItemID.SlimeStaff, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));

                                    break;
                                case 4:
                                    itemsToAdd.Add((ItemID.Diamond, Main.rand.Next(1, 20)));
                                    itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;

                                case 5:
                                    itemsToAdd.Add((ModContent.ItemType<IronCrossbow>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;

                                case 6:
                                    //itemsToAdd.Add((ModContent.ItemType<EaglesGrace>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;

                                case 7:
                                    itemsToAdd.Add((ItemID.ShinyRedBalloon, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;

                                case 8:
                                    itemsToAdd.Add((ItemID.BandofRegeneration, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;

                                case 9:
                                    itemsToAdd.Add((ItemID.BandofStarpower, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;

                                case 10:
                                    itemsToAdd.Add((ItemID.PlatinumBar, Main.rand.Next(1, 20)));
                                    itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                    break;
                            }

                            // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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

                    placed = true;
                }
            }

        }

        private void WorldGenStalker(GenerationProgress progress, GameConfiguration configuration)
        {
            StructureMap structures = GenVars.structures;
            Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Ocean/SunAlter2");
            progress.Message = "Bird building alters";

            int[] tileBlend = new int[]
        {
                TileID.RubyGemspark
        };

            for (int k = 0; k < 1; k++)
            {
                bool placed = false;
                int attempts = 0;
                while (!placed && attempts++ < 1000000)
                {
                    // Select a place in the first 6th of the world, avoiding the oceans
                    int smx = WorldGen.genRand.Next(350, (Main.maxTilesX) - 350); // from 50 since there's a unaccessible area at the world's borders
                                                                                  // 50% of choosing the last 6th of the world
                                                                                  // Choose which side of the world to be on randomly
                    ///if (WorldGen.genRand.NextBool())
                    ///{
                    ///	towerX = Main.maxTilesX - towerX;
                    ///}

                    //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                    int smy = ((int)(Main.worldSurface - 200));

                    // We go down until we hit a solid tile or go under the world's surface
                    while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
                    {
                        smy++;
                    }

                    // If we went under the world's surface, try again
                    if (smy > Main.worldSurface - 5)
                    {
                        continue;
                    }
                    Tile tile = Main.tile[smx, smy];
                    // If the type of the tile we are placing the tower on doesn't match what we want, try again
                    if (!(tile.TileType == TileID.Sand
                            || tile.TileType == TileID.HardenedSand
                        || tile.TileType == TileID.Sandstone))

                    {
                        continue;
                    }

                    Point Loc = new Point(smx, smy + 10);
                    string path = "Struct/Ocean/SunAlter2";
                    if (!StructureLoader.TryPlaceAndProtectStructure(Loc, path))
                        continue;

                    for (int da = 0; da < 1; da++)
                    {
                        int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path, tileBlend);
                        foreach (int chestIndex in ChestIndexs)
                        {
                            var chest = Main.chest[chestIndex];
                            // etc

                            // itemsToAdd will hold type and stack data for each item we want to add to the chest
                            var itemsToAdd = new List<(int type, int stack)>();

                            // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                            int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                                    Tuple.Create(ModContent.ItemType<CinderBraker>(), 0.1)


                            );
                            if (specialItem != ItemID.None)
                            {
                                itemsToAdd.Add((specialItem, 1));
                            }
                            // Using a switch statement and a random choice to add sets of items.
                            switch (Main.rand.Next(1))
                            {
                                case 0:

                                    itemsToAdd.Add((ModContent.ItemType<OceanScroll>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<OceanRuneI>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                    itemsToAdd.Add((ItemID.AntlionMandible, Main.rand.Next(5, 10)));
                                    itemsToAdd.Add((ItemID.Coral, Main.rand.Next(1, 25)));
                                    itemsToAdd.Add((ItemID.SharkFin, Main.rand.Next(1, 25)));
                                    itemsToAdd.Add((ItemID.MasterBait, Main.rand.Next(1, 25)));

                                    break;

                            }

                            // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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

                    placed = true;
                }
            }

        }

        private void WorldGenGiaHouse(GenerationProgress progress, GameConfiguration configuration)
        {
            StructureMap structures = GenVars.structures;
            Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Acid/GiaHouse");
            progress.Message = "Gia living fruitfully";


            for (int k = 0; k < 1; k++)
            {
                bool placed = false;
                int attempts = 0;
                while (!placed && attempts++ < 1000000)
                {
                    // Select a place in the first 6th of the world, avoiding the oceans
                    int smx = WorldGen.genRand.Next(250, (Main.maxTilesX) - 250); // from 50 since there's a unaccessible area at the world's borders
                                                                                  // 50% of choosing the last 6th of the world
                                                                                  // Choose which side of the world to be on randomly
                    ///if (WorldGen.genRand.NextBool())
                    ///{
                    ///	towerX = Main.maxTilesX - towerX;
                    ///}

                    //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                    int smy = ((int)(Main.worldSurface - 200));

                    // We go down until we hit a solid tile or go under the world's surface
                    while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
                    {
                        smy++;
                    }

                    // If we went under the world's surface, try again
                    if (smy > Main.worldSurface - 5)
                    {
                        continue;
                    }
                    Tile tile = Main.tile[smx, smy];
                    // If the type of the tile we are placing the tower on doesn't match what we want, try again
                    if (!(tile.TileType == ModContent.TileType<AcidialDirt>()))
                    {
                        continue;
                    }



                    // place the Rogue
                    //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                    //Main.npc[num].homeTileX = -1;
                    //	Main.npc[num].homeTileY = -1;
                    //	Main.npc[num].direction = 1;
                    //	Main.npc[num].homeless = true;



                    for (int da = 0; da < 1; da++)
                    {
                        Point Loc = new Point(smx, smy + 5);
                        rectangle.Location = Loc;
                        StructureLoader.ReadStruct(Loc, "Struct/Acid/GiaHouse");
                        NPCs.Town.AlcadSpawnSystem.GiaTile = Loc;




                    }

                    placed = true;
                }
            }

        }


        private void WorldGenSeaTemple(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Definitely not making elder guardians from minecraft.";


            for (int k = 0; k < 1; k++)
            {
                bool placed = false;
                int attempts = 0;
                while (!placed && attempts++ < 1000000)
                {
                    // Select a place in the first 6th of the world, avoiding the oceans
                    int smx = WorldGen.genRand.Next((Main.maxTilesX) - 260, (Main.maxTilesX) - 120); // from 50 since there's a unaccessible area at the world's borders
                                                                                                     // 50% of choosing the last 6th of the world
                                                                                                     // Choose which side of the world to be on randomly
                    ///if (WorldGen.genRand.NextBool())
                    ///{
                    ///	towerX = Main.maxTilesX - towerX;
                    ///}

                    //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                    int smy = ((int)(Main.worldSurface - 200));

                    // We go down until we hit a solid tile or go under the world's surface
                    while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
                    {
                        smy++;
                    }

                    // If we went under the world's surface, try again
                    if (smy > Main.worldSurface - 5)
                    {
                        continue;
                    }
                    Tile tile = Main.tile[smx, smy];
                    // If the type of the tile we are placing the tower on doesn't match what we want, try again
                    if (!(tile.TileType == TileID.Sand
                            || tile.TileType == TileID.HardenedSand
                            || tile.TileType == TileID.Dirt
                        || tile.TileType == TileID.Sandstone))

                    {
                        continue;
                    }


                    // place the Rogue
                    //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                    //Main.npc[num].homeTileX = -1;
                    //	Main.npc[num].homeTileY = -1;
                    //	Main.npc[num].direction = 1;
                    //	Main.npc[num].homeless = true;



                    for (int da = 0; da < 1; da++)
                    {
                        Point Loc = new Point(smx, smy + 350);
                        int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Ocean/SeaTemple");
                        foreach (int chestIndex in ChestIndexs)
                        {
                            var chest = Main.chest[chestIndex];
                            // etc

                            // itemsToAdd will hold type and stack data for each item we want to add to the chest
                            var itemsToAdd = new List<(int type, int stack)>();

                            // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                            itemsToAdd.Add((ModContent.ItemType<OceanScroll>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<OceanRuneI>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ItemID.Coral, Main.rand.Next(1, 25)));
                            itemsToAdd.Add((ItemID.SharkFin, Main.rand.Next(1, 25)));
                            itemsToAdd.Add((ItemID.MasterBait, Main.rand.Next(1, 25)));

                            // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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

                    placed = true;
                }
            }

        }

        private void WorldGenCatacombsWater2(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Definitely not making even more trapped elder guardians";

            int yOffset = 0;
            for (int k = 0; k < 1; k++)
            {
                bool placed = false;
                int attempts = 0;

                while (!placed && attempts++ < 3000)
                {
                    // Select a place in the first 6th of the world, avoiding the oceans
                    int smx = WorldGen.genRand.Next(220, 300); // from 50 since there's a unaccessible area at the world's borders


                    // 50% of choosing the last 6th of the world
                    // Choose which side of the world to be on randomly
                    ///if (WorldGen.genRand.NextBool())
                    ///{
                    ///	towerX = Main.maxTilesX - towerX;
                    ///}

                    //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                    int smy = ((int)(Main.worldSurface - 200));

                    // We go down until we hit a solid tile or go under the world's surface
                    while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
                    {
                        smy++;
                    }

                    // If we went under the world's surface, try again
                    if (smy > Main.worldSurface - 5)
                    {
                        continue;
                    }

                    Point Loc = new Point(smx - 100, smy + 275 + yOffset);

                    if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Catacombs/CatacombsWater"))
                    {
                        yOffset++;
                        attempts++;
                        continue;
                    }

                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Catacombs/CatacombsWater");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                                Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.1)


                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(1))
                        {
                            case 0:

                                itemsToAdd.Add((ModContent.ItemType<CursedShard>(), Main.rand.Next(7, 12)));

                                break;

                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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


                    yOffset += 125;
                    placed = true;
                }
            }

        }


        private void WorldGenCatacombsWater(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Definitely not making some trapped elder guardians";

            int yOffset = 0;
            for (int k = 0; k < 1; k++)
            {
                bool placed = false;
                int attempts = 0;

                while (!placed && attempts++ < 3000)
                {
                    // Select a place in the first 6th of the world, avoiding the oceans
                    int smx = WorldGen.genRand.Next((Main.maxTilesX) - 160, (Main.maxTilesX) - 120); // from 50 since there's a unaccessible area at the world's borders
                                                                                                     // 50% of choosing the last 6th of the world
                                                                                                     // Choose which side of the world to be on randomly
                    ///if (WorldGen.genRand.NextBool())
                    ///{
                    ///	towerX = Main.maxTilesX - towerX;
                    ///}

                    //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                    int smy = ((int)(Main.worldSurface - 200));

                    // We go down until we hit a solid tile or go under the world's surface
                    while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
                    {
                        smy++;
                    }

                    // If we went under the world's surface, try again
                    if (smy > Main.worldSurface - 5)
                    {
                        continue;
                    }

                    Point Loc = new Point(smx - 100, smy + 275 + yOffset);
                    if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Catacombs/CatacombsWater"))
                    {
                        yOffset++;
                        attempts++;
                        continue;
                    }

                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Catacombs/CatacombsWater");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                                Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.1)


                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(1))
                        {
                            case 0:

                                itemsToAdd.Add((ModContent.ItemType<CursedShard>(), Main.rand.Next(7, 15)));

                                break;

                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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


                    yOffset += 125;
                    placed = true;
                }
            }

        }
        private int SmoothHeight(Point startPoint)
        {
            int range = 3;
            int left = startPoint.X - range;
            int right = startPoint.X + range;

            left = Math.Clamp(left, 0, Main.maxTilesX - 1);
            right = Math.Clamp(right, 0, Main.maxTilesX - 1);

            int sum = 0;
            int count = 0;
            for (int x = left; x <= right; x++)
            {
                if (x == startPoint.X)
                    continue;

                int heightAtPoint = FindSurfacePoint(x).Y;
                heightAtPoint = Math.Min(heightAtPoint, startPoint.Y);
                sum += heightAtPoint;
                count++;
            }

            int avg = sum / count;
            return avg;
        }

        private void SmoothenOut(int x, int y, int avgHeight)
        {
            int direction = avgHeight < y ? -1 : 1;
            Tile originalTile = Main.tile[x, y];
            while (y != avgHeight)
            {
                if (y < avgHeight)
                {
                    Tile tile = Main.tile[x, y];
                    tile.ClearEverything();
                }
                y += direction;
            }
        }
        private Point TileBounds(in int tileType, int left, int right)
        {
            Point p = new Point();
            p.X = Main.maxTilesX - 1;
            p.Y = 0;
            for (int x = left; x < right; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile && tile.TileType == tileType)
                    {
                        p.X = Math.Min(p.X, x);
                        p.Y = Math.Max(p.Y, x);
                    }
                }

            }
            return p;
        }

        private Point SnowBounds()
        {
            Point p = new Point();
            p.X = Main.maxTilesX - 1;
            p.Y = 0;
            for (int x = GenVars.snowOriginLeft - 500; x < GenVars.snowOriginRight + 500; x++)
            {
                int y = FindSurfacePoint(x).Y;
                Tile tile = Main.tile[x, y];
                if (tile.HasTile && tile.TileType == TileID.SnowBlock)
                {
                    p.X = Math.Min(p.X, x);
                    p.Y = Math.Max(p.Y, x);
                }
            }
            return p;
        }

        private void SmoothenSurface(int left, int right, int levels)
        {
            /*
            int totalHeight = 0;
            int count = right - left;
            for (int x = left; x <= right; x++)
            {
                totalHeight += FindSurfacePoint(x).Y;
            }
            totalHeight /= count;

            for(int x = left; x <= right; x++)
            {
                int y = 0;
                while(y < totalHeight)
                {
                    Tile tile = Main.tile[x, y];
                    tile.ClearEverything();
                    y++;
                }
            }
          */
            for (int i = 0; i < levels; i++)
            {
                //Calculate new heights
                int[] heights = new int[right - left];
                int[] originalY = new int[heights.Length];
                for (int x = left; x < right; x++)
                {
                    int y = FindSurfacePoint(x).Y;
                    heights[x - left] = SmoothHeight(new Point(x, y));
                    originalY[x - left] = y;
                }

                //APply
                for (int x = left; x < right; x++)
                {
                    int index = x - left;
                    SmoothenOut(x, originalY[index], heights[index]);
                }

            }


        }
        private void WorldGenBridget(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "The Almighty weapon being burried";


            StructureMap structures = GenVars.structures;
            Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Veriplant/BigVeriplant1");




            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 1000000)
            {


                int abysmx = WorldGen.genRand.Next(300, Main.maxTilesX - 300); // from 50 since there's a unaccessible area at the world's borders

                // Select a place in the first 6th of the world, avoiding the oceans
                int abysmy = ((Main.maxTilesY / 2));

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(abysmx, abysmy) && abysmy <= Main.UnderworldLayer)
                {
                    abysmy++;
                }

                // If we went under the world's surface, try again
                if (abysmy > Main.UnderworldLayer - 50)
                {
                    continue;
                }




                Tile tile = Main.tile[abysmx, abysmy];



                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == ModContent.TileType<AcidialDirt>() || tile.TileType == TileID.Sand))
                {
                    continue;
                }



                // place the Rogue
                //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                //Main.npc[num].homeTileX = -1;
                //	Main.npc[num].homeTileY = -1;
                //	Main.npc[num].direction = 1;
                //	Main.npc[num].homeless = true;



                for (int da = 0; da < 1; da++)
                {

                    rectangle.Location = pointVeri;
                    int[] ChestIndexs = StructureLoader.ReadStruct(pointVeri, "Struct/Veriplant/BigVeriplant1");
                    StructureLoader.ProtectStructure(pointVeri, "Struct/Veriplant/BigVeriplant1");

                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                            Tuple.Create((int)ItemID.Acorn, 0.1),
                            Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.1),
                            Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
                                Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8)// Choose no item with a high weight of 7.
                        );

                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(4))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ModContent.ItemType<CocoSpark>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                break;
                            case 2:
                                itemsToAdd.Add((ModContent.ItemType<MorrowRapier>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ModContent.ItemType<MorrowSword>(), Main.rand.Next(1, 1)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Bongos>(), Main.rand.Next(1, 1)));

                                break;
                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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

                placed = true;
            }



        }

        private void WorldGenCatacombsFlames(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Burning the world with catacombs";

            for (int k = 0; k < 1; k++)
            {
                bool placed = false;
                int attempts = 0;
                while (!placed && attempts++ < 1000000)
                {
                    int abysmx = WorldGen.genRand.Next(500, Main.maxTilesX - 500); // from 50 since there's a unaccessible area at the world's borders

                    // Select a place in the first 6th of the world, avoiding the oceans
                    int abysmyy = Main.maxTilesY - (Main.maxTilesY / 3) + WorldGen.genRand.Next(0, 200) - 100;

                    // We go down until we hit a solid tile or go under the world's surface
                    while (!WorldGen.SolidTile(abysmx, abysmyy) && abysmyy <= Main.UnderworldLayer)
                    {
                        abysmyy++;
                    }

                    // If we went under the world's surface, try again
                    if (abysmyy > Main.UnderworldLayer - 50)
                    {
                        continue;
                    }

                    int abysmy = abysmyy;
                    Tile tile = Main.tile[abysmx, abysmy];
                    // If the type of the tile we are placing the tower on doesn't match what we want, try again
                    if (!(tile.TileType == TileID.Sandstone))
                    {
                        continue;
                    }

                    Point Loc = new Point(abysmx, abysmy);
                    if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Catacombs/CatacombsFire"))
                        continue;

                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Catacombs/CatacombsFire");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                            Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

                        // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(1))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<CursedShard>(), Main.rand.Next(7, 12)));
                                break;

                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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

                    placed = true;
                }
            }
        }


        private void WorldGenCatacombsTrap(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Trapping the world with catacombs";

            int leftmostTileX = int.MaxValue;
            int rightmostTileX = int.MinValue;
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                int y = (int)(Main.worldSurface - 50);
                while (!WorldGen.SolidTile(x, y) && y <= Main.worldSurface)
                {
                    y++;
                }

                Tile tile = Main.tile[x, y];
                if (tile.TileType == TileID.SnowBlock)
                {
                    if (leftmostTileX > x)
                        leftmostTileX = x;
                    if (rightmostTileX < x)
                        rightmostTileX = x;
                }
            }

            int xOffset = -400;
            for (int k = 0; k < 1; k++)
            {
                bool placed = false;
                int attempts = 0;
                while (!placed && attempts++ < 1000000)
                {
                    int x = leftmostTileX + xOffset; // from 50 since there's a unaccessible area at the world's borders

                    // Select a place in the first 6th of the world, avoiding the oceans
                    int y = Main.UnderworldLayer - 300;


                    // We go down until we hit a solid tile or go under the world's surface
                    while (!WorldGen.SolidTile(x, y))
                    {
                        x++;
                    }

                    Point Loc = new Point(x, y);
                    if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Catacombs/CatacombsTrap"))
                    {
                        xOffset++;
                        continue;
                    }

                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Catacombs/CatacombsTrap");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                            Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

                        // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }

                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(1))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<CursedShard>(), Main.rand.Next(7, 12)));
                                break;

                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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

                    xOffset += 90;
                    placed = true;
                }
            }
        }





        private void WorldGenRallad(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Rallad killing people";



            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 1000000)
            {


                int abysmx = WorldGen.genRand.Next(500, Main.maxTilesX - 500); // from 50 since there's a unaccessible area at the world's borders

                // Select a place in the first 6th of the world, avoiding the oceans
                int abysmy = ((Main.maxTilesY / 2));

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(abysmx, abysmy) && abysmy <= Main.UnderworldLayer)
                {
                    abysmy++;
                }

                // If we went under the world's surface, try again
                if (abysmy > Main.UnderworldLayer - 50)
                {
                    continue;
                }
                Tile tile = Main.tile[abysmx, abysmy];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == ModContent.TileType<AbyssalDirt>()))
                {
                    continue;
                }


                // place the Rogue
                //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                //Main.npc[num].homeTileX = -1;
                //	Main.npc[num].homeTileY = -1;
                //	Main.npc[num].direction = 1;
                //	Main.npc[num].homeless = true;



                for (int da = 0; da < 1; da++)
                {
                    Point Loc = new Point(abysmx - 150, abysmy + 200);

                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Aurelus/Rallad");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                            Tuple.Create(ModContent.ItemType<OldCarianTome>(), 0.5)


                        // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(7))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<MagnusMagnum>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ModContent.ItemType<Venatici>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(100, 1500)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                break;
                            case 2:
                                itemsToAdd.Add((ModContent.ItemType<Neptune8Card>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(100, 1500)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<TON618Crossbow>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(100, 1500)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                            case 4:
                                itemsToAdd.Add((ModContent.ItemType<HolmbergScythe>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(100, 1500)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                break;

                            case 5:
                                itemsToAdd.Add((ModContent.ItemType<AurelusBlightBroochA>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(100, 1500)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 10)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;

                            case 6:
                                itemsToAdd.Add((ModContent.ItemType<AbyssalPowder>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Shiverthorn, Main.rand.Next(2, 15)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(100, 1500)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 10)));
                                itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
                                break;
                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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

                placed = true;
            }


        }










        private void WorldGenMechShop(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Finding a place for the shop";

            bool placed = false;
            int attempts = 0;
            while (!placed && attempts < 10000)
            {
                attempts++;
                int abysmx = WorldGen.genRand.Next(700, Main.maxTilesX - 700); // from 50 since there's a unaccessible area at the world's borders

                //This code makes it avoid teh center
                int distanceBetween = Math.Abs(Main.spawnTileX - abysmx);
                for (int i = 0; i < 1000; i++)
                {
                    abysmx = WorldGen.genRand.Next(700, Main.maxTilesX - 700);
                    distanceBetween = Math.Abs(Main.spawnTileX - abysmx);
                    if (distanceBetween < 200)
                        break;
                }

                // Select a place in the first 6th of the world, avoiding the oceans
                int abysmy = FindSurfacePoint(abysmx).Y;
                abysmy += 400;
                Tile tile = Main.tile[abysmx, abysmy];
                while (tile.HasTile)
                {
                    abysmy++;
                    if (abysmy >= Main.maxTilesY)
                        break;
                    tile = Main.tile[abysmx, abysmy];
                }

                Point Loc = new Point(abysmx, abysmy);
                string path = "Struct/Underground/MechanicShop";
                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, path))
                    continue;

                int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path);
                NPCs.Town.AlcadSpawnSystem.MechanicsTownTile = Loc;
                placed = true;
            }
        }







        private void WorldGenAurelusTemple(GenerationProgress progress, GameConfiguration configuration)
        {
            StructureMap structures = GenVars.structures;
            Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Aurelus/AurelusTemple2");
            progress.Message = "Singularities singing!";

            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 1000000)
            {
                //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                //Main.npc[num].homeTileX = -1;
                //	Main.npc[num].homeTileY = -1;
                //	Main.npc[num].direction = 1;
                //	Main.npc[num].homeless = true;

                Point abyssBounds = AbyssBounds();
                Point spawnPoint = new Point();
                spawnPoint.X = abyssBounds.X + abyssBounds.Y;
                spawnPoint.X /= 2;
                spawnPoint.Y = Main.maxTilesY / 2;


                for (int da = 0; da < 1; da++)
                {
                    Point Loc = new Point(spawnPoint.X - 150, spawnPoint.Y + 100);
                    rectangle.Location = Loc;
                    StructureLoader.ProtectStructure(Loc, "Struct/Aurelus/AurelusTemple2");

                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Aurelus/AurelusTemple2");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                            Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
                            Tuple.Create(ModContent.ItemType<ConvulgingMater>(), 0.1),
                            Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

                        // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(7))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<MagnusMagnum>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ModContent.ItemType<Venatici>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                break;
                            case 2:
                                itemsToAdd.Add((ModContent.ItemType<Neptune8Card>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner8>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<TON618Crossbow>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                            case 4:
                                itemsToAdd.Add((ModContent.ItemType<HolmbergScythe>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                break;

                            case 5:
                                itemsToAdd.Add((ModContent.ItemType<AurelusBlightBroochA>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner8>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 10)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;

                            case 6:
                                itemsToAdd.Add((ModContent.ItemType<AbyssalPowder>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner8>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Shiverthorn, Main.rand.Next(2, 15)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 10)));
                                itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
                                break;
                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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

                placed = true;
            }


        }





        private void WorldGenGovheilCastle(GenerationProgress progress, GameConfiguration configuration)
        {
            StructureMap structures = GenVars.structures;
            Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Huntria/Govheil2");
            progress.Message = "Irradia marrying Paraffin instead of Delgrim";

            int[] tileBlend = new int[]
            {
                TileID.RubyGemspark
            };


            bool placed = false;
            int attempts = 0;
            Point acidBounds = TileBounds(ModContent.TileType<AcidialDirt>(), 0, Main.maxTilesX);
            Point placePoint = pointL;
            while (!placed && attempts < 1000)
            {
                attempts++;
                int abysmx = acidBounds.X + acidBounds.Y;
                abysmx /= 2; // from 50 since there's a unaccessible area at the world's borders

                // Select a place in the first 6th of the world, avoiding the oceans
                int abysmy = ((Main.maxTilesY / 2));
                string path = "Struct/Huntria/Govheil2";
                placePoint.Y += attempts;

                int[] ChestIndexs = StructureLoader.ReadStruct(placePoint, path, tileBlend);
                StructureLoader.ProtectStructure(placePoint, path);
                NPCs.Town.AlcadSpawnSystem.IrrTile = placePoint;
                NPCs.Town.AlcadSpawnSystem.GothTile = placePoint;

                foreach (int chestIndex in ChestIndexs)
                {
                    var chest = Main.chest[chestIndex];
                    // etc

                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                    var itemsToAdd = new List<(int type, int stack)>();

                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                        Tuple.Create(ModContent.ItemType<AlcadizScrap>(), 0.5),
                        Tuple.Create(ModContent.ItemType<LostScrap>(), 0.4),
                        Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.1)

                    // Choose no item with a high weight of 7.
                    );
                    if (specialItem != ItemID.None)
                    {
                        itemsToAdd.Add((specialItem, 1));
                    }
                    // Using a switch statement and a random choice to add sets of items.
                    switch (Main.rand.Next(10))
                    {
                        case 0:
                            itemsToAdd.Add((ModContent.ItemType<GovheilPowder>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                            break;
                        case 1:
                            itemsToAdd.Add((ModContent.ItemType<GreekLantern>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 30)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
                            break;
                        case 2:
                            itemsToAdd.Add((ModContent.ItemType<Kilvier>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                            itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                            break;
                        case 3:
                            itemsToAdd.Add((ModContent.ItemType<Galvinie>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<AlcadizScrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 30)));
                            itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                            break;
                        case 4:
                            itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.JungleSpores, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            break;

                        case 5:
                            itemsToAdd.Add((ModContent.ItemType<GovhenShield>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                            break;

                        case 6:
                            itemsToAdd.Add((ModContent.ItemType<TheBurningRod>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 15)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
                            itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
                            break;


                        case 7:
                            itemsToAdd.Add((ModContent.ItemType<GovheilHolsterBroochA>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 15)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
                            itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
                            break;

                        case 8:
                            itemsToAdd.Add((ModContent.ItemType<Blackdot>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 30)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
                            break;

                        case 9:
                            itemsToAdd.Add((ModContent.ItemType<SrTetanus>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 15)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
                            itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
                            break;
                    }

                    // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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
                placed = true;
            }

            placed = false;
            attempts = 0;
            string lab = "Struct/Acid/Lab";

            while (!placed && attempts < 1000)
            {
                attempts++;
                rectangle.Location = placePoint;
                Point ponta = new Point(placePoint.X + 150, placePoint.Y + 300);
                ponta.Y += attempts;

                int[] ChestIndexs = StructureLoader.ReadStruct(ponta, lab);
                StructureLoader.ProtectStructure(ponta, lab);
                NPCs.Town.AlcadSpawnSystem.LabTile = ponta;

                foreach (int chestIndex in ChestIndexs)
                {
                    var chest = Main.chest[chestIndex];
                    // etc

                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                    var itemsToAdd = new List<(int type, int stack)>();

                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                        Tuple.Create(ModContent.ItemType<AlcadizScrap>(), 0.5),
                        Tuple.Create(ModContent.ItemType<LostScrap>(), 0.1),
                        Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

                    // Choose no item with a high weight of 7.
                    );
                    if (specialItem != ItemID.None)
                    {
                        itemsToAdd.Add((specialItem, 1));
                    }
                    // Using a switch statement and a random choice to add sets of items.
                    switch (Main.rand.Next(9))
                    {
                        case 0:
                            itemsToAdd.Add((ModContent.ItemType<GovheilPowder>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(5, 20)));
                            ;
                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                            break;
                        case 1:
                            itemsToAdd.Add((ModContent.ItemType<GreekLantern>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 30)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
                            break;
                        case 2:
                            itemsToAdd.Add((ModContent.ItemType<Kilvier>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                            itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                            break;
                        case 3:
                            itemsToAdd.Add((ModContent.ItemType<Galvinie>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<AlcadizScrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 30)));
                            itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                            break;
                        case 4:
                            itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.JungleSpores, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            break;

                        case 5:
                            itemsToAdd.Add((ModContent.ItemType<GovhenShield>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                            break;

                        case 6:
                            itemsToAdd.Add((ModContent.ItemType<TheBurningRod>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 15)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
                            itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
                            break;


                        case 7:
                            itemsToAdd.Add((ModContent.ItemType<GovheilHolsterBroochA>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 15)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
                            itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
                            break;

                        case 9:
                            itemsToAdd.Add((ModContent.ItemType<Blackdot>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 30)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
                            break;
                    }

                    // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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
                placed = true;
            }
        }

        private void WorldGenAbysm(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Shifting Shadows deep in the Ice";
            Point snowBounds = SnowBounds();
            snowBounds.X -= 500;
            snowBounds.Y += 500;

            //Check around ice biome and the center height of the world for frozen tiles to turn into abyss.
            snowBounds.X = Math.Clamp(snowBounds.X, 0, Main.maxTilesX - 1);
            snowBounds.Y = Math.Clamp(snowBounds.Y, 0, Main.maxTilesX - 1);
            for (int x = 0; x < 10000; x++)
            {
                int randX = WorldGen.genRand.Next(snowBounds.X, snowBounds.Y);
                int randY = ((Main.maxTilesY / 2));

                Tile tile = Main.tile[randX, randY];
                if (!tile.HasTile)
                    continue;
                if (!MatchesRequiredTileTypes(tile, TileID.CorruptIce, TileID.SnowBlock, TileID.FleshIce, TileID.IceBlock, TileID.Slush))
                    continue;

                WorldGen.TileRunner(randX, randY, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(150, 150), ModContent.TileType<AbyssalDirt>());
            }
        }




        private void WorldGenCinderspark(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Cindersparking SO HARD right now";

            for (int x = 0; x < 20000; x++)
            {
                int randX = WorldGen.genRand.Next(250, Main.maxTilesX - 250);
                int randY = (Main.UnderworldLayer - (Main.maxTilesY / 20));
                randY += WorldGen.genRand.Next(-100, 200);
                if (randY >= Main.maxTilesY)
                    continue;

                Tile tile = Main.tile[randX, randY];
                if (!tile.HasTile)
                    continue;

                if (!MatchesRequiredTileTypes(tile, TileID.Stone, TileID.Ash, TileID.Dirt, TileID.Mud, TileID.IceBlock))
                    continue;

                WorldGen.TileRunner(randX, randY, 32, 100, ModContent.TileType<CindersparkDirt>());
            }
        }





        public void WorldGenVirulent(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Virulifying the Morrow";



            bool placed = false;
            int attempts = 0;
            int leftmostJungleTileX = int.MaxValue;
            int rightmostJungleTileX = int.MinValue;
            for (int x = 500; x < Main.maxTilesX - 500; x++)
            {
                int jungleY = (int)(Main.worldSurface - 50);
                while (!WorldGen.SolidTile(x, jungleY) && jungleY <= Main.worldSurface)
                {
                    jungleY++;
                }

                Tile tile = Main.tile[x, jungleY];
                if (tile.TileType == TileID.Mud)
                {
                    if (leftmostJungleTileX > x)
                        leftmostJungleTileX = x;
                    if (rightmostJungleTileX < x)
                        rightmostJungleTileX = x;
                }
            }



            while (!placed && attempts < 10000)
            {
                attempts++;
                int minX = leftmostJungleTileX + 200;
                int maxX = rightmostJungleTileX - 200;
                if (maxX < minX)
                    maxX = minX + 1;
                int abysmx = WorldGen.genRand.Next(minX, maxX);
                int abysmy = FindSurfacePoint(abysmx).Y;

                Point Loc7 = new Point(abysmx, abysmy);
                WorldGen.TileRunner(Loc7.X + 200, Loc7.Y, 500, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>());
                WorldGen.TileRunner(Loc7.X + 200, Loc7.Y + 300, 400, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>());
                WorldGen.TileRunner(Loc7.X + 200, Loc7.Y + 600, 300, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>());

                Point Loc = new Point(abysmx + 50, abysmy + 255);
                pointL = new Point(abysmx + 50, abysmy + 255);//


                placed = true;
            }

            for (int fa = 0; fa < 20; fa++)
            {
                int abysmxd = WorldGen.genRand.Next(500, Main.maxTilesX - 500);
                int abysmyd = (int)(Main.worldSurface - 50);

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(abysmxd, abysmyd) && abysmyd <= Main.worldSurface)
                {
                    abysmyd++;
                }

                // If we went under the world's surface, try again
                if (abysmyd > Main.worldSurface)
                {
                    continue;
                }
                Tile tile = Main.tile[abysmxd, abysmyd];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == ModContent.TileType<Tiles.Acid.AcidialDirt>()))
                {
                    continue;
                }
                for (int da = 0; da < 1; da++)
                {
                    Point Loc = new Point(abysmxd, abysmyd);


                    WorldGen.digTunnel(Loc.X, Loc.Y, 0, 1, 130, 3, false);
                }




            }


        }

        Point pointL;

        Point pointLil;

        private Point FindSurfacePoint(int x)
        {
            int y = (int)GenVars.worldSurfaceLow - 35;
            while (y < Main.worldSurface)
            {
                if (WorldGen.SolidTile(x, y))
                {
                    break;
                }
                else
                {
                    y++;
                }
            }
            return new Point(x, y);
        }

        #region Royal Capital


        private void WorldGenVU(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Residents of the veil crafting chasms";


            //Place Veil Spot
            Point snowbounds = SnowBounds();
            int iceBiomeCenter = snowbounds.X + snowbounds.Y;
            iceBiomeCenter /= 2;
            Point veilSpot = FindSurfacePoint(iceBiomeCenter);
            NPCs.Town.AlcadSpawnSystem.LiberatTile = veilSpot;
            pointLil = veilSpot;



            int centerBasedWidth = 200;
            int centerBasedHeight = 50;
            Rectangle fillRect = new Rectangle(pointLil.X - centerBasedWidth / 2, pointLil.Y - centerBasedHeight, centerBasedWidth, centerBasedHeight);
            float fillPercent = FillPercent(fillRect);
            int failsafe = 0;
            while (fillPercent < 0.62f)
            {
                pointLil.Y += 2;
                fillRect.Location += new Point(0, 2);
                fillPercent = FillPercent(fillRect);
                failsafe++;
                if (failsafe >= 1000)
                    break;
            }


            int[] tileBlend = new int[]
            {
                TileID.RubyGemspark
            };

            bool placed = false;
            int attempts = 0;
            Rectangle rect = StructureLoader.ReadRectangle("Struct/Underground/Catacombz");
            int offset = 292;

            Rectangle placementRect = rect;
            placementRect.Location = pointLil;
            placementRect.Location += new Point(-rect.Width / 2, offset);


            Point point = pointLil;
            point.Y += offset;
            point.X -= rect.Width / 2;
            point.X -= 40;
            /*
            for (int x = rect.Left; x < rect.Right; x++)
            {
                for (int y = -200; y < 0; y++)
                {
                    Tile tile = Main.tile[point.X + x, point.Y + y - offset];
                    tile.ClearEverything();
                }
            }

            for (int x = rect.Left; x < rect.Right; x++)
            {
                for (int y = rect.Top; y < rect.Top + 25; y++)
                {
                    Tile tile = Main.tile[point.X + x, point.Y + y - offset];
                    tile.ClearTile();
                    tile.HasTile = true;
                    tile.TileType = TileID.SnowBlock;
                }
            }

                         point.Y = pointLil.Y;
            point.Y += offset + 25;
             */

            /*
            for (int x = fillRect.Left; x < fillRect.Right; x++)
            {
                for (int y = -50; y < 0; y++)
                {
                    Tile tile = Main.tile[x, fillRect.Top + y];
                    tile.ClearEverything();
                }
            }*/


            while (!placed && attempts++ < 10000000)
            {


                NPCs.Town.AlcadSpawnSystem.LiberatTile = point;
                NPCs.Town.AlcadSpawnSystem.JhoviaTile = point;
                StructureLoader.ReadStruct(point, "Struct/Underground/Catacombz", tileBlend);
                StructureLoader.ProtectStructure(point, "Struct/Underground/Catacombz");
                placed = true;

            }

        }









        public void WorldGenAlcadSpot(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Fighting the Virulent with magic";



            var rand = WorldGen.genRand;
            int left = pointAlcadthingy.X;
            int right = pointAlcadthingy.X + 400;
            int top = pointAlcadthingy.Y;
            int bottom = top + 1000;

            left = Math.Clamp(left, 0, Main.maxTilesX - 1);
            right = Math.Clamp(right, 0, Main.maxTilesX - 1);
            top = Math.Clamp(top, 0, Main.maxTilesY - 1);
            bottom = Math.Clamp(bottom, 0, Main.maxTilesY - 1);
            for (int i = 0; i < 2000; i++)
            {
                int x = rand.Next(left, right);
                int y = rand.Next(top, bottom);
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;
                if (WorldGen.SolidTile(x, y))
                {
                    WorldGen.TileRunner(x, y, 32, 32, (ushort)ModContent.TileType<StarbloomDirt>());
                }
            }
        }
        public void WorldGenRoyalCapitalSpot(GenerationProgress progress, GameConfiguration configuration)
        {
            StructureMap structures = GenVars.structures;
            Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Alcad/RoyalCapital2");
            progress.Message = "Fighting the Virulent with magic";

            bool placed = false;
            int attempts = 0;
            while (!placed && attempts < 10000)
            {
                attempts++;
                int smx = WorldGen.genRand.Next(260, (Main.maxTilesX) / 15);
                int smy = FindSurfacePoint(smx).Y;
                int smxx = smx;
                int smyy = smy - 20;
                Point Loc = new Point(smx + 20, smyy - 60);
                rectangle.Location = Loc;
                pointAlcadthingy = new Point(smx - 10, smyy - 60);

                //Small world sqush in
                while (rectangle.Location.Y - rectangle.Height <= 100)
                {
                    Loc.Y++;
                    pointAlcadthingy.Y++;
                    rectangle.Location = Loc;
                }

                NPCs.Town.AlcadSpawnSystem.AlcadTile = Loc;
                StructureLoader.ProtectStructure(Loc, "Struct/Alcad/RoyalCapital2");
                placed = true;
            }
        }



        public void WorldGenRoyalCapital(GenerationProgress progress, GameConfiguration configuration)
        {
            StructureMap structures = GenVars.structures;
            Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Alcad/RoyalCapital2");
            progress.Message = "Fighting the Virulent with magic";





            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 10000000)
            {
                for (int da = 0; da < 1; da++)
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(NPCs.Town.AlcadSpawnSystem.AlcadTile, "Struct/Alcad/RoyalCapital2");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                            Tuple.Create(ModContent.ItemType<AlcadizScrap>(), 0.5),
                            Tuple.Create(ModContent.ItemType<LostScrap>(), 0.1),
                            Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

                        // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(6))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<LittleWand>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(5, 20)));
                                ;
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ModContent.ItemType<AlcaricQuiver>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
                                break;
                            case 2:
                                itemsToAdd.Add((ModContent.ItemType<BlackRose>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<FloweredInsource>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<AlcadizScrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                            case 4:
                                itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                break;

                            case 5:
                                itemsToAdd.Add((ItemID.FuneralHat, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;


                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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

                placed = true;




            }
        }


        public void WorldGenIlluria(GenerationProgress progress, GameConfiguration configuration)
        {
            StructureMap structures = GenVars.structures;
            progress.Message = "Niivi protecting the cities above.";


            string structure = "Struct/Overworld/Illuria";
            Rectangle rect = StructureLoader.ReadRectangle(structure);
            int attempts = 0;
            int extraX = 0;
            while (attempts < 100000)
            {
                attempts++;
                int smx = WorldGen.genRand.Next(Main.maxTilesX - 600, (Main.maxTilesX - 250));
                smx += extraX;

                int smy = FindSurfacePoint(smx).Y;
                int smxx = smx;
                int smyy = smy - 20;

                //Small WOrld squish in support
                Point Loc = new Point(smx - 270, smyy - 20);
                rect.Location = Loc;
                while (rect.Location.Y - rect.Height <= 100)
                {
                    Loc.Y++;
                    rect.Location = Loc;
                }

                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, structure, ignoreStructures: true))
                {

                    continue;
                }

                StructureLoader.ReadStruct(Loc, structure, new int[] { TileID.RubyGemspark });
                NPCs.Town.AlcadSpawnSystem.IlluriaTile = Loc;
                break;
            }
        }


        public void WorldGenSylia(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Leaving the Royal Capital";

            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 10000000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int smx = WorldGen.genRand.Next(220, (Main.maxTilesX) / 15); // from 50 since there's a unaccessible area at the world's borders
                                                                             // 50% of choosing the last 6th of the world																	 // Choose which side of the world to be on randomly
                                                                             //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                int smy = Main.UnderworldLayer - 200;
                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(smx, smy) && smy <= Main.maxTilesY)
                {
                    smy++;
                }

                // If we went under the world's surface, try again
                if (smy > Main.maxTilesY - 30)
                {
                    continue;
                }

                Tile tile = Main.tile[smx, smy];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if ((tile.TileType == TileID.Ash))
                {
                    continue;
                }

                int smyy = smy + Main.maxTilesY / 18;
                Point Loc = new Point(smx - 10, smyy + 170);
                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Underworld/UnderworldRuins"))
                    continue;

                StructureLoader.ReadStruct(Loc, "Struct/Underworld/UnderworldRuins");
                NPCs.Town.AlcadSpawnSystem.UnderworldRuinsTile = Loc;
                placed = true;
            }
        }

        #endregion
        // 6. This is the actual world generation code.
        private void WorldGenFlameOre(GenerationProgress progress, GameConfiguration configuration)
        {
            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Scorching Gild and Arnchar burning into the world";


            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
            {
                // 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
                int x = WorldGen.genRand.Next(0, Main.maxTilesX / 2);
                int y = WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY);

                // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 14), WorldGen.genRand.Next(2, 9), ModContent.TileType<VerianoreTile>());


            }


            // 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.


        }
        private void WorldGenArncharOre(GenerationProgress progress, GameConfiguration configuration)
        {
            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Scorching Arnchar into the world";


            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
            {


                int xz = WorldGen.genRand.Next(0, Main.maxTilesX);
                int yz = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY - 200);

                // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                WorldGen.TileRunner(xz, yz, WorldGen.genRand.Next(4, 13), WorldGen.genRand.Next(7, 13), ModContent.TileType<Arnchar>());
            }




            // 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.


        }

        private void WorldGenArncharOre2(GenerationProgress progress, GameConfiguration configuration)
        {
            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Scorching more Arnchar into the world";


            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
            {


                int xz = WorldGen.genRand.Next(0, Main.maxTilesX);
                int yz = WorldGen.genRand.Next(Main.UnderworldLayer - (Main.maxTilesY / 20), Main.UnderworldLayer);

                // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                WorldGen.TileRunner(xz, yz, WorldGen.genRand.Next(4, 20), WorldGen.genRand.Next(5, 15), ModContent.TileType<Arnchar>(), false, 0, 0, true, true, -1);
            }




            // 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.


        }
        private void WorldGenFrileOre(GenerationProgress progress, GameConfiguration configuration)
        {
            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Freezing the world with Frile";


            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
            {
                // 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY);
                Tile tile = Main.tile[x, y];

                // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 10), WorldGen.genRand.Next(2, 10), ModContent.TileType<FrileOreTile>());
            }
        }




        private void WorldGenDarkstone(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Blackening Stones for racist effect";
            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY);
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;

                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 50), WorldGen.genRand.Next(2, 150), ModContent.TileType<DiminishedStone>());
            }

            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
            {
                int xz = WorldGen.genRand.Next(0, Main.maxTilesX);
                int yz = WorldGen.genRand.Next((int)GenVars.worldSurface, Main.maxTilesY - 300);
                Tile tile = Main.tile[xz, yz];
                if (!tile.HasTile)
                    continue;
                if (!MatchesRequiredTileTypes(tile, TileID.Stone, TileID.Grass))
                    continue;

                WorldGen.TileRunner(xz, yz, WorldGen.genRand.Next(3, 50), WorldGen.genRand.Next(2, 100), ModContent.TileType<VeriplantGrass>());
            }

        }

        private bool MatchesRequiredTileTypes(in Tile tile, params ushort[] tileTypes)
        {
            for (int i = 0; i < tileTypes.Length; i++)
            {
                ref ushort t = ref tileTypes[i];
                if (t == tile.TileType)
                    return true;
            }
            return false;
        }



        private void WorldGenVeriplantBlobs(GenerationProgress progress, GameConfiguration configuration)
        {
            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Veribloom forgetting their memories";


            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07 + 3); k++)
            {
                // 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY - 300);

                // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(50, 100), WorldGen.genRand.Next(100, 200), ModContent.TileType<VeriplantDirt>());
            }


            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07); k++)
            {

                int xa = WorldGen.genRand.Next(0, Main.maxTilesX);
                int ya = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY - 300);
                Point Loc = new Point(xa, ya);

                // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                Tile tile = Main.tile[Loc.X, Loc.Y];

                if (!(tile.TileType == TileID.Stone))
                {
                    continue;
                }

                if (tile.HasTile)
                {
                    switch (Main.rand.Next(5))
                    {
                        case 0:
                            StructureLoader.ReadStruct(Loc, "Struct/Veriplant/Veriplant1");
                            break;
                        case 1:
                            StructureLoader.ReadStruct(Loc, "Struct/Veriplant/Veriplant2");
                            break;
                        case 2:
                            StructureLoader.ReadStruct(Loc, "Struct/Veriplant/Veriplant3");
                            break;
                        case 3:

                            StructureLoader.ReadStruct(Loc, "Struct/Veriplant/Veriplant4");
                            break;

                        case 4:
                            WorldGen.digTunnel(Loc.X, Loc.Y, 0, 1, 30, 3, false);

                            break;

                    }



                }

            }
        }




        private void WorldGenAbandonedMineshafts(GenerationProgress progress, GameConfiguration configuration)
        {
            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Getting shafted";
            string[] pool = new string[]
            {
                "Struct/Underground/AbandonedMineshaft1",
                "Struct/Underground/AbandonedMineshaft2",
                "Struct/Underground/AbandonedMineshaft3",
                "Struct/Underground/AbandonedMineshaft4"
            };
            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05 - 3); k++)
            {
                int xa = WorldGen.genRand.Next(500, Main.maxTilesX - 500);
                int ya = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY);
                Point Loc = new Point(xa, ya);
                Tile tile = Main.tile[Loc.X, Loc.Y];
                if (!tile.HasTile)
                    continue;
                if (tile.TileType != TileID.Stone)
                    continue;

                string structure = pool[WorldGen.genRand.Next(0, pool.Length)];
                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, structure))
                    continue;

                int[] ChestIndexs = StructureLoader.ReadStruct(Loc, structure);
                foreach (int chestIndex in ChestIndexs)
                {
                    var chest = Main.chest[chestIndex];
                    // etc

                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                    var itemsToAdd = new List<(int type, int stack)>();

                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                        Tuple.Create(ModContent.ItemType<AlcadizMetal>(), 0.5),
                        Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

                    // Choose no item with a high weight of 7.
                    );
                    if (specialItem != ItemID.None)
                    {
                        itemsToAdd.Add((specialItem, 1));
                    }
                    // Using a switch statement and a random choice to add sets of items.
                    switch (Main.rand.Next(9))
                    {
                        case 0:
                            itemsToAdd.Add((ModContent.ItemType<LifeSeekingVial>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));

                            itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 3)));
                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.TinOre, Main.rand.Next(1, 100)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                            break;
                        case 1:
                            itemsToAdd.Add((ModContent.ItemType<KnivedQuiver>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 3)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.CopperOre, Main.rand.Next(1, 100)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                            break;
                        case 2:
                            itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 3)));
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.CopperOre, Main.rand.Next(1, 100)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            break;
                        case 3:
                            itemsToAdd.Add((ItemID.MiningHelmet, Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 4)));
                            itemsToAdd.Add((ItemID.MiningPants, Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 100)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                            break;
                        case 4:
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 10)));
                            itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 10)));
                            itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.LeadOre, Main.rand.Next(1, 100)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            break;

                        case 5:
                            itemsToAdd.Add((ItemID.IronBar, Main.rand.Next(1, 40)));
                            itemsToAdd.Add((ItemID.Deathweed, Main.rand.Next(2, 25)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                            break;

                        case 6:
                            itemsToAdd.Add((ModContent.ItemType<AlcadizDagger>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.Waterleaf, Main.rand.Next(2, 25)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(2, 10)));
                            itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
                            break;


                        case 7:
                            itemsToAdd.Add((ItemID.WaterWalkingBoots, Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.IronOre, Main.rand.Next(1, 100)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
                            break;

                        case 8:
                            itemsToAdd.Add((ItemID.MiningShirt, Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<StumpBuster>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 6)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(1, 7)));
                            break;

                        case 9:
                            itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner1>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 6)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(1, 7)));
                            break;
                    }

                    // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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
        }

        private void WorldGenUnderworldSpice(GenerationProgress progress, GameConfiguration configuration)
        {
            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Sylia using magic in the Underworld";
            string[] pool = new string[]
            {
                "Struct/Underworld/Underworld1",
                "Struct/Underworld/Underworld2",
                "Struct/Underworld/Underworld3",
                "Struct/Underworld/Underworld4"
            };

            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-06 + 10); k++)
            {

                int xa = WorldGen.genRand.Next(0, Main.maxTilesX);
                int ya = WorldGen.genRand.Next(Main.maxTilesY - 400, Main.maxTilesY - 50);
                Point Loc = new Point(xa, ya);
                Tile tile = Main.tile[Loc.X, Loc.Y];
                if (!MatchesRequiredTileTypes(tile, TileID.Ash, TileID.Stone, (ushort)ModContent.TileType<CindersparkDirt>()))
                    continue;

                if (!tile.HasTile)
                    continue;
                string structure = pool[WorldGen.genRand.Next(0, pool.Length)];
                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, structure))
                    continue;

                int[] ChestIndexs = StructureLoader.ReadStruct(Loc, structure);
                foreach (int chestIndex in ChestIndexs)
                {
                    var chest = Main.chest[chestIndex];
                    var itemsToAdd = new List<(int type, int stack)>();
                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                        Tuple.Create(ModContent.ItemType<AlcaricMush>(), 0.5),
                        Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)
                    );
                    if (specialItem != ItemID.None)
                    {
                        itemsToAdd.Add((specialItem, 1));
                    }

                    switch (Main.rand.Next(9))
                    {
                        case 0:
                            itemsToAdd.Add((ModContent.ItemType<Infernis>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));

                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.FlameDye, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
                            break;
                        case 2:
                            itemsToAdd.Add((ItemID.LavaproofTackleBag, Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                            itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                            break;
                        case 3:
                            itemsToAdd.Add((ItemID.ObsidianRose, Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                            break;
                        case 4:
                            itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            break;

                        case 5:
                            itemsToAdd.Add((ItemID.LavaCharm, Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                            break;

                        case 6:
                            itemsToAdd.Add((ItemID.Obsidian, Main.rand.Next(1, 20)));
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 15)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
                            itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
                            break;


                        case 7:
                            itemsToAdd.Add((ItemID.WaterWalkingBoots, Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.Fireblossom, Main.rand.Next(2, 15)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
                            break;

                        case 8:
                            itemsToAdd.Add((ItemID.ObsidianSkull, Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
                            break;
                    }

                    // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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
        }



        private void WorldGenVirulentStructures(GenerationProgress progress, GameConfiguration configuration)
        {
            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Hunters getting kicked out";


            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-06 - 5); k++)
            {
                // 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
                int xa = WorldGen.genRand.Next(500, Main.maxTilesX - 500);
                int ya = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.rockLayerHigh);
                Point Loc = new Point(xa, ya);

                // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                Tile tile = Main.tile[Loc.X, Loc.Y];

                if (!(tile.TileType == ModContent.TileType<Tiles.Acid.AcidialDirt>() || tile.TileType == TileID.Mud))
                {
                    continue;
                }

                if (!tile.HasTile)
                    continue;

                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Acid/A3"))
                    continue;

                int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Acid/A3");
                foreach (int chestIndex in ChestIndexs)
                {
                    var chest = Main.chest[chestIndex];
                    // etc

                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                    var itemsToAdd = new List<(int type, int stack)>();

                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                            Tuple.Create((int)ItemID.Acorn, 0.1),
                            Tuple.Create((int)ItemID.ManaCrystal, 0.1),
                        Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
                            Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.7)

                    );
                    if (specialItem != ItemID.None)
                    {
                        itemsToAdd.Add((specialItem, 1));
                    }
                    // Using a switch statement and a random choice to add sets of items.
                    switch (Main.rand.Next(4))
                    {
                        case 0:

                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));

                            itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.JungleSpores, 7));

                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));

                            itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                            break;
                        case 2:

                            itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                            itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                            break;
                        case 3:

                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(10, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                            break;
                    }

                    // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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




            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-06 - 4); k++)
            {
                // 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
                int xa = WorldGen.genRand.Next(500, Main.maxTilesX - 200);
                int ya = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.rockLayerHigh);
                Point Loc = new Point(xa, ya);

                // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                Tile tile = Main.tile[Loc.X, Loc.Y];

                if (!(tile.TileType == ModContent.TileType<Tiles.Acid.AcidialDirt>() || tile.TileType == TileID.Mud || tile.TileType == TileID.Stone))
                {
                    continue;
                }

                if (tile.HasTile)
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Acid/A3");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                            Tuple.Create((int)ItemID.Acorn, 0.1),
                                Tuple.Create((int)ItemID.ManaCrystal, 0.1),
                            Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
                                Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.7)

                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(4))
                        {
                            case 0:


                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));

                                itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.JungleSpores, 7));

                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                break;
                            case 2:

                                itemsToAdd.Add((ItemID.Daybloom, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:

                                itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(30, 55)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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
            }

        }






























        private void WorldGenMorrowedStructures(GenerationProgress progress, GameConfiguration configuration)
        {
            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Hunters settling down";
            string[] pool = new string[]
            {
                "Struct/Morrow/MorrowStructHouse1",
                "Struct/Morrow/MorrowedSmallStruct",
                "Struct/Morrow/MorrowUnder1",
                "Struct/Morrow/MorrowStructHouseM",
            };

            //actually nvm im not gonna change this

            for (int k = 0; k < 2; k++)
            {
                // 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
                int xa = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
                int ya = WorldGen.genRand.Next((int)GenVars.rockLayerLow + 200, (int)GenVars.rockLayerHigh + 200);
                Point Loc = new Point(xa, ya);

                // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                Tile tile = Main.tile[Loc.X, Loc.Y];
                if (tile.HasTile && StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Morrow/MorrowStructHouse1"))
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowStructHouse1");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc				

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                            Tuple.Create((int)ItemID.Acorn, 0.1),
                            Tuple.Create(ModContent.ItemType<MorrowSalface>(), 0.1),
                            Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
                                Tuple.Create(ModContent.ItemType<MorrowValswa>(), 0.6),
                                Tuple.Create(ModContent.ItemType<MorrowSword>(), 0.9),
                                Tuple.Create(ModContent.ItemType<MorrowRapier>(), 0.7),
                                Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8),
                            Tuple.Create(ModContent.ItemType<Bongos>(), 0.4) // Choose no item with a high weight of 7.
                        );

                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(4))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));

                                itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.Duck, 1));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                break;
                            case 2:
                                itemsToAdd.Add((ModContent.ItemType<MorrowRapier>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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
            }

            for (int k = 0; k < 2; k++)
            {
                // 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
                int xa = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
                int ya = WorldGen.genRand.Next((int)GenVars.rockLayerLow + 150, (int)GenVars.rockLayerHigh + 150);
                Point Loc = new Point(xa, ya);
                // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                Tile tile = Main.tile[Loc.X, Loc.Y];
                if (tile.HasTile && tile.TileType == TileID.Dirt && StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Morrow/MorrowedSmallStruct"))
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowedSmallStruct");
                    Chest c = Main.chest[ChestIndexs[0]];
                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                    var itemsToAdd = new List<(int type, int stack)>();

                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                        Tuple.Create((int)ItemID.Acorn, 0.1),
                        Tuple.Create(ModContent.ItemType<MorrowSalface>(), 0.1),
                        Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
                            Tuple.Create(ModContent.ItemType<MorrowValswa>(), 0.6),
                            Tuple.Create(ModContent.ItemType<MorrowSword>(), 0.9),
                            Tuple.Create(ModContent.ItemType<MorrowRapier>(), 0.7),
                            Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8),
                        Tuple.Create(ModContent.ItemType<Bongos>(), 0.4) // Choose no item with a high weight of 7.
                    );
                    if (specialItem != ItemID.None)
                    {
                        itemsToAdd.Add((specialItem, 1));
                    }
                    // Using a switch statement and a random choice to add sets of items.
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));

                            itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.Duck, 1));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));

                            itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                            break;
                        case 2:
                            itemsToAdd.Add((ModContent.ItemType<MorrowRapier>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                            itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                            break;
                        case 3:
                            itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(10, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                            break;
                    }

                    // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
                    int chestItemIndex = 0;
                    foreach (var itemToAdd in itemsToAdd)
                    {
                        Item item = new Item();
                        item.SetDefaults(itemToAdd.type);
                        item.stack = itemToAdd.stack;
                        c.item[chestItemIndex] = item;
                        chestItemIndex++;
                        if (chestItemIndex >= 40)
                            break; // Make sure not to exceed the capacity of the chest
                    }
                }
            }


            for (int g = 0; g < 1; g++)
            {
                int xab = WorldGen.genRand.Next(0, Main.maxTilesX);
                int yab = WorldGen.genRand.Next((int)GenVars.rockLayerHigh, Main.maxTilesY);
                Point Loc = new Point(xab, yab);
                if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
                {
                    continue;
                }

                Tile tile = Main.tile[Loc.X, Loc.Y];
                if (tile.HasTile && StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Morrow/MorrowUnder1"))
                {
                    StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowUnder1");
                }
            }


            for (int k = 0; k < 1; k++)
            {
                int xab = WorldGen.genRand.Next(-50, -40);
                int yab = WorldGen.genRand.Next(-200, -190);
                Point Loc = new Point(pointVeri.X + xab, pointVeri.Y + yab);
                if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
                {

                    continue;
                }

                Tile tile = Main.tile[Loc.X, Loc.Y];
                if (tile.HasTile && StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Morrow/MorrowStructHouseM"))
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowStructHouseM");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.


                        // itemsToAdd will hold type and stack data for each item we want to add to the chest


                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                            Tuple.Create((int)ItemID.Acorn, 0.1),
                            Tuple.Create(ModContent.ItemType<MorrowSalface>(), 0.1),
                            Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
                                Tuple.Create(ModContent.ItemType<MorrowValswa>(), 0.6),
                                Tuple.Create(ModContent.ItemType<MorrowSword>(), 0.9),
                                Tuple.Create(ModContent.ItemType<MorrowRapier>(), 0.7),
                                Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8),
                            Tuple.Create(ModContent.ItemType<Bongos>(), 0.4) // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(4))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.Duck, 1));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
                                break;
                            case 2:
                                itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<BroochesTableI>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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


            }



            for (int k = 0; k < 1; k++)
            {
                int xab = WorldGen.genRand.Next(-50, -40);
                int yab = WorldGen.genRand.Next(-150, -149);
                Point Loc = new Point(pointVeri.X + xab, pointVeri.Y + yab);
                if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
                {

                    continue;
                }

                Tile tile = Main.tile[Loc.X, Loc.Y];
                if (tile.HasTile && StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Morrow/MorrowStructHouseM"))
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowStructHouseM");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.


                        // itemsToAdd will hold type and stack data for each item we want to add to the chest


                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                            Tuple.Create((int)ItemID.Acorn, 0.1),
                            Tuple.Create(ModContent.ItemType<MorrowSalface>(), 0.1),
                            Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
                                Tuple.Create(ModContent.ItemType<MorrowValswa>(), 0.6),
                                Tuple.Create(ModContent.ItemType<MorrowSword>(), 0.9),
                                Tuple.Create(ModContent.ItemType<MorrowRapier>(), 0.7),
                                Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8),
                            Tuple.Create(ModContent.ItemType<Bongos>(), 0.4) // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(4))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.Duck, 1));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
                                break;
                            case 2:
                                itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<BroochesTableI>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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
            }



            for (int k = 0; k < 1; k++)
            {
                int xab = WorldGen.genRand.Next(-50, -40);
                int yab = WorldGen.genRand.Next(-100, -99);
                Point Loc = new Point(pointVeri.X + xab, pointVeri.Y + yab);
                if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
                {

                    continue;
                }

                Tile tile = Main.tile[Loc.X, Loc.Y];
                if (tile.HasTile && StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Morrow/MorrowStructHouseM"))
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowStructHouseM");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.


                        // itemsToAdd will hold type and stack data for each item we want to add to the chest


                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                            Tuple.Create((int)ItemID.Acorn, 0.1),
                            Tuple.Create(ModContent.ItemType<MorrowSalface>(), 0.1),
                            Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
                                Tuple.Create(ModContent.ItemType<MorrowValswa>(), 0.6),
                                Tuple.Create(ModContent.ItemType<MorrowSword>(), 0.9),
                                Tuple.Create(ModContent.ItemType<MorrowRapier>(), 0.7),
                                Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8),
                            Tuple.Create(ModContent.ItemType<Bongos>(), 0.4) // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(4))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.Duck, 1));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
                                break;
                            case 2:
                                itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<BroochesTableI>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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
            }

            for (int k = 0; k < 1; k++)
            {
                int xab = WorldGen.genRand.Next(-50, -40);
                int yab = WorldGen.genRand.Next(-50, -49);
                Point Loc = new Point(pointVeri.X + xab, pointVeri.Y + yab);
                if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
                {

                    continue;
                }

                Tile tile = Main.tile[Loc.X, Loc.Y];
                if (tile.HasTile && StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Morrow/MorrowStructHouseM"))
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowStructHouseM");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.


                        // itemsToAdd will hold type and stack data for each item we want to add to the chest


                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                            Tuple.Create((int)ItemID.Acorn, 0.1),
                            Tuple.Create(ModContent.ItemType<MorrowSalface>(), 0.1),
                            Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
                                Tuple.Create(ModContent.ItemType<MorrowValswa>(), 0.6),
                                Tuple.Create(ModContent.ItemType<MorrowSword>(), 0.9),
                                Tuple.Create(ModContent.ItemType<MorrowRapier>(), 0.7),
                                Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8),
                            Tuple.Create(ModContent.ItemType<Bongos>(), 0.4) // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(4))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.Duck, 1));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
                                break;
                            case 2:
                                itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<BroochesTableI>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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
            }

            for (int k = 0; k < 1; k++)
            {
                int xab = WorldGen.genRand.Next(-50, -40);
                int yab = WorldGen.genRand.Next(-250, -249);
                Point Loc = new Point(pointVeri.X + xab, pointVeri.Y + yab);
                if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
                {

                    continue;
                }

                Tile tile = Main.tile[Loc.X, Loc.Y];
                if (tile.HasTile && StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Morrow/MorrowStructHouseM"))
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowStructHouseM");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.


                        // itemsToAdd will hold type and stack data for each item we want to add to the chest


                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                            Tuple.Create((int)ItemID.Acorn, 0.1),
                            Tuple.Create(ModContent.ItemType<MorrowSalface>(), 0.1),
                            Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
                                Tuple.Create(ModContent.ItemType<MorrowValswa>(), 0.6),
                                Tuple.Create(ModContent.ItemType<MorrowSword>(), 0.9),
                                Tuple.Create(ModContent.ItemType<MorrowRapier>(), 0.7),
                                Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8),
                            Tuple.Create(ModContent.ItemType<Bongos>(), 0.4) // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(4))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.Duck, 1));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
                                break;
                            case 2:
                                itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<BroochesTableI>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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
            }

            for (int k = 0; k < 1; k++)
            {
                int xab = WorldGen.genRand.Next(-50, -40);
                int yab = WorldGen.genRand.Next(-300, -299);
                Point Loc = new Point(pointVeri.X + xab, pointVeri.Y + yab);
                if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
                {

                    continue;
                }

                Tile tile = Main.tile[Loc.X, Loc.Y];
                if (tile.HasTile && StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Morrow/MorrowStructHouseM"))
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowStructHouseM");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.


                        // itemsToAdd will hold type and stack data for each item we want to add to the chest


                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                            Tuple.Create((int)ItemID.Acorn, 0.1),
                            Tuple.Create(ModContent.ItemType<MorrowSalface>(), 0.1),
                            Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
                                Tuple.Create(ModContent.ItemType<MorrowValswa>(), 0.6),
                                Tuple.Create(ModContent.ItemType<MorrowSword>(), 0.9),
                                Tuple.Create(ModContent.ItemType<MorrowRapier>(), 0.7),
                                Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8),
                            Tuple.Create(ModContent.ItemType<Bongos>(), 0.4) // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(4))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.Duck, 1));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
                                break;
                            case 2:
                                itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<BroochesTableI>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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
            }
        }

        private void WorldGenWorshipingTowers(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Praising our lord and savior Niivi";

            int leftmostJungleTileX = int.MaxValue;
            int rightmostJungleTileX = int.MinValue;
            for (int x = 500; x < Main.maxTilesX - 500; x++)
            {
                int jungleY = (int)(Main.worldSurface - 50);
                while (!WorldGen.SolidTile(x, jungleY) && jungleY <= Main.worldSurface)
                {
                    jungleY++;
                }

                Tile tile = Main.tile[x, jungleY];
                if (tile.TileType == TileID.Mud)
                {
                    if (leftmostJungleTileX > x)
                        leftmostJungleTileX = x;
                    if (rightmostJungleTileX < x)
                        rightmostJungleTileX = x;
                }
            }

            string[] structures = new string[]
            {
                "Struct/Jungle/WorshipingTower1",
                "Struct/Jungle/WorshipingTower2",
                "Struct/Jungle/WorshipingTower3"
            };

            int[] tileBlend = new int[]
            {
                TileID.RubyGemspark
            };


            int numberToPlace = Main.rand.Next(10, 17);
            int attempts = 0;
            int maxAttempts = 100000;
            for (int k = 0; k < numberToPlace; k++)
            {
                bool placed = false;
                if (attempts > maxAttempts)
                    break;

                while (!placed)
                {
                    attempts++;
                    if (attempts > maxAttempts)
                        break;
                    int xa = WorldGen.genRand.Next(leftmostJungleTileX, rightmostJungleTileX);
                    int ya = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY);
                    Point Loc = new Point(xa, ya);

                    // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                    Tile tile = Main.tile[Loc.X, Loc.Y];

                    if (!(tile.TileType == TileID.Mud))
                    {
                        continue;
                    }

                    if (!tile.HasTile)
                        continue;

                    string randomStructure = structures[Main.rand.Next(0, structures.Length)];


                    //Avoid the temple
                    Rectangle structureRect = StructureLoader.ReadRectangle(randomStructure);
                    structureRect.Location = Loc;
                    if (!StructureLoader.TryPlaceAndProtectStructure(Loc, randomStructure))
                        continue;

                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, randomStructure, tileBlend);

                    //GUARDS!!!
                    IllurianGuardSpawnSystem.Add(Loc, randomStructure);
                    placed = true;
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        int tileType = Main.tile[chest.x, chest.y].TileType;
                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        if (tileType == ModContent.TileType<IlluriaChest>())
                        {
                            //Illura Chest Loot here
                            switch (Main.rand.Next(6))
                            {
                                case 0:
                                    //Illuria Brooch
                                    itemsToAdd.Add((ModContent.ItemType<IllurianBroochA>(), 1));
                                    break;
                                case 1:
                                    //Alcalite Set
                                    itemsToAdd.Add((ModContent.ItemType<AlcaliteMask>(), 1));
                                    itemsToAdd.Add((ModContent.ItemType<AlcaliteRobe>(), 1));
                                    itemsToAdd.Add((ModContent.ItemType<AlcaliteTrunks>(), 1));
                                    break;
                                case 2:
                                    //Illurite Dril
                                    itemsToAdd.Add((ModContent.ItemType<IlluriteDrill>(), 1));
                                    break;
                                case 3:
                                    itemsToAdd.Add((ModContent.ItemType<IllurianLoveLocket>(), 1));
                                    break;
                                case 4:
                                    itemsToAdd.Add((ModContent.ItemType<MsFreeze>(), 1));
                                    break;
                                case 5:
                                    itemsToAdd.Add((ModContent.ItemType<IllurianBible>(), 1));
                                    break;
                            }

                            switch (Main.rand.Next(1))
                            {
                                case 0:
                                    itemsToAdd.Add((ModContent.ItemType<IllurineScale>(), Main.rand.Next(2, 5)));
                                    itemsToAdd.Add((ItemID.LifeFruit, Main.rand.Next(1, 4)));
                                    itemsToAdd.Add((ItemID.Ectoplasm, Main.rand.Next(2, 5)));
                                    break;
                            }
                        }
                        else
                        {
                            //Jungle Loot Here
                            int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                                Tuple.Create((int)ItemID.AnkletoftheWind, 0.5),
                                Tuple.Create((int)ItemID.StaffofRegrowth, 0.5),
                                Tuple.Create((int)ItemID.FlowerBoots, 0.5),
                                Tuple.Create((int)ItemID.Boomstick, 0.5),
                                Tuple.Create(ModContent.ItemType<JungleRuneI>(), 0.15)
                                );

                            itemsToAdd.Add((specialItem, 1));
                            switch (Main.rand.Next(4))
                            {
                                case 0:
                                    itemsToAdd.Add((ModContent.ItemType<FlowerBatch>(), Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.JungleSpores, Main.rand.Next(2, 5)));
                                    itemsToAdd.Add((ItemID.Stinger, Main.rand.Next(3, 7)));
                                    break;
                                case 1:
                                    itemsToAdd.Add((ItemID.Vine, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.JungleSpores, Main.rand.Next(2, 5)));
                                    itemsToAdd.Add((ItemID.Stinger, Main.rand.Next(3, 7)));
                                    break;
                                case 2:
                                    itemsToAdd.Add((ModContent.ItemType<FlowerBatch>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ItemID.Vine, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.CalmingPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                    break;
                                case 3:
                                    itemsToAdd.Add((ItemID.Stinger, Main.rand.Next(3, 7)));
                                    itemsToAdd.Add((ItemID.RecallPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                    break;
                            }
                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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
            }
        }

        private void WorldGenCathedral(GenerationProgress progress, GameConfiguration configuration)
        {


            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Verlia Ark";

            Point snowBounds = SnowBounds();
            bool placed = false;
            int attempts = 0;
            string structure = "Struct/Ice/VerliasCathedral";
            while (!placed && attempts < 1000)
            {
                attempts++;
                // Select a place in the first 6th of the world, avoiding the oceans
                int towerX = snowBounds.Y + WorldGen.genRand.Next(0, 250);
                int towerY = FindSurfacePoint(towerX).Y; //(int)Main.worldSurface - 200;
                Point Loc = new Point(towerX, towerY - 50);
                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, structure))
                    continue;

                for (int da = 0; da < 1; da++)
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, structure);
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                            Tuple.Create(ModContent.ItemType<EmptyMoonflameLantern>(), 0.5)

                        // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(6))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<LittleWand>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(5, 20)));
                                ;
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ModContent.ItemType<AlcaricQuiver>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
                                break;
                            case 2:
                                itemsToAdd.Add((ModContent.ItemType<BlackRose>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<FloweredInsource>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<AlcadizScrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                            case 4:
                                itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                break;

                            case 5:
                                itemsToAdd.Add((ItemID.FuneralHat, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;


                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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


                    placed = true;

                }
            }

        }




        private void WorldGenVeldris(GenerationProgress progress, GameConfiguration configuration)
        {

            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Veldris Building his house";

            bool placed = false;
            int attempts = 0;
            Point snowBounds = SnowBounds();
            var genRand = WorldGen.genRand;
            string structure = "Struct/Ice/VeldrisHouse";
            int randX = 0;
            while (!placed && attempts++ < 100000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int towerX = snowBounds.Y + randX; // genRand.Next(0, 100);
                int towerY = FindSurfacePoint(towerX).Y;
                Point Loc = new Point(towerX, towerY + 14);

                // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                StructureMap structures = GenVars.structures;
                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, structure))
                {
                    randX = genRand.Next(0, 100);
                    continue;
                }
                int[] ChestIndexs = StructureLoader.ReadStruct(Loc, structure);
                NPCs.Town.AlcadSpawnSystem.VelTile = Loc;
                foreach (int chestIndex in ChestIndexs)
                {
                    var chest = Main.chest[chestIndex];
                    // etc

                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                    var itemsToAdd = new List<(int type, int stack)>();

                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                        Tuple.Create(ModContent.ItemType<FrostSwing>(), 0.5)

                    // Choose no item with a high weight of 7.
                    );
                    if (specialItem != ItemID.None)
                    {
                        itemsToAdd.Add((specialItem, 1));
                    }
                    // Using a switch statement and a random choice to add sets of items.
                    switch (Main.rand.Next(6))
                    {
                        case 0:
                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
                            break;

                        case 2:
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                            itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                            break;
                        case 3:
                            itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                            break;
                        case 4:
                            itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            break;

                        case 5:
                            itemsToAdd.Add((ItemID.FuneralHat, Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                            itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                            break;


                    }

                    // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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
                placed = true;
            }

        }













        private void FallToSolidTile(int towerX, ref int towerY)
        {
            for (int x = 0; x < 300; x++)
            {
                if (WorldGen.SolidTile(towerX, towerY))
                {
                    break;
                }
                else
                {
                    towerY++;
                }
            }
        }



        private void WorldGenTS(GenerationProgress progress, GameConfiguration configuration)
        {

            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Sigfried being demoralized";

            int[] tileBlend = new int[]
            {
                ModContent.TileType<VeriplantGrass>()
            };
            bool placed = false;
            int attempts = 0;
            Point snowBounds = SnowBounds();
            while (!placed && attempts++ < 100000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int towerX = snowBounds.X;
                towerX += WorldGen.genRand.Next(-150, 0);
                int towerY = FindSurfacePoint(towerX).Y;

                //Try to put the structure here and protect it
                string path = "Struct/Overworld/TowerSigfried";
                Point Loc = new Point(towerX, towerY + 20);
                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, path))
                {
                    continue;
                }

                for (int daa = 0; daa < 1; daa++)
                {
                    Point Loc2 = new Point(towerX, towerY + 21);
                    WorldUtils.Gen(Loc2, new Shapes.Rectangle(45, 25), new Actions.SetTile(TileID.SnowBlock));
                }
                int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path, tileBlend);
                NPCs.Town.AlcadSpawnSystem.SireTile = Loc;
                foreach (int chestIndex in ChestIndexs)
                {
                    var chest = Main.chest[chestIndex];
                    // etc

                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                    var itemsToAdd = new List<(int type, int stack)>();

                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                        Tuple.Create(ModContent.ItemType<VeiledScriptureSigfried>(), 0.5)

                    // Choose no item with a high weight of 7.
                    );
                    if (specialItem != ItemID.None)
                    {
                        itemsToAdd.Add((specialItem, 1));
                    }
                    // Using a switch statement and a random choice to add sets of items.
                    switch (Main.rand.Next(5))
                    {
                        case 0:
                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
                            break;

                        case 2:
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                            itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                            break;
                        case 3:
                            itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                            break;
                        case 4:
                            itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            break;



                    }

                    // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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
                placed = true;
            }

        }

        private void WorldGenTA(GenerationProgress progress, GameConfiguration configuration)
        {

            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Azurerin Sleeping the whole time";

            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 100000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int towerX = WorldGen.genRand.Next(500, Main.maxTilesX - 500); // from 50 since there's a unaccessible area at the world's borders
                                                                               // 50% of choosing the last 6th of the world
                                                                               // Choose which side of the world to be on randomly
                ///if (WorldGen.genRand.NextBool())
                ///{
                ///	towerX = Main.maxTilesX - towerX;
                ///}

                //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                int towerY = (int)Main.worldSurface - 200;

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(towerX, towerY) && towerY <= Main.worldSurface)
                {
                    towerY++;
                }

                // If we went under the world's surface, try again
                if (towerY > Main.worldSurface)
                {
                    continue;
                }
                Tile tile = Main.tile[towerX, towerY];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == TileID.Mud || tile.TileType == TileID.JungleGrass))
                {
                    continue;
                }

                string path = "Struct/Overworld/TowerAzurerin";
                Point Loc = new Point(towerX, towerY + 20);
                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, path))
                {
                    continue;
                }


                for (int da = 0; da < 1; da++)
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path);
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                            Tuple.Create(ModContent.ItemType<VeiledScriptureAzurerin>(), 0.5)

                        // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(6))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
                                break;

                            case 2:
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                            case 4:
                                itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                break;

                            case 5:
                                itemsToAdd.Add((ModContent.ItemType<SirestiasToken>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;


                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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


                    placed = true;

                }
            }

        }

        private void WorldGenTC(GenerationProgress progress, GameConfiguration configuration)
        {

            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Cozmire getting her singularity stolen";

            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 100000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int towerX = WorldGen.genRand.Next(500, Main.maxTilesX - 500); // from 50 since there's a unaccessible area at the world's borders
                                                                               // 50% of choosing the last 6th of the world
                                                                               // Choose which side of the world to be on randomly
                ///if (WorldGen.genRand.NextBool())
                ///{
                ///	towerX = Main.maxTilesX - towerX;
                ///}

                //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                int towerY = (int)Main.worldSurface - 200;

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(towerX, towerY) && towerY <= Main.worldSurface)
                {
                    towerY++;
                }

                // If we went under the world's surface, try again
                if (towerY > Main.worldSurface)
                {
                    continue;
                }
                Tile tile = Main.tile[towerX, towerY];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == TileID.Sandstone || tile.TileType == TileID.Sand))
                {
                    continue;
                }

                string path = "Struct/Overworld/TowerCozmire";
                Point Loc = new Point(towerX, towerY + 20);
                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, path))
                {
                    continue;
                }

                for (int da = 0; da < 1; da++)
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path);
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                            Tuple.Create(ModContent.ItemType<VeiledScriptureCozmire>(), 0.5)

                        // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(6))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
                                break;

                            case 2:
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                            case 4:
                                itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                break;


                            case 5:
                                itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                break;


                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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


                    placed = true;

                }
            }

        }

        private void WorldGenTL(GenerationProgress progress, GameConfiguration configuration)
        {

            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Lumi collecting singularities";

            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 1000000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int towerX = WorldGen.genRand.Next(Main.maxTilesX - 500, Main.maxTilesX - 220); // from 50 since there's a unaccessible area at the world's borders
                                                                                                // 50% of choosing the last 6th of the world
                                                                                                // Choose which side of the world to be on randomly
                ///if (WorldGen.genRand.NextBool())
                ///{
                ///	towerX = Main.maxTilesX - towerX;
                ///}

                //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                int towerY = (int)Main.worldSurface - 200;

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(towerX, towerY) && towerY <= Main.worldSurface)
                {
                    towerY++;
                }

                // If we went under the world's surface, try again
                if (towerY > Main.worldSurface)
                {
                    continue;
                }
                Tile tile = Main.tile[towerX, towerY];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == TileID.Dirt
                     || tile.TileType == ModContent.TileType<VeriplantGrass>()
                     || tile.TileType == TileID.Grass
                     || tile.TileType == TileID.Sand))
                {
                    continue;
                }

                string path = "Struct/Overworld/TowerLumi";
                Point Loc = new Point(towerX, towerY + 20);
                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, path))
                {
                    continue;
                }

                for (int da = 0; da < 1; da++)
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path);
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                            Tuple.Create(ModContent.ItemType<VeiledScriptureLumi>(), 0.5)

                        // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(6))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
                                break;

                            case 2:
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                            case 4:
                                itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                break;


                            case 5:
                                itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                break;


                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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


                    placed = true;

                }
            }

        }
        private void WorldGenTG(GenerationProgress progress, GameConfiguration configuration)
        {

            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Gothivia preparing her escape.";

            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 100000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int towerX = WorldGen.genRand.Next(210, 500); // from 50 since there's a unaccessible area at the world's borders
                                                              // 50% of choosing the last 6th of the world
                                                              // Choose which side of the world to be on randomly
                ///if (WorldGen.genRand.NextBool())
                ///{
                ///	towerX = Main.maxTilesX - towerX;
                ///}

                //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                int towerY = (int)Main.worldSurface - 200;

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(towerX, towerY) && towerY <= Main.worldSurface)
                {
                    towerY++;
                }

                // If we went under the world's surface, try again
                if (towerY > Main.worldSurface)
                {
                    continue;
                }
                Tile tile = Main.tile[towerX, towerY];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == TileID.Sand
                    || tile.TileType == TileID.Dirt))
                {
                    continue;
                }

                //Try to put the structure here and protect it
                string path = "Struct/Overworld/TowerGothivia";
                Point Loc = new Point(towerX, towerY + 20);
                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, path))
                {
                    continue;
                }

                for (int da = 0; da < 1; da++)
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path);
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                            Tuple.Create(ModContent.ItemType<VeiledScriptureGothivia>(), 0.5)

                        // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(6))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
                                break;

                            case 2:
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                            case 4:
                                itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                break;

                            case 5:
                                itemsToAdd.Add((ModContent.ItemType<SirestiasToken>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;


                        }

                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
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


                    placed = true;

                }
            }

        }


        private void WorldGenDreadMonoliths(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Dreading..";
            int[] tileBlend = new int[]
            {
                TileID.RubyGemspark
            };

            bool placed = false;
            int attempts = 0;

            while (!placed && attempts++ < 100000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int towerX = WorldGen.genRand.Next(NPCs.Town.AlcadSpawnSystem.AlcadTile.X + 400, NPCs.Town.AlcadSpawnSystem.AlcadTile.X + 800); // from 50 since there's a unaccessible area at the world's borders
                int towerY = (int)Main.worldSurface - 200;

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(towerX, towerY) && towerY <= Main.worldSurface)
                {
                    towerY++;
                }

                // If we went under the world's surface, try again
                if (towerY > Main.worldSurface)
                {
                    continue;
                }

                //Try to put the structure here and protect it
                string path = "Struct/Overworld/DreadMonolith";
                Point Loc = new Point(towerX, towerY - 105);
                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, path))
                {
                    continue;
                }

                for (int da = 0; da < 1; da++)
                {
                    StructureLoader.ReadStruct(Loc, path, tileBlend);
                    NPCs.Town.AlcadSpawnSystem.DreadMonolithTile1 = Loc;
                    placed = true;
                }
            }


            placed = false;
            while (!placed && attempts++ < 100000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int towerX = WorldGen.genRand.Next(NPCs.Town.AlcadSpawnSystem.IlluriaTile.X - 800, NPCs.Town.AlcadSpawnSystem.IlluriaTile.X - 400); // from 50 since there's a unaccessible area at the world's borders
                int towerY = (int)Main.worldSurface - 200;

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(towerX, towerY) && towerY <= Main.worldSurface)
                {
                    towerY++;
                }

                // If we went under the world's surface, try again
                if (towerY > Main.worldSurface)
                {
                    continue;
                }

                //Try to put the structure here and protect it
                string path = "Struct/Overworld/DreadMonolith";
                Point Loc = new Point(towerX, towerY - 105);
                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, path))
                {
                    continue;
                }

                for (int da = 0; da < 1; da++)
                {
                    StructureLoader.ReadStruct(Loc, path, tileBlend);
                    NPCs.Town.AlcadSpawnSystem.DreadMonolithTile2 = Loc;
                    placed = true;
                }
            }


            placed = false;
            while (!placed && attempts++ < 100000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int center = Main.maxTilesX / 2;
                int towerX = WorldGen.genRand.Next(center - 300, center + 300); // from 50 since there's a unaccessible area at the world's borders
                int towerY = (int)Main.worldSurface - 200;

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(towerX, towerY) && towerY <= Main.worldSurface)
                {
                    towerY++;
                }

                // If we went under the world's surface, try again
                if (towerY > Main.worldSurface)
                {
                    continue;
                }

                //Try to put the structure here and protect it
                string path = "Struct/Overworld/DreadMonolith";
                Point Loc = new Point(towerX, towerY - 125);
                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, path))
                {
                    continue;
                }

                for (int da = 0; da < 1; da++)
                {
                    StructureLoader.ReadStruct(Loc, path, tileBlend);
                    NPCs.Town.AlcadSpawnSystem.DreadMonolithTile3 = Loc;
                    placed = true;
                }
            }
        }

        const string SavestringX = "Savestring1";
        const string SavestringY = "Savestring2";


        public static Point MorrowEdge = new Point(0, 0);
        public static Point MorrowEdgeY = new Point(0, 0);

        private void WorldGenMorrow(GenerationProgress progress, GameConfiguration configuration)
        {
            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Gild settling in the ground";
            int xa = WorldGen.genRand.Next(0, Main.maxTilesX / 2);
            int ya = WorldGen.genRand.Next((int)GenVars.worldSurface, Main.maxTilesY / 2);

            int yb = WorldGen.genRand.Next((int)GenVars.worldSurface, (int)GenVars.worldSurface);


            for (int da = 0; da < 1; da++)
            {
                WorldGen.TileRunner(xa, ya, WorldGen.genRand.Next(1100, 1100), WorldGen.genRand.Next(1100, 1100), ModContent.TileType<OvermorrowdirtTile>());


            }

            MorrowEdge.X = xa - 500;
            MorrowEdge.Y = ya - 100;
            MorrowEdgeY.Y = yb - 100;
            MorrowEdgeY.X = xa - 100;




            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07 + 56); k++)
            {
                Point Loc = new Point(MorrowEdge.X + Main.rand.Next(0, 900), MorrowEdge.Y + Main.rand.Next(0, 900));


                if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
                {

                    continue;
                }

                Tile tile = Main.tile[Loc.X, Loc.Y];
                if (tile.HasTile && tile.TileType == ModContent.TileType<OvermorrowdirtTile>())
                {
                    StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowUnder1");
                }








            }








            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-06 + 6); k++)
            {
                Point Loc = new Point(MorrowEdge.X + Main.rand.Next(0, 1100), MorrowEdge.Y + Main.rand.Next(0, 1000));


                if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
                {

                    continue;
                }

                Tile tile = Main.tile[Loc.X, Loc.Y];
                if (tile.HasTile && tile.TileType == ModContent.TileType<OvermorrowdirtTile>())
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowStructHouse1");
                    Chest c = Main.chest[ChestIndexs[0]];
                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                    var itemsToAdd = new List<(int type, int stack)>();

                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                        Tuple.Create((int)ItemID.Acorn, 0.1),
                        Tuple.Create(ModContent.ItemType<MorrowSalface>(), 0.1),
                        Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
                            Tuple.Create(ModContent.ItemType<MorrowValswa>(), 0.6),
                            Tuple.Create(ModContent.ItemType<MorrowSword>(), 0.9),
                            Tuple.Create(ModContent.ItemType<MorrowRapier>(), 0.7),
                            Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8),
                        Tuple.Create(ModContent.ItemType<Bongos>(), 0.4) // Choose no item with a high weight of 7.
                    );
                    if (specialItem != ItemID.None)
                    {
                        itemsToAdd.Add((specialItem, 1));
                    }
                    // Using a switch statement and a random choice to add sets of items.
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));

                            itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.Duck, 1));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                            break;
                        case 2:
                            itemsToAdd.Add((ModContent.ItemType<MorrowRapier>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                            break;
                        case 3:
                            itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(10, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                            break;
                    }

                    // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
                    int chestItemIndex = 0;
                    foreach (var itemToAdd in itemsToAdd)
                    {
                        Item item = new Item();
                        item.SetDefaults(itemToAdd.type);
                        item.stack = itemToAdd.stack;
                        c.item[chestItemIndex] = item;
                        chestItemIndex++;
                        if (chestItemIndex >= 40)
                            break; // Make sure not to exceed the capacity of the chest
                    }
                }
            }










            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07 + 9); k++)
            {
                Point Loc = new Point(MorrowEdge.X + Main.rand.Next(0, 1000), MorrowEdge.Y + Main.rand.Next(0, 800));


                if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
                {

                    continue;
                }

                Tile tile = Main.tile[Loc.X, Loc.Y];
                if (tile.HasTile && tile.TileType == ModContent.TileType<OvermorrowdirtTile>())
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowStructHouseM");
                    Chest c = Main.chest[ChestIndexs[0]];
                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                    var itemsToAdd = new List<(int type, int stack)>();

                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                        Tuple.Create((int)ItemID.Acorn, 0.1),
                        Tuple.Create(ModContent.ItemType<MorrowSalface>(), 0.1),
                        Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
                            Tuple.Create(ModContent.ItemType<MorrowValswa>(), 0.6),
                            Tuple.Create(ModContent.ItemType<MorrowSword>(), 0.9),
                            Tuple.Create(ModContent.ItemType<MorrowRapier>(), 0.7),
                            Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8),
                        Tuple.Create(ModContent.ItemType<Bongos>(), 0.4) // Choose no item with a high weight of 7.
                    );
                    if (specialItem != ItemID.None)
                    {
                        itemsToAdd.Add((specialItem, 1));
                    }
                    // Using a switch statement and a random choice to add sets of items.
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.Duck, 1));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
                            break;
                        case 2:
                            itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                            break;
                        case 3:
                            itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(10, 15)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                            break;
                    }

                    // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
                    int chestItemIndex = 0;
                    foreach (var itemToAdd in itemsToAdd)
                    {
                        Item item = new Item();
                        item.SetDefaults(itemToAdd.type);
                        item.stack = itemToAdd.stack;
                        c.item[chestItemIndex] = item;
                        chestItemIndex++;
                        if (chestItemIndex >= 40)
                            break; // Make sure not to exceed the capacity of the chest
                    }
                }








            }


            for (int da = 0; da < 1; da++)
            {
                Point Loc = new Point(MorrowEdge.X + Main.rand.Next(500, 500), MorrowEdge.Y + Main.rand.Next(25, 25));


                if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
                {

                    continue;
                }

                Tile tile = Main.tile[Loc.X, Loc.Y];
                if (tile.HasTile && tile.TileType == ModContent.TileType<OvermorrowdirtTile>())
                {
                    StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowOutpost");
                }








            }












            for (int i = MorrowEdge.X; i < MorrowEdge.X + 1000; i++)
            {
                for (int j = MorrowEdge.Y; j < MorrowEdge.Y + 600; j++)
                {
                    WorldGen.PlaceWall(i, j, ModContent.WallType<OvermorrowdirtWall>());
                }
            }
        }














        public override void SaveWorldData(TagCompound tag)
        {
            tag[SavestringX] = MorrowEdge.X;
            tag[SavestringY] = MorrowEdge.Y;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            int x = tag.Get<int>(SavestringX);
            int y = tag.Get<int>(SavestringY);
            MorrowEdge = new Point(x, y);
        }
    }
}