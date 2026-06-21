using ReLogic.Utilities;
using Stellamod.Common.DungeonGeneration;
using Stellamod.Content.Areas.Abyss.WeaponsAB;
using Stellamod.Content.Areas.Cinderspark.WeaponsCS;
using Stellamod.Content.Areas.Collosseum.TilesCL;
using Stellamod.Content.Areas.Collosseum.WeaponsCL;
using Stellamod.Content.Areas.Fable.WeaponsFB;
using Stellamod.Content.Areas.Junkyard.TilesJY;
using Stellamod.Content.Areas.SpringHills.AccSH;
using Stellamod.Content.Areas.SpringHills.WeaponsSH;
using Stellamod.Content.Areas.Terror.TilesTR;
using Stellamod.Content.Areas.Underground.TilesUG;
using Stellamod.Content.Areas.WaterSide.TilesWS;
using Stellamod.Content.Areas.WondrousDarkspace.TilesWD;
using Stellamod.Content.Areas.WorldsEnd.TilesWE;
using Stellamod.Content.Armors.Alcalite;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.RibbonSystem;
using Stellamod.Core.Utilities;
using Stellamod.Core.ZTileSystem;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.AlcadChests;
using Stellamod.Items.Armors.Windmillion;
using Stellamod.Items.Consumables;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Items.Quest.Merena;
using Stellamod.Items.Special.MinerLogs;
using Stellamod.Items.Tools;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.Tiles;
using Stellamod.Tiles.Abyss;
using Stellamod.Tiles.Acid;
using Stellamod.Tiles.Illuria;
using Stellamod.Tiles.Veil;
using Stellamod.TilesNew.MothlightTiles;
using Stellamod.TilesNew.RainforestTiles;
using Stellamod.TilesNew.Virulent;
using Stellamod.WorldG.MarshJungle;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.GameContent.Biomes.Desert;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace Stellamod.WorldG;

public class PassWriter
{
    private int _insertionIndex;
    public PassWriter(List<GenPass> tasks)
    {
        this.Tasks = tasks;
    }
    public readonly List<GenPass> Tasks;

    public void SetInsertionIndex(int index)
    {
        _insertionIndex = index;
    }
    public void SetInsertionIndex(string passName) => SetInsertionIndex(Tasks.FindIndex(genpass => genpass.Name.Equals(passName)));
    public void NextPass(GenPass genPass)
    {
        _insertionIndex++;
        Tasks.Insert(_insertionIndex, genPass);

    }
    public void DisablePass(string passName)
    {
        Tasks[Tasks.FindIndex(genpass => genpass.Name.Equals(passName))].Disable();

    }
    public void ReplacePass(GenPass genPass)
    {
        Tasks[_insertionIndex] = genPass;
    }
}


public partial class StellaWorld : ModSystem
{
    public Point RoyalCapitalLocation { get; private set; }
    public Point VeizalHillStartLcoation { get; private set; }
    public Point VeizalHillEndLocation { get; private set; }
    public Point MistyHillStartLocation { get; private set; }
    public Point MistyHillEndLocation { get; private set; }
    public Point MistyDungeonLocation { get; private set; }
    public Point FableFarEdgeLocation { get; private set; }
    public Point FableLocation { get; private set; }
    public Point FableHillStartLocation { get; private set; }
    public Point FableHillEndLocation { get; private set; }
    public Point DesertLocation { get; private set; }
    public Point WitchTownLocation { get; private set; }
    public Point ManorLocation { get; private set; }
    public Point MarshLocation { get; private set; }
    public Point AlcadLocation { get; private set; }
    public Point CoralwaysLocation { get; private set; }
    public Point SnowClumpOriginPoint { get; private set; }
    public Point GothiviaSpawnOffset => new Point(246, -99);
    public Point BublbtrifierSpawnOffset => new Point(246, -99);

    public int CindersparkStart { get; private set; }
    public int CindersparkEnd { get; private set; }
    public int DarkspaceStart { get; private set; }
    public int DarkspaceEnd { get; private set; }
    public int HeatedDepthsStart { get; private set; }
    public int HeatedDepthsEnd { get; private set; }
    public override void Load()
    {
        base.Load();
        On_DesertDescription.CreateFromPlacement += ClampHive;
    }

    private DesertDescription ClampHive(On_DesertDescription.orig_CreateFromPlacement orig, Point origin)
    {
        var description = orig(origin);

        //TODO:
        /*
        Rectangle hiveRect = description.Hive;
        hiveRect.Height = DarkspaceStart - (int)Main.worldSurface;
        hiveRect.Height -= 32;
        description.Hive = hiveRect;*/
        return description;
    }

    private void DisableGenTask(List<GenPass> tasks, string passName)
    {
        tasks.Find(x => x.Name.Equals(passName)).Disable();
    }

    private void DisableAllGenTasks(List<GenPass> tasks)
    {
        foreach (GenPass task in tasks)
        {
            task.Disable();
        }
    }

    /*
    private void AddWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        //We don't need this for now
        int MorrowGen = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
        int RoyalGen = tasks.FindIndex(genpass => genpass.Name.Equals("Corruption"));

        int oceanSand = tasks.FindIndex(genpass => genpass.Name.Equals("Ocean Sand"));
        tasks[oceanSand].Disable();
        tasks.Insert(oceanSand + 1, new ReworkedOceanSandPass());

        oceanSand = tasks.FindIndex(genpass => genpass.Name.Equals("Beaches"));
        tasks[oceanSand].Disable();
        tasks.Insert(oceanSand + 1, new ReworkedBeachesPass());


        int fullDesert = tasks.FindIndex(genpass => genpass.Name.Equals("Full Desert"));
        tasks[fullDesert] = new PassLegacy("Lock Full Desert", LockDesert);

        int terrainIndex = tasks.FindIndex(x => x.Name.Equals("Terrain"));
        if (terrainIndex != -1)
        {
            tasks.Insert(terrainIndex + 1, new VanillaTerrainPass());
            tasks.Insert(terrainIndex + 2, new PassLegacy("Desert Pyr", InitializePyr));
            tasks.Insert(terrainIndex + 3, new PassLegacy("World Gen GenVar Locations", WorldGenVarLocations));
        }

        int iceGen = tasks.FindIndex(genpass => genpass.Name.Equals("Generate Ice Biome"));
        tasks.Insert(iceGen + 1, new ReworkedVanillaIceBiomePass());
        tasks.Insert(iceGen + 2, new PassLegacy("Ice Clumping", IceClump));
        //  tasks.Insert(iceGen + 3, new PassLegacy("Ice Housing 1", InGroundIceHouses));
        //tasks.Insert(iceGen + 4, new PassLegacy("Ice Housing 2", RuneBridges));

        tasks.Insert(iceGen + 3, new PassLegacy("Ice Spikes", MakingIcyRandomness));
        tasks.Insert(iceGen + 4, new PassLegacy("World Gen Abysm", WorldGenAbysm));
        tasks.Insert(iceGen + 5, new PassLegacy("World Gen Abysm Caves", NewCaveFormationAbysm));
        tasks.Insert(iceGen + 6, new PassLegacy("World Gen Ice Ores", WorldGenFrileOre));
        tasks.Insert(iceGen + 7, new PassLegacy("Icey Caverns", WorldGenIceCaverns));
        tasks.Insert(iceGen + 8, new PassLegacy("World Gen Ice Ores", WorldGenGlisteningOre));
        tasks.Insert(iceGen + 9, new PassLegacy("Ice Housing 3", SurfaceIceHouses));
        int shimmerGen = tasks.FindIndex(x => x.Name.Equals("Shimmer"));
        if (shimmerGen != -1)
        {
            tasks.Insert(shimmerGen + 1, new PassLegacy("Fake Shimmer", WorldGenShimmerSpot));
        }

        int caveGen = tasks.FindIndex(x => x.Name.Equals("Jungle"));
        if (caveGen != -1)
        {

            //  tasks.Insert(caveGen + 2, new PassLegacy("Granite Caves", WorldGenMarbleCaves));
            tasks.Insert(caveGen + 1, new MarshJungleMudPass());
            tasks.Insert(caveGen + 2, new PassLegacy("Caves 1", WorldGenCaves));
            tasks.Insert(caveGen + 3, new PassLegacy("Wonderous Darkspace", WorldGenDarkspace));
        }


        if (MorrowGen != -1)
        {
            tasks.Insert(MorrowGen + 1, new PassLegacy("Marsh Jungle", WorldGenMarsh));
            tasks.Insert(MorrowGen + 2, new PassLegacy("World Gen Royal Castle", WorldGenRoyalCapital));
            tasks.Insert(MorrowGen + 3, new PassLegacy("World Gen Worlds End", WorldGenWorldsEnd));
            tasks.Insert(MorrowGen + 4, new PassLegacy("World Gen Other stones", WorldGenDarkstone));
            tasks.Insert(MorrowGen + 5, new PassLegacy("World Gen Flame Ores", WorldGenFlameOre));
            tasks.Insert(MorrowGen + 6, new PassLegacy("World Gen Illuria", WorldGenIlluria));
            tasks.Insert(MorrowGen + 7, new PassLegacy("World Gen Cinderspark", WorldGenCinderspark));
            tasks.Insert(MorrowGen + 8, new PassLegacy("World Gen Cinderspark", WorldGenMoreFlameOre));
            tasks.Insert(MorrowGen + 9, new PassLegacy("World Gen Ice Ores", WorldGenFrileOre));
            tasks.Insert(MorrowGen + 10, new PassLegacy("World Gen Dungeon Location", WorldGenDungeonLocation));
            tasks.Insert(MorrowGen + 11, new PassLegacy("World Gen Misty Dungeon", GenerateMistyDungeon));
        }

        int CathedralGen3 = tasks.FindIndex(genpass => genpass.Name.Equals("Buried Chests"));
        if (CathedralGen3 != -1)
        {
            tasks.Insert(CathedralGen3 + 1, new PassLegacy("World Gen Ambience", WorldGenAmbience));
        }

        int CathedralGen2 = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
        if (CathedralGen2 != -1)
        {
            tasks.Insert(CathedralGen2 + 1, new PassLegacy("World Gen Abandoned Mineshafts", WorldGenMineshafts));
            tasks.Insert(CathedralGen2 + 2, new PassLegacy("World Gen AureTemple", WorldGenAurelusTemple));

            tasks.Insert(CathedralGen2 + 3, new PassLegacy("World Gen Virulent Structures", WorldGenVirulentStructures));
            tasks.Insert(CathedralGen2 + 4, new PassLegacy("World Gen Govheil Castle", WorldGenGovheilCastle));

            tasks.Insert(CathedralGen2 + 5, new PassLegacy("World Gen Veldris", WorldGenVeizalManor));
            tasks.Insert(CathedralGen2 + 6, new PassLegacy("World Gen Underworld rework", WorldGenUnderworldSpice));
            tasks.Insert(CathedralGen2 + 7, new PassLegacy("World Gen Xix Village", WorldGenXixVillage));
            tasks.Insert(CathedralGen2 + 8, new PassLegacy("World Gen Stone Golem Cave", WorldGenStoneGolemCave));

            tasks.Insert(CathedralGen2 + 9, new PassLegacy("World Gen Windmills Village", WorldGenWindmills));
            tasks.Insert(CathedralGen2 + 10, new PassLegacy("World Gen Rysa House", WorldGenRysaHouse));
            tasks.Insert(CathedralGen2 + 11, new PassLegacy("World Gen Manor", WorldGenManor));
            tasks.Insert(CathedralGen2 + 12, new PassLegacy("World Gen Gia's House", WorldGenGiaHouse));
            // tasks.Insert(CathedralGen2 + 13, new PassLegacy("World Gen Worshiping Towers", WorldGenWorshipingTowers));
            tasks.Insert(CathedralGen2 + 13, new PassLegacy("World Gen Bridget", WorldGenFabledTrees));
            //      tasks.Insert(CathedralGen2 + 14, new PassLegacy("World Gen Blood Catherdal", WorldGenBloodCathedral));
            tasks.Insert(CathedralGen2 + 14, new PassLegacy("World Gen Ashoti Temple", WorldGenAshotiTemple));
            tasks.Insert(CathedralGen2 + 15, new PassLegacy("World Gen Dock", WorldGenDock));
            tasks.Insert(CathedralGen2 + 16, new PassLegacy("World Gen Evil", WorldGenEvil));
            tasks.Insert(CathedralGen2 + 17, new PassLegacy("World Gen Colosseum", WorldGenColosseum));
            tasks.Insert(CathedralGen2 + 18, new PassLegacy("Grassing Caves", WorldGenGrassPass));
            tasks.Insert(CathedralGen2 + 19, new PassLegacy("World Gen Skullrunner", WorldGenSkullrunner));
            tasks.Insert(CathedralGen2 + 20, new PassLegacy("World Gen Fable", WorldGenFabiliaRuin));
            //     tasks.Insert(CathedralGen2 + 22, new PassLegacy("World Gen Water", WorldGenWater));
        }
    }*/

    private void ForceCrimson(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Nothing Lol";
        WorldGen.WorldGenParam_Evil = 1;
        WorldGen.crimson = true;
    }

    private void CindersparkCavesPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Making Cinderspark Caves";
        var genRand = WorldGen.genRand;


        //Here we're going to use the same technique i used in the darkspace
        FastNoiseLite topFNL = new FastNoiseLite();
        topFNL.SetSeed(genRand.Next(0, int.MaxValue));
        topFNL.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        topFNL.SetFrequency(0.15f);
        topFNL.SetDomainWarpAmp(10);
        topFNL.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);

        FastNoiseLite bottomFNL = new FastNoiseLite();
        bottomFNL.SetSeed(genRand.Next(0, int.MaxValue));
        bottomFNL.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        bottomFNL.SetFrequency(0.15f);
        bottomFNL.SetDomainWarpAmp(10);
        bottomFNL.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);

        //The cinderspark is defined by long narrow passage ways
        //So just pick a random point, decide to go left and right, and go from there
        //Then sprinkle vertical caves so you can actually move down in the place
        float numCaves = Main.maxTilesX * Main.maxTilesY * 0.000004f;
        for (float f = 0; f < numCaves; f++)
        {
            //Reset the seed for each cave
            topFNL.SetSeed(genRand.Next(0, int.MaxValue));
            bottomFNL.SetSeed(genRand.Next(0, int.MaxValue));

            int sx = genRand.Next(0, Main.maxTilesX);
            int sy = genRand.Next(CindersparkStart, Main.UnderworldLayer);
            int minCaveDistance = genRand.Next(4, 5);
            int maxCaveDistance = genRand.Next(8, 10);
            int steps = genRand.Next(128, 900);
            int dir = genRand.NextBool(2) ? 1 : -1;
            for (int s = 0; s < steps; s++)
            {
                float SampleNoise(int x, int y)
                {
                    return topFNL.GetNoise(x * 0.05f, y * 0.05f) * 0.5f + 0.5f;
                }
                float SampleNoise2(int x, int y)
                {
                    return bottomFNL.GetNoise(x * 0.05f, y * 0.05f) * 0.5f + 0.5f;
                }

                int x = sx + s * dir;
                if (x < 0 || x >= Main.maxTilesX)
                    break;

                float topNoise = SampleNoise(x, sy);
                float bottomNoise = SampleNoise2(x, sy);

                //Cave middle up
                int topDistance = (int)MathHelper.Lerp(minCaveDistance, maxCaveDistance, topNoise) + genRand.Next(-1, 1);
                for (int y = 0; y < topDistance; y++)
                {
                    Tile tile = Main.tile[x, sy - y];
                    tile.ClearEverything();
                }

                //Cave middle down
                int bottomDistance = (int)MathHelper.Lerp(minCaveDistance, maxCaveDistance, bottomNoise) + genRand.Next(-1, 1);
                for (int y = 0; y < bottomDistance; y++)
                {
                    Tile tile = Main.tile[x, sy + y];
                    tile.ClearEverything();
                }
            }
        }


        //Vertical Caves
        for (float f = 0; f < numCaves; f++)
        {
            //Reset the seed for each cave
            topFNL.SetSeed(genRand.Next(0, int.MaxValue));
            bottomFNL.SetSeed(genRand.Next(0, int.MaxValue));

            int sx = genRand.Next(0, Main.maxTilesX);
            int sy = genRand.Next(CindersparkStart, Main.UnderworldLayer);
            Tile startTile = Main.tile[sx, sy];

            //Only place on air, guaranteeing that the cave connects to another cave
            if (startTile.HasTile)
                continue;

            int minCaveDistance = genRand.Next(3, 4);
            int maxCaveDistance = genRand.Next(6, 8);
            int steps = genRand.Next(32, 100);
            for (int s = 0; s < steps; s++)
            {
                float SampleNoise(int x, int y)
                {
                    return topFNL.GetNoise(x * 0.05f, y * 0.05f) * 0.5f + 0.5f;
                }
                float SampleNoise2(int x, int y)
                {
                    return bottomFNL.GetNoise(x * 0.05f, y * 0.05f) * 0.5f + 0.5f;
                }

                int y = sy + s;
                if (y <= 0 || y >= Main.maxTilesY)
                    break;

                float topNoise = SampleNoise(sx, y);
                float bottomNoise = SampleNoise2(sx, y);

                //Cave middle up
                int topDistance = (int)MathHelper.Lerp(minCaveDistance, maxCaveDistance, topNoise) + genRand.Next(-1, 1);
                for (int x = 0; x < topDistance; x++)
                {
                    int newX = sx - x;
                    if (newX <= 0)
                        break;

                    Tile tile = Main.tile[newX, y];
                    tile.ClearEverything();
                }

                //Cave middle down
                int bottomDistance = (int)MathHelper.Lerp(minCaveDistance, maxCaveDistance, bottomNoise) + genRand.Next(-1, 1);
                for (int x = 0; x < bottomDistance; x++)
                {
                    int newX = sx + x;
                    if (newX >= Main.maxTilesX)
                        break;
                    Tile tile = Main.tile[newX, y];
                    tile.ClearEverything();
                }
            }
        }

        //Smoothing will get rid of the lonely tiles
        Rectangle smoothingRect = new Rectangle(0, CindersparkStart, Main.maxTilesX, Main.UnderworldLayer - CindersparkStart);
        CellularAutomataParams @params = new CellularAutomataParams() with { Steps = 3, RandomFill = 55, BirthLimit = 4, DeathLimit = 4 };
        VeilGen.AutomataSmoothErase(smoothingRect, in @params);
    }

    private void AddNewGenerationPasses(List<GenPass> tasks, ref double totalWeight)
    {
        PassWriter passWriter = new PassWriter(tasks);
        // passWriter.DisablePass("Grass");

        passWriter.SetInsertionIndex("Ocean Sand");
        passWriter.NextPass(new ReworkedOceanSandPass());

        passWriter.SetInsertionIndex("Beaches");
        passWriter.NextPass(new ReworkedBeachesPass());

        passWriter.SetInsertionIndex("Reset");
        passWriter.NextPass(new PassLegacy("Crimsoning", ForceCrimson));

        passWriter.SetInsertionIndex("Terrain");
        passWriter.NextPass(new VanillaTerrainPass());
        passWriter.NextPass(new PassLegacy("Desert Pyr", InitializePyr));
        passWriter.NextPass(new PassLegacy("Set Xix Village Location", SetXixVillageLocation));
        passWriter.NextPass(new PassLegacy("World Gen GenVar Locations", WorldGenVarLocations));
        passWriter.NextPass(new PassLegacy("World Gen GenVar Locations2", WorldGenSpawnPoint));
        passWriter.NextPass(new PassLegacy("FableTerrain", WorldGenFableTerrain));
        passWriter.NextPass(new PassLegacy("MarshTerrain", WorldGenMarsh));
        passWriter.NextPass(new PassLegacy("Veizal Hill Terrain", WorldGenVeizalHillsTerrain));
        passWriter.NextPass(new PassLegacy("Misty Dungeon Hill Terrain", WorldGenMistyDungeonHill));
        passWriter.NextPass(new PassLegacy("RoyalCapitalTerrain", WorldGenCapitalTerrain));
        passWriter.NextPass(new PassLegacy("World Gen Cinderspark", WorldGenCinderspark));
        passWriter.NextPass(new PassLegacy("Cinderspark Caves", CindersparkCavesPass));
        passWriter.NextPass(new PassLegacy("Tree Caves", TreeCavesPass));


        passWriter.SetInsertionIndex("Shimmer");
        passWriter.NextPass(new PassLegacy("Fake Shimmer", WorldGenShimmerSpot));

        passWriter.SetInsertionIndex("Planting Trees");
        passWriter.NextPass(new PassLegacy("MarshTrees", WorldGenMarshTrees));

        passWriter.SetInsertionIndex("Micro Biomes");
        passWriter.DisablePass("Micro Biomes");
        passWriter.NextPass(new PassLegacy("World Gen Worlds End", WorldGenWorldsEnd));
        passWriter.NextPass(new PassLegacy("World Gen Ice Ores", WorldGenGlisteningOre));
        passWriter.NextPass(new PassLegacy("World Gen Flame Ores", WorldGenFlameOre));
        passWriter.NextPass(new PassLegacy("World Gen Dragon Ores", WorldGenDragonpieceOre));
        passWriter.NextPass(new PassLegacy("World Gen Illuria", WorldGenIlluria));
        passWriter.NextPass(new PassLegacy("World Gen Ice Ores", WorldGenFrileOre));
        passWriter.NextPass(new PassLegacy("World Gen Royal Castle", WorldGenRoyalCapital));
        passWriter.NextPass(new PassLegacy("World Gen Hills and Veizal House", WorldGenHillsAndVeizal));

        passWriter.NextPass(new PassLegacy("HillsnFable", WorldGenFabiliaRuin));
        passWriter.NextPass(new PassLegacy("World Gen Rysa House", WorldGenRysaHouse));
        passWriter.NextPass(new PassLegacy("MistyDungeon", GenerateMistyDungeon));

        passWriter.NextPass(new PassLegacy("Marsh Housing", WorldGenMarshHousing));
        passWriter.NextPass(new PassLegacy("Aegislav", WorldGen_AegislavFull));
        passWriter.NextPass(new PassLegacy("Water Wobble Cave", WorldGen_WaterWobbleCave));
        passWriter.NextPass(new PassLegacy("Craftsman Cave", WorldGen_CraftsMenCaves));
        passWriter.NextPass(new PassLegacy("Treasure Trove", WorldGen_TreasureTrove));
        passWriter.NextPass(new PassLegacy("Moonspiral Tower", WorldGen_MoonspiralTower));

        passWriter.SetInsertionIndex("Generate Ice Biome");

        passWriter.NextPass(new ReworkedVanillaIceBiomePass());
        passWriter.NextPass(new PassLegacy("Ice Clumping", IceClump));
        passWriter.NextPass(new PassLegacy("Ice Spikes", MakingIcyRandomness));
        passWriter.NextPass(new PassLegacy("World Gen Abysm", WorldGenAbysm));
        passWriter.NextPass(new PassLegacy("World Gen Abysm Caves", NewCaveFormationAbysm));
        passWriter.NextPass(new PassLegacy("Icey Caverns", WorldGenIceCaverns));
        passWriter.NextPass(new PassLegacy("Ice Housing 3", SurfaceIceHouses));


        passWriter.SetInsertionIndex("Jungle");
        passWriter.NextPass(new MarshJungleMudPass());
        passWriter.NextPass(new PassLegacy("Jungle Surface Caves", WorldGenJungleSurfaceCaves));
        passWriter.NextPass(new PassLegacy("Wonderous Darkspace", WorldGenDarkspace));
        passWriter.NextPass(new PassLegacy("Charred Stones", HardRocksPass));
        passWriter.NextPass(new PassLegacy("Ravine Caves", RavinesPass));
        passWriter.NextPass(new PassLegacy("Deep Caves", DeepCavesPass));
        passWriter.NextPass(new PassLegacy("Cavernous Caves", MineshaftsPass));
        passWriter.NextPass(new PassLegacy("Vanilla Caves", ExtraCavesPass));
        passWriter.NextPass(new PassLegacy("Cavern Waters", CavernWaters));
        passWriter.NextPass(new PassLegacy("Black Stones", WorldGenDarkstone));

        //Set desert location
        passWriter.SetInsertionIndex("Full Desert");
        passWriter.ReplacePass(new PassLegacy("Full Desert Rework", LockDesert));

        //Final Structures and Whatnot
        passWriter.SetInsertionIndex("Final Cleanup");
        passWriter.NextPass(new PassLegacy("Shimmer Fix", ReplaceLavaWithShimmerPass));
        passWriter.NextPass(new PassLegacy("Runica Waterside Underwater", WorldGenRunicaUnderwaterCaves));
        passWriter.NextPass(new PassLegacy("Junkyard Caves", WorldGenJunkyardCaves));
        passWriter.NextPass(new PassLegacy("World Gen Manor", WorldGenManor));
        passWriter.NextPass(new PassLegacy("World Gen Skullrunner", WorldGenSkullrunner));
        passWriter.NextPass(new PassLegacy("World Gen Dock", WorldGenDock));
        //   passWriter.NextPass(new PassLegacy("World Gen Evil", WorldGenEvil));
        passWriter.NextPass(new PassLegacy("World Gen Ashoti Temple", WorldGenAshotiTemple));
        passWriter.NextPass(new PassLegacy("World Gen AureTemple", WorldGenAurelusTemple));
        passWriter.NextPass(new PassLegacy("World Gen Windmills Village", WorldGenWindmills));
        passWriter.NextPass(new PassLegacy("World Gen Colosseum", WorldGenColosseum));
        passWriter.NextPass(new PassLegacy("World Gen Xix Village", WorldGenXixVillage));
        passWriter.NextPass(new PassLegacy("World Gen Stone Golem Cave", WorldGenStoneGolemCave));
        passWriter.NextPass(new PassLegacy("Charred Stone Walls", HardWallsPass));
        passWriter.NextPass(new PassLegacy("Grassing Caves", WorldGenGrassPass));
    }

    private void WorldGen_MoonspiralTower(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Moonspiral Tower";
        Point snowCenter = SnowClumpOriginPoint;

        string structurePath = $"Structures/MoonspiralTower";
        Rectangle structureRect = Structurizer.ReadRectangle(structurePath);
        snowCenter.X -= structureRect.Width / 2;
        snowCenter.Y -= 120;
        snowCenter.Y += 8;
        Structurizer.ReadStruct(snowCenter, structurePath, Structurizer.DefaultTileBlend);
    }

    private void WorldGen_TreasureTrove(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Treasure Trove";
        Point caveOrigin = AbyssCenter;

        caveOrigin.Y -= 800;
        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab("TreasureTrove");
        Rectangle bounds = prefab.GetBounds(caveOrigin.X, caveOrigin.Y, PrefabPlacementType.FromTopCenter);

        //Fill up area with random tiles fr
        for (int x = bounds.Left; x < bounds.Right; x++)
        {
            for (int y = bounds.Top; y < bounds.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!Main.rand.NextBool(16))
                    continue;

                int randTile = Main.rand.Next(3);
                int tileToPlace = TileID.SnowBlock;
                WorldGen.TileRunner(x, y, 16, 32, tileToPlace, addTile: true, 1, 1);
            }
        }

        prefab.PasteErase(caveOrigin, PrefabPlacementType.FromTopCenter);
    }

    private void WorldGen_CraftsMenCaves(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Craftsman Tunnels";
        Point caveOrigin = RoyalCapitalLocation;
        caveOrigin.X -= 310;
        caveOrigin.Y += 100;
        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab("CraftsmanTunnels");
        prefab.PasteErase(caveOrigin, PrefabPlacementType.FromTopCenter);
    }

    private void WorldGen_WaterWobbleCave(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Water Wobble Cave";
        Point caveOrigin = CoralwaysLocation;
        caveOrigin.X += 60;
        caveOrigin.Y += 334;
        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab("WaterWobbleCave");
        Rectangle bounds = prefab.GetBounds(caveOrigin.X, caveOrigin.Y, PrefabPlacementType.FromTopRight);


        int deepSeaTile = ModContent.TileType<DeepSeaTile>();
        int pinkSandTile = ModContent.TileType<PinkSandTile>();
        int reefTile = ModContent.TileType<ReefTile>();
        int[] tiles = new int[]
        {
            deepSeaTile,
            pinkSandTile,
            reefTile
        };
        for (int x = bounds.Left; x < bounds.Right; x++)
        {
            for (int y = bounds.Top; y < bounds.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                ModContent.GetInstance<ZTileMap>().KillAnyTile(new Point(x, y));
            }
        }


        //Fill up area with random tiles fr
        for (int x = bounds.Left; x < bounds.Right; x++)
        {
            for (int y = bounds.Top; y < bounds.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!Main.rand.NextBool(16))
                    continue;

                int randTile = Main.rand.Next(3);
                int tileToPlace = tiles[randTile];
                WorldGen.TileRunner(x, y, 16, 32, tileToPlace, addTile: true, 1, 1);
            }
        }

        prefab.PasteErase(caveOrigin, PrefabPlacementType.FromTopRight);
    }

    private void WorldGen_AegislavFull(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Creating an Evil Place...";
        Point startTile = MistyHillEndLocation;
        startTile.X -= 50;
        startTile.Y -= 300;
        startTile = FallToSolidTile(startTile);


        Point endTile = startTile;
        endTile.X += 850;
        endTile.Y -= 500;
        endTile = FallToSolidTile(endTile);


        int sandTile = ModContent.TileType<AegislavSandTile>();
        float minDepth = 45;
        float maxDepth = 200;
        int[] heights = new int[endTile.X - startTile.X];
        int length = endTile.X - startTile.X;
        int startHeight = (int)Main.worldSurface - 500;

        //Place all the sand
        for (int x = startTile.X; x < endTile.X; x++)
        {
            int localX = x - startTile.X;
            float ratio = localX / (float)length;
            float bump = EasingFunction.QuadraticBump(ratio);
            float depthAtPosition = MathHelper.Lerp(minDepth, maxDepth, bump);

            Point point = new Point(x, startHeight);
            point = FallToSolidTile(point);
            heights[localX] = point.Y;


            //Clear every tile above the ground
            for (int d = 0; d < 50; d++)
            {
                Main.tile[x, point.Y - (1 + d)].ClearEverything();
            }
            for (int depthY = 0; depthY < depthAtPosition; depthY++)
            {
                Point tileToPlaceAt = new Point(x, point.Y + depthY);
                tileToPlaceAt.Y -= 2;
                if (!Main.tile[tileToPlaceAt].HasTile)
                    continue;
                WorldGen.PlaceTile(tileToPlaceAt.X, tileToPlaceAt.Y, sandTile, mute: true, forced: true);
            }
        }


        Point evilPoint = startTile;
        evilPoint.X = (int)MathHelper.Lerp(evilPoint.X, endTile.X, 0.5f);
        evilPoint = FallToSolidTile(evilPoint);
        evilPoint.Y += 250;
        WorldGen_EvilCircle(evilPoint);
        ushort uGrassTileType = (ushort)sandTile;
        var genRand = WorldGen.genRand;
        //Generate big trees, mangrove trees
        for (int x = startTile.X; x < endTile.X; x++)
        {
            float localX = x - startTile.X;
            float ratio = localX / length;
            int heightIndex = x - startTile.X;
            int height = heights[heightIndex];

            int y = height;
            Tile tile = Main.tile[x, y];

            Rectangle scanArea = new Rectangle(x, y, 5, 2);
            Point point = new Point(x - scanArea.Width / 2, y);
            Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
            WorldUtils.Gen(point, new Shapes.Rectangle(scanArea.Width, scanArea.Height),
                new Actions.TileScanner(uGrassTileType).Output(dictionary));
            int tileCount = dictionary[uGrassTileType];

            if (tileCount >= 5)
            {
                if (genRand.NextBool(32))
                {
                    int treeHeight = genRand.Next(20, 48);
                    VeilGen.PlaceBigTrees<BigDeadTree, BigDeadTreeTop>(x, y, treeHeight);
                }
            }
        }

        //Now we're going to place acacia trees
        ushort bigTreeTileType = (ushort)ModContent.TileType<BigDeadTree>();
        for (int x = startTile.X; x < endTile.X; x++)
        {
            float localX = x - startTile.X;
            float ratio = localX / length;
            int heightIndex = x - startTile.X;
            int height = heights[heightIndex];

            int y = height;
            Tile tile = Main.tile[x, y];

            Rectangle scanArea = new Rectangle(x, y, 5, 2);
            Point point = new Point(x - scanArea.Width / 2, y);
            Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
            WorldUtils.Gen(point, new Shapes.Rectangle(scanArea.Width, scanArea.Height),
                new Actions.TileScanner(uGrassTileType, bigTreeTileType).Output(dictionary));
            int tileCount = dictionary[uGrassTileType];
            int mangroveTreeCount = dictionary[bigTreeTileType];

            if (tileCount >= 5 && mangroveTreeCount <= 0)
            {
                if (genRand.NextBool(8))
                {
                    int treeHeight = genRand.Next(6, 20);
                    VeilGen.PlaceTrees<DeadTree, DeadTreeTop>(x, y, treeHeight);
                }
            }
        }

        Point aegislavCastlePoint = new Point();
        aegislavCastlePoint = endTile;
        aegislavCastlePoint.X -= 300;
        aegislavCastlePoint.Y -= 20;
        aegislavCastlePoint = FallToSolidTile(aegislavCastlePoint.X, aegislavCastlePoint.Y);

        string path = "Structures/BloodletCastle";
        aegislavCastlePoint.Y += 15;
        Structurizer.ReadStruct(aegislavCastlePoint, path, Structurizer.DefaultTileBlend);
        Structurizer.ProtectStructure(aegislavCastlePoint, path);
    }

    private void WorldGenMarshHousing(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Placing Marshy Outposts";
        PatternManager<int> houses = new PatternManager<int>(
            new Tuple<int, float>(0, 1.0f),
            new Tuple<int, float>(1, 1.0f),
            new Tuple<int, float>(2, 1.0f),
            new Tuple<int, float>(3, 1.0f));

        string GetStruturePath(int index)
        {
            return $"Structures/MarshOutpost{index + 1}";
        }

        //Place ravager first
        string ravagerArena = "Structures/RavagerArena";
        Point ravagerPlacementPoint = MarshLocation;
        ravagerPlacementPoint.X += 550;
        ravagerPlacementPoint.Y -= 500;
        ravagerPlacementPoint = FallToSolidTile(ravagerPlacementPoint);
        Structurizer.ProtectStructure(ravagerPlacementPoint, ravagerArena);

        int[] tileBlend = new int[]
{
            TileID.RubyGemspark
};
        Structurizer.ReadStruct(ravagerPlacementPoint, ravagerArena, tileBlend);

        int numHouses = 5;
        for (int i = 0; i < numHouses; i++)
        {
            int houseIndex = houses.NextPattern();
            string structure = GetStruturePath(houseIndex);

            for (int a = 0; a < 100000; a++)
            {
                Point houseFallingPoint = MarshLocation;
                houseFallingPoint.Y -= 1000;

                int dir = Main.rand.NextBool(2) ? 1 : -1;

                //Need to avoid the center point
                houseFallingPoint.X = GenVars.jungleOriginX + Main.rand.Next(200, 500) * dir;
                houseFallingPoint = FallToSolidTile(houseFallingPoint);

                if (!Structurizer.TryPlaceAndProtectStructure(houseFallingPoint, structure))
                    continue;
                int[] chestIndices = Structurizer.ReadStruct(houseFallingPoint, structure, tileBlend);
                Rectangle structureRectangle = Structurizer.ReadRectangle(structure);
                structureRectangle.Location = houseFallingPoint;
                for (int beamX = structureRectangle.Location.X;
                    beamX < structureRectangle.Location.X + structureRectangle.Width; beamX += 4)
                {
                    //Place beams
                    int beamY = structureRectangle.Location.Y;
                    Tile tile = Main.tile[beamX, beamY];
                    int solidCount = 0;
                    while (solidCount < 5)
                    {
                        if (!WorldGen.SolidTile(beamX, beamY))
                        {
                            WorldGen.PlaceTile(beamX, beamY, TileID.BorealBeam);
                        }
                        else
                        {
                            solidCount++;
                        }
                        beamY++;
                    }
                }
                break;
            }
        }
    }

    private void WorldGenRunicaUnderwaterCaves(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Runica Caves";
        var rand = WorldGen.genRand;
        int caveOriginX = GenVars.snowOriginRight + 250;
        int caveOriginY = (int)Main.worldSurface - 100;

        Point point = new Point(caveOriginX, caveOriginY);
        caveOriginY = FallToSolidTile(point).Y;

        int width = 500;
        int left = caveOriginX - width / 2;
        int right = caveOriginX + width / 2;
        int bottom = caveOriginY + 1800;

        int deepSeaTile = ModContent.TileType<DeepSeaTile>();
        int pinkSandTile = ModContent.TileType<PinkSandTile>();
        int reefTile = ModContent.TileType<ReefTile>();


        void ScatterBlotch(int numBlotches, int t)
        {
            int attempts = 0;
            int n = 0;
            while (n < numBlotches)
            {
                attempts++;
                if (attempts > 1000000)
                {
                    Console.WriteLine("Failed to generate enough blotches");
                    break;
                }
                int randY = rand.Next(caveOriginY, bottom);
                int randX = rand.Next(left, right);
                if (randX >= Main.maxTilesX)
                    continue;

                Tile tile = Main.tile[randX, randY];
                if (!tile.HasTile)
                    continue;
                if (tile.TileType != deepSeaTile)
                    continue;

                //We have a spot
                float strength = rand.NextFloat(4, 8);
                int steps = rand.Next(5, 10);
                WorldGen.OreRunner(randX, randY, strength, steps, (ushort)t);
                n++;
            }
        }
        void ScatterBlotchEdges(int numBlotches, int t)
        {
            int attempts = 0;
            int n = 0;
            while (n < numBlotches)
            {
                attempts++;
                if (attempts > 1000000)
                {
                    Console.WriteLine("Failed to generate enough blotches");
                    break;
                }
                int randY = rand.Next(caveOriginY, bottom);
                int randX = rand.Next(left, right);
                if (randX >= Main.maxTilesX)
                    continue;

                Tile tile = Main.tile[randX, randY];
                if (!tile.HasTile)
                    continue;
                if (tile.TileType != reefTile)
                    continue;
                if (!WorldGen.TileIsExposedToAir(randX, randY))
                    continue;
                //We have a spot
                float strength = rand.NextFloat(8, 16);
                int steps = rand.Next(10, 20);
                WorldGen.OreRunner(randX, randY, strength, steps, (ushort)t);
                n++;
            }
        }

        void ScatterBlotchWallEdges(int numBlotches, params ushort[] wallIDs)
        {
            int attempts = 0;
            int n = 0;
            while (n < numBlotches)
            {
                attempts++;
                if (attempts > 1000000)
                {
                    Console.WriteLine("Failed to generate enough blotches");
                    break;
                }
                int randY = rand.Next(caveOriginY, bottom);
                int randX = rand.Next(left, right);
                if (randX >= Main.maxTilesX)
                    continue;

                Tile tile = Main.tile[randX, randY];
                if (!tile.HasTile)
                    continue;
                if (!WorldGen.TileIsExposedToAir(randX, randY))
                    continue;

                Point point = new Point(randX, randY);
                int steps = rand.Next(1, 4);
                Vector2 baseDirection = -Vector2.UnitY;
                int caveWidth = 3;

                byte paint = PaintID.TealPaint;
                switch (rand.Next(4))
                {
                    case 0:
                        break;
                    case 1:
                        paint = PaintID.SkyBluePaint;
                        break;
                    case 2:
                        paint = PaintID.PinkPaint;
                        break;
                    case 3:
                        paint = PaintID.RedPaint;
                        break;
                }
                for (int s = 0; s < steps; s++)
                {
                    if (point.X - caveWidth > 0 && point.X + caveWidth < Main.maxTilesX && point.Y + caveWidth < Main.maxTilesY && point.Y - caveWidth > 0)
                    {
                        ushort wallId = wallIDs[rand.Next(wallIDs.Length)];
                        WorldUtils.Gen(point, new Shapes.Circle(caveWidth, caveWidth),
                            Actions.Chain(
                                new Actions.PlaceWall(wallId),
                                new PaintWall(paint)));
                    }

                    point += (baseDirection * caveWidth).RotatedByRandom(MathHelper.ToRadians(30)).ToPoint();
                }
                n++;
            }
        }

        for (int y = caveOriginY; y < bottom; y++)
        {

            for (int x = left; x < right && x < Main.maxTilesX; x++)
            {
                float ratio = (x - left) / (float)(right - left);
                float ease = EasingFunction.QuadraticBump(ratio);

                if (ease < 0.5f)
                {
                    int denom = (int)MathHelper.Lerp(1, 8, ease);
                    if (Main.rand.NextBool(denom))
                        continue;
                }

                if (caveOriginY > bottom - 25)
                {
                    float heightRatio = (caveOriginY - (bottom - 25)) / 25f;
                    int heightDenom = (int)MathHelper.Lerp(1, 16, heightRatio);
                    if (!Main.rand.NextBool(heightDenom))
                        continue;
                }
                Tile tile = Main.tile[x, y];
                if (tile.HasTile)
                {
                    int tileToPlace = deepSeaTile;
                    if (y > bottom - 400)
                        tileToPlace = ModContent.TileType<SeavathanBrick>();
                    WorldGen.PlaceTile(x, y, tileToPlace, forced: true);
                }
            }
        }

        CoralwaysLocation = new Point(caveOriginX - 150, caveOriginY);
        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab("HarmonicCoralways");
        prefab.PasteErase(caveOriginX, caveOriginY, PrefabPlacementType.FromTopCenter);

        //Set random reef blocks
        ScatterBlotchEdges(200, TileID.ShellPile);
        ScatterBlotch(3500, pinkSandTile);
        ScatterBlotch(3500, reefTile);
        ScatterBlotch(500, TileID.ReefBlock);
        ScatterBlotch(1500, TileID.Coralstone);
        ScatterBlotchWallEdges(15000, WallID.PoopWall, WallID.PoopWall, WallID.PoopWall, WallID.HardenedSandEcho, WallID.SandstoneEcho);


        ZTileMap tileMap = ModContent.GetInstance<ZTileMap>();
        var items = new ZTile[]
        {
            ModContent.GetInstance<RedCoralMedium>(),
            ModContent.GetInstance<BlueCoralLarge>(),
            ModContent.GetInstance<PinkCoralLarge>()
        };


        for (int y = caveOriginY; y < bottom; y++)
        {

            for (int x = left; x < right && x < Main.maxTilesX; x++)
            {
                if (!WorldGen.TileIsExposedToAir(x, y))
                    continue;
                Tile mainTile = Main.tile[x, y];
                if (!mainTile.HasTile)
                    continue;

                if (!rand.NextBool(7))
                    continue;

                ZTile tile = items[rand.Next(items.Length)];
                var templateData = ModContent.GetInstance<ZTileLoader>().InstanceTileData(tile);
                DecorationBuilder.frame = 0;

                ZTileInstanceData instanceData = templateData;
                instanceData.scale = 1;
                instanceData.rotation = 0;
                instanceData.frameNumber = 0;
                instanceData.flipX = false;
                instanceData.value = 0;

                Vector2 position = new Point(x, y + 1).ToWorldCoordinates();
                tileMap.CreateTile(ZRenderLayer.InFrontOfWalls, position, 0, instanceData);
            }
        }

        for (int y = caveOriginY; y < bottom; y++)
        {
            for (int x = left; x < right; x++)
            {
                WorldGen.PlaceLiquid(x, y, (byte)LiquidID.Water, byte.MaxValue);
            }
        }


        for (int y = caveOriginY; y < bottom; y++)
        {
            for (int x = left; x < right; x++)
            {
                WorldGen.PlaceLiquid(x, y, (byte)LiquidID.Water, byte.MaxValue);
            }
        }

        //Just throw a big ass circle of water at the top to fill the empty space
        Point centerPoint = new Point(caveOriginX, caveOriginY);
        WorldUtils.Gen(centerPoint, new Shapes.Circle(10, 10),
            Actions.Chain(new GenAction[]
        {
            new Actions.SetLiquid(LiquidID.Water)
        }));
    }

    private void WorldGenJunkyardCaves(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Junkyard Caves";
        int caveOriginX = MarshLocation.X;
        caveOriginX -= 350;

        int caveOriginY = MarshLocation.Y;
        caveOriginY -= 35;

        int width = 500;

        int left = caveOriginX - width / 2;
        int right = caveOriginX + width / 2;
        int bottom = caveOriginY + 1800;
        int tileType = ModContent.TileType<JunkyTile>();
        for (int y = caveOriginY; y < bottom; y++)
        {

            for (int x = left; x < right; x++)
            {
                float ratio = (x - left) / (float)(right - left);
                float ease = EasingFunction.QuadraticBump(ratio);
                int denom = (int)MathHelper.Lerp(1, 8, ease);
                if (ease < 0.5f)
                {
                    if (Main.rand.NextBool(denom))
                        continue;
                }

                if (caveOriginY > bottom - 25)
                {
                    float heightRatio = (caveOriginY - (bottom - 25)) / 25f;
                    int heightDenom = (int)MathHelper.Lerp(1, 16, heightRatio);
                    if (!Main.rand.NextBool(heightDenom))
                        continue;
                }
                Tile tile = Main.tile[x, y];
                if (tile.HasTile)
                {
                    WorldGen.PlaceTile(x, y, tileType, forced: true);
                }
            }
        }

        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab("Junkyard");
        prefab.PasteErase(caveOriginX, caveOriginY, PrefabPlacementType.FromTopCenter);
    }

    private void WorldGenJungleSurfaceCaves(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Jungle Surface Caves";
        int caveOriginX = GenVars.jungleOriginX;
        int caveOriginY = MarshLocation.Y;
        caveOriginY -= 35;
        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab("JungleTop");
        prefab.PasteErase(caveOriginX, caveOriginY, PrefabPlacementType.FromTopCenter);
    }
    private void WorldGenCapitalTerrain(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Royal Capital Dirt";

        Point capitalSpot = new Point(666, 1000);
        capitalSpot = FallToSolidTile(capitalSpot);
        RoyalCapitalLocation = capitalSpot;
        WorldGen.TileRunner(capitalSpot.X + 260, capitalSpot.Y + 10, 350, 2, ModContent.TileType<Tiles.StarbloomDirt>(), true, 0f, 0f, true, false);
        WorldGen.TileRunner(capitalSpot.X + 260, capitalSpot.Y + 100, 550, 2, ModContent.TileType<Tiles.StarbloomDirt>(), true, 0f, 0f, true, true);
        WorldGen.TileRunner(capitalSpot.X + 260, capitalSpot.Y + 250, 350, 2, ModContent.TileType<Tiles.StarbloomDirt>(), true, 0f, 0f, true, true);
        WorldGen.TileRunner(capitalSpot.X + 260, capitalSpot.Y + 400, 550, 2, ModContent.TileType<Tiles.StarbloomDirt>(), true, 0f, 0f, true, true);
        WorldGen.TileRunner(capitalSpot.X + 260, capitalSpot.Y + 600, 550, 2, ModContent.TileType<Tiles.StarbloomDirt>(), true, 0f, 0f, true, true);


        //int sb = ModContent.TileType<Tiles.StarbloomDirt>();
        // TileID.Sets.CanBeClearedDuringOreRunner[sb] = false;
        //  TileID.Sets.CanBeClearedDuringGeneration[sb] = false;
    }

    private void WorldGenVeizalHillsTerrain(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Veizal Hills Terrain";
        Point startHillTile = MarshLocation;
        startHillTile.X += 1400;
        Point endHillTile = startHillTile;
        endHillTile.X += 900;

        startHillTile.Y -= 90;
        while (WorldGen.InWorld(endHillTile.X, endHillTile.Y) && !WorldGen.SolidTile(endHillTile.X, endHillTile.Y))
        {
            endHillTile.Y++;
        }


        //Move the start tile backwards so it connects to the marsh
        while (WorldGen.InWorld(startHillTile.X, startHillTile.Y) && !WorldGen.SolidTile(endHillTile.X, endHillTile.Y))
        {
            startHillTile.X--;
        }


        Point waterLakeStart = new Point();
        waterLakeStart.X = (int)MathHelper.Lerp(startHillTile.X, endHillTile.X, 0.2f);
        waterLakeStart.Y = (int)(Main.worldSurface - 200);

        Point waterLakeEnd = new Point();
        waterLakeEnd.X = (int)MathHelper.Lerp(startHillTile.X, endHillTile.X, 0.4f);
        waterLakeEnd.Y = (int)(Main.worldSurface - 200);



        //Move a bit more into the hill so it's more cleanly integrated
        startHillTile.X -= 80;

        VeizalHillStartLcoation = startHillTile;
        VeizalHillEndLocation = endHillTile;
        for (int x = startHillTile.X; x < endHillTile.X; x++)
        {
            //Calculate heights, creating a slowly descending slope
            float width = endHillTile.X - startHillTile.X;
            float ratio = (x - startHillTile.X) / width;

            float tileYHeight = MathHelper.Lerp(startHillTile.Y, endHillTile.Y, ratio);

            //Create some signing motions for variance in the terrain
            tileYHeight += MathF.Sin(ratio * 4.0f) * 16;
            tileYHeight += MathF.Sin(ratio * 8.0f + 0.5f) * 2f;
            tileYHeight += MathF.Sin(ratio * 16.0f + 0.75f) * 5;
            int y = (int)tileYHeight;

            while (WorldGen.InWorld(x, y) && !WorldGen.SolidTile(x, y))
            {
                if (!Main.tileSolid[Main.tile[x, y].TileType])
                    WorldGen.KillTile(x, y);
                WorldGen.PlaceTile(x, y, TileID.Dirt);
                y++;
            }
        }
        GenerateBowlLake(waterLakeStart, waterLakeEnd, maxLakeDepth: 65);
    }
    private void WorldGenHillsAndVeizal(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Hills and Veizal's House";

        //Place Veizal Manor
        StructureMap structures = GenVars.structures;
        string structure = "Struct/Overworld/VeizalManor";
        Rectangle rectangle = Structurizer.ReadRectangle(structure);
        progress.Message = "WE'RE RICH!";
        int[] tileBlend = new int[]
        {
            TileID.RubyGemspark
        };
        int maxAttemptCount = 1000;
        for (int a = 0; a < maxAttemptCount; a++)
        {
            // Select a place in the first 6th of the world, avoiding the oceans
            int x = (int)MathHelper.Lerp(VeizalHillStartLcoation.X, VeizalHillEndLocation.X, 0.7f);
            int y = (int)(Main.worldSurface - 500);
            Point tileToPlaceOn = FallToSolidTile(x, y);
            int cathedralY = tileToPlaceOn.Y;

            //Start at 200 tiles above the surface instead of 0, to exclude floating islands
            Point Loc = tileToPlaceOn;
            if (!Structurizer.TryPlaceAndProtectStructure(Loc, structure))
                continue;
            Structurizer.ReadStruct(Loc, structure, tileBlend);
            Rectangle structureRectangle = Structurizer.ReadRectangle(structure);
            structureRectangle.Location = Loc;
            for (int beamX = structureRectangle.Location.X;
                beamX < structureRectangle.Location.X + structureRectangle.Width; beamX += 4)
            {
                //Place beams
                int beamY = structureRectangle.Location.Y;
                Tile tile = Main.tile[beamX, beamY];
                int solidCount = 0;
                while (solidCount < 5)
                {
                    if (!WorldGen.SolidTile(beamX, beamY))
                    {
                        WorldGen.PlaceTile(beamX, beamY, TileID.BorealBeam);
                    }
                    else
                    {
                        solidCount++;
                    }
                    beamY++;
                }
            }
            break;
        }

        /*
        Point startLakeTile = new Point();
        startLakeTile.X = (int)MathHelper.Lerp(startLakeTile.X, endHillTile.Y, 0.2f);
        startLakeTile.Y = startHillTile.Y;
        startLakeTile = FallToSolidTile(startLakeTile.X, startLakeTile.Y);

        Point endLakeTile = startLakeTile;
        endLakeTile.X += */
    }

    private void WorldGenSpawnPoint(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Placing Spawn Point";
        Main.spawnTileX = 16;
        Main.spawnTileY = 16;
    }

    private void WorldGenHeight(GenerationProgress progress, GameConfiguration configuration)
    {
        /*
        progress.Message = "Generating Terrain Heights";



        float maxX = Main.maxTilesX;
        float heightMaxX = HeightMapWidth;
        float widthScaleFactor = maxX / heightMaxX;


        float maxY = Main.maxTilesY;
        float widthMaxX = HeightMapHeight;
        float heightScaleFactor = maxY / widthMaxX;
        int startHeight = 0;
        for (int y = 0; y < Main.maxTilesY; y++)
        {
            Tile t = Main.tile[0, y];
            if (t.HasTile)
            {
                startHeight = y;
                break;
            }
        }

        startHeight -= 1900;
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = 0; y < Main.maxTilesY; y++)
            {
                Tile t = Main.tile[x, y];
                t.ClearEverything();
            }
        }

        var rand = WorldGen.genRand;
        double worldSurface = Main.maxTilesY * 0.95;
        worldSurface *= rand.Next(90, 110) * 0.005;
        double rockLayer = worldSurface + Main.maxTilesY * 0.13;
        rockLayer *= rand.Next(90, 110) * 0.01;

        double worldSurfaceLow = worldSurface;
        double worldSurfaceHigh = worldSurface;
        double rockLayerLow = rockLayer;
        double rockLayerHigh = rockLayer;
        double num9 = Main.maxTilesY * 0.23;

        Main.worldSurface = (int)(worldSurfaceHigh + 25.0);
        Main.rockLayer = rockLayerHigh;
        double num12 = (int)((Main.rockLayer - Main.worldSurface) / 6.0) * 6;
        Main.rockLayer = (int)(Main.worldSurface + num12);
        int num13 = (int)(Main.rockLayer + Main.maxTilesY) / 2 + rand.Next(-100, 20);
        int lavaLine = num13 + rand.Next(50, 80);

        int num14 = 20;
        if (rockLayerLow < worldSurfaceHigh + num14)
        {
            double num15 = (rockLayerLow + worldSurfaceHigh) / 2.0;
            double num16 = Math.Abs(rockLayerLow - worldSurfaceHigh);
            if (num16 < num14)
                num16 = num14;

            rockLayerLow = num15 + num16 / 2.0;
            worldSurfaceHigh = num15 - num16 / 2.0;
        }

        GenVars.rockLayer = rockLayer;
        GenVars.rockLayerHigh = rockLayerHigh;
        GenVars.rockLayerLow = rockLayerLow;
        GenVars.worldSurface = worldSurface;
        GenVars.worldSurfaceHigh = worldSurfaceHigh;
        GenVars.worldSurfaceLow = worldSurfaceLow;
        GenVars.waterLine = num13;
        GenVars.lavaLine = lavaLine;

        for (int x = 0; x < Main.maxTilesX; x++)
        {
            //Sample the height for this tile
            float tileX = x;
            tileX /= widthScaleFactor;

            //Convert to the image space
            int startPixel = (int)tileX;
            float pixelHeight = 0;
            for (int y = 0; y < HeightMapHeight; y++)
            {
                int pixelIndex = startPixel + y * HeightMapWidth;
                Color pixelColor = HeightMapPixels[pixelIndex];
                if (pixelColor.R > 0)
                {
                    pixelHeight = y;
                    //This is the ground
                    break;
                }
            }

            int tileY = (int)(pixelHeight * heightScaleFactor);
            tileY -= 1300;
            for (int y = tileY; y < Main.maxTilesY; y++)
            {
                Tile t = Main.tile[x, y];
                t.ClearEverything();
                t.TileType = TileID.Dirt;
                t.HasTile = true;
            }
        }

        */
    }

    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {

        //Disable Some Passes
        DisableGenTask(tasks, "Terrain");
        DisableGenTask(tasks, "Tunnels");
        DisableGenTask(tasks, "Mount Caves");
        DisableGenTask(tasks, "Surface Caves");
        DisableGenTask(tasks, "Mountain Caves");
        DisableGenTask(tasks, "Generate Ice Biome");
        DisableGenTask(tasks, "Dungeon");
        DisableGenTask(tasks, "Wavy Caves");
        DisableGenTask(tasks, "Living Trees");
        DisableGenTask(tasks, "Dirt Layer Caves");
        DisableGenTask(tasks, "Rock Layer Caves");
        DisableGenTask(tasks, "Small Holes");
        DisableGenTask(tasks, "Corruption");
        DisableGenTask(tasks, "Floating Islands");
        DisableGenTask(tasks, "Shimmer");
        DisableGenTask(tasks, "Jungle Temple");
        DisableGenTask(tasks, "Temple");
        DisableGenTask(tasks, "Lihzahrd Altars");
        DisableGenTask(tasks, "Sand Patches");
        DisableGenTask(tasks, "Dunes");
        DisableGenTask(tasks, "Marble");
        DisableGenTask(tasks, "Granite");
        DisableGenTask(tasks, "Jungle");
        DisableGenTask(tasks, "Wall Variety");
        DisableGenTask(tasks, "Mushroom Patches");
        //  DisableAllGenTasks(tasks);
        //    AddWorldGenTasks(tasks, ref totalWeight);
        AddNewGenerationPasses(tasks, ref totalWeight);
    }

    public void ClearTrees(Rectangle rectangle)
    {
        int startX = rectangle.Location.X;
        int endX = startX + rectangle.Width;
        int startY = rectangle.Location.Y;
        int endY = rectangle.Location.Y + rectangle.Height;

        startX = Math.Clamp(startX, 0, Main.maxTilesX - 1);
        endX = Math.Clamp(endX, 0, Main.maxTilesX - 1);
        startY = Math.Clamp(startY, 0, Main.maxTilesY - 1);
        endY = Math.Clamp(endY, 0, Main.maxTilesY - 1);

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Tile tile = Main.tile[x, y];
                if (TileID.Sets.IsATreeTrunk[tile.TileType])
                {
                    tile.ClearEverything();
                }
            }
        }
    }
    public void ClearLonelyTiles(Rectangle rectangle)
    {
        int startX = rectangle.Location.X;
        int endX = startX + rectangle.Width;
        int startY = rectangle.Location.Y;
        int endY = rectangle.Location.Y + rectangle.Height;

        //Add 1 extra tile of fluff since we're checking adjacent tiles
        startX = Math.Clamp(startX, 1, Main.maxTilesX - 2);
        endX = Math.Clamp(endX, 1, Main.maxTilesX - 2);
        startY = Math.Clamp(startY, 1, Main.maxTilesY - 2);
        endY = Math.Clamp(endY, 1, Main.maxTilesY - 2);

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;

                int adjacentCount = 0;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        //Ignore diagonals
                        if (i != 0 && j != 0)
                            continue;
                        if (i == 0 && j == 0)
                            continue;
                        Tile adjacentTile = Main.tile[x + i, y + j];
                        if (adjacentTile.HasTile && Main.tileSolid[adjacentTile.TileType])
                            adjacentCount++;
                    }
                }

                if (adjacentCount <= 1)
                    tile.ClearEverything();
            }
        }
    }
    private void WorldGenWorldsEnd(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Ending the World";

        int startTileX = 0;
        int endTileX = RoyalCapitalLocation.X;
        int maxDepth = 125;
        int minDepth = 25;
        int grass = ModContent.TileType<WhiteGrass>();

        TileID.Sets.CanBeClearedDuringGeneration[grass] = false;
        TileID.Sets.CanBeClearedDuringOreRunner[grass] = false;

        Rectangle treeRect = new Rectangle(0, RoyalCapitalLocation.Y - 32, RoyalCapitalLocation.X, 500);
        ClearTrees(treeRect);

        //Create a base for all the grass
        for (int tileX = startTileX; tileX < endTileX; tileX++)
        {
            int startY = (int)Main.worldSurface - 100;
            while (!WorldGen.SolidTile(tileX, startY))
                startY++;

            float width = endTileX - startTileX;
            float ratio = (tileX - startTileX) / width;
            int depth = (int)MathHelper.SmoothStep(maxDepth, minDepth, ratio);
            for (int tileY = startY; tileY < startY + depth; tileY++)
            {
                WorldGen.TileRunner(tileX, tileY, 2, 4, grass);
            }
        }


        Point startSlope = RoyalCapitalLocation;
        startSlope.X -= 250;

        int startSlopeY = startSlope.Y;

        for (int tileX = startSlope.X; tileX < endTileX; tileX++)
        {
            float ratio = (tileX - startSlope.X) / (float)(endTileX - startSlope.X);
            float y = MathHelper.SmoothStep(0f, 27, ratio);
            int tileY = (int)(startSlopeY - y);
            for (int innerY = tileY; innerY < startSlopeY; innerY++)
            {
                Tile tile = Main.tile[tileX, innerY];
                tile.ClearTile();
                tile.TileType = (ushort)grass;
                tile.TileFrameX = -1;
                tile.TileFrameY = -1;
                //  WorldGen.PlaceTile(tileX, innerY, grass, forced: true);
            }
        }

        //Generate water bowl
        int maxLakeDepth = 65;
        Point waterStart = new Point();
        waterStart.X = 4;
        waterStart.Y = (int)Main.worldSurface - 100;
        while (!WorldGen.SolidTile(waterStart))
            waterStart.Y++;

        Point waterEnd = new Point();
        waterEnd.X = waterStart.X + 300;
        waterEnd.Y = (int)Main.worldSurface - 100;
        while (!WorldGen.SolidTile(waterEnd))
            waterEnd.Y++;
        for (int lakeX = waterStart.X; lakeX < waterEnd.X; lakeX++)
        {
            float ratio = (lakeX - waterStart.X) / (float)(waterEnd.X - waterStart.X);
            float bump = EasingFunction.QuadraticBump(ratio);
            int depth = (int)MathHelper.Lerp(0, maxLakeDepth, bump);

            int startY = (int)Main.worldSurface - 100;
            while (!WorldGen.SolidTile(lakeX, startY))
                startY++;
            int endY = startY + depth;
            int d = 0;
            for (int lakeY = startY; lakeY < endY; lakeY++)
            {
                Tile tile = Main.tile[lakeX, lakeY];
                tile.ClearEverything();
                d++;
                if (d > 10)
                {

                    WorldGen.PlaceLiquid(lakeX, lakeY, (byte)LiquidID.Water, byte.MaxValue);
                }

            }
        }

        ClearLonelyTiles(treeRect);

    }

    private void GenerateBowlLake(Point waterStart, Point waterEnd, int maxLakeDepth)
    {
        //Generate water bowl
        while (!WorldGen.SolidTile(waterStart))
            waterStart.Y++;

        while (!WorldGen.SolidTile(waterEnd))
            waterEnd.Y++;
        for (int lakeX = waterStart.X; lakeX < waterEnd.X; lakeX++)
        {
            float ratio = (lakeX - waterStart.X) / (float)(waterEnd.X - waterStart.X);
            float bump = EasingFunction.QuadraticBump(ratio);
            int depth = (int)MathHelper.Lerp(0, maxLakeDepth, bump);

            int startY = (int)Main.worldSurface - 100;
            while (!WorldGen.SolidTile(lakeX, startY))
                startY++;
            int endY = startY + depth;
            int d = 0;
            for (int lakeY = startY; lakeY < endY; lakeY++)
            {
                WorldGen.KillTile(lakeX, lakeY);
                WorldGen.KillWall(lakeX, lakeY);
                d++;
                if (d > 10)
                {

                    WorldGen.PlaceLiquid(lakeX, lakeY, (byte)LiquidID.Water, byte.MaxValue);
                }

            }
        }
    }

    private void InitializePyr(GenerationProgress progress, GameConfiguration configuration)
    {
        var genRand = WorldGen.genRand;
        GenVars.PyrX = new int[3];
        GenVars.PyrY = new int[3];
    }

    private void GenerateMistyDungeon(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Mistying the Dungeon";
        Room[] prefabs = DungeonSaveUtility.GetDungeonPrefabs("Dungeon");


        int dungeonLayoutCount = 1;
        string path = $"MistyDungeon_{WorldGen.genRand.Next(dungeonLayoutCount) + 1}";
        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab(path);
        DungeonChart chart = DungeonChart.FromPrefab(prefab);
        Room[] map = Dungeonizer.GenerateFromChart(prefabs, chart, WorldGen.genRand);
        int[] tileBlend = new int[]
        {
            TileID.RubyGemspark
        };
        Point topLeft = Point.Zero;
        Point bottomRight = Point.Zero;
        for (int r = 0; r < map.Length; r++)
        {
            Room room = map[r];
            if (topLeft.X > room.bounds.Left)
                topLeft.X = room.bounds.Left;
            if (topLeft.Y > room.bounds.Top)
                topLeft.Y = room.bounds.Top;

            if (bottomRight.X < room.bounds.Right)
                bottomRight.X = room.bounds.Right;
            if (bottomRight.Y < room.bounds.Bottom)
                bottomRight.Y = room.bounds.Bottom;
        }
        Rectangle rectangle = new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

        //Look for a spot to place it
        //We're placing it on the right side of the world ig
        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 10000000)
        {
            Point point = MistyDungeonLocation;
            Point vectorToOrigin = (point - rectangle.Top().ToPoint());
            rectangle.Location += vectorToOrigin;

            //Just a failsafe
            while (rectangle.Right().X >= Main.maxTilesX)
                rectangle.Location -= new Point(32, 0);

            int width = rectangle.Width;
            width -= 150;
            int height = rectangle.Height;


            //Not generating a large square anymore that looked weird

            /*
            Point rectanglePoint = point;
            rectanglePoint.X -= width / 2;
            rectanglePoint.Y += 30;

            WorldUtils.Gen(rectanglePoint, new Shapes.Rectangle(width, height),
               Actions.Chain(
                    new Actions.ClearTile(),
                    new Actions.ClearWall(),
                    new Actions.SetTile((ushort)ModContent.TileType<MothlightBrick>()))
               );
            */
            //Override dungeon variables
            GenVars.dungeonLocation = point.X;
            GenVars.dungeonX = point.X;
            GenVars.dungeonY = point.Y;

            //Here we need to get the first room and like draw some blocks downward
            Rectangle firstRoomRect = Structurizer.ReadRectangle(map[0].prefab);
            //This hsould give us an outline of bricks, I think


            //The first room is the starting room, we don't want to outline that one
            //So we're just gonna start from index 1 to skip it
            for (int r = 1; r < map.Length; r++)
            {
                Room room = map[r];
                int padding = 80;
                Rectangle roomRectangle = Structurizer.ReadRectangle(room.prefab);
                int outlineWidth = roomRectangle.Width + padding;
                int outlineHeight = roomRectangle.Height + padding;

                //This hsould give us an outline of bricks, I think
                Point topLeftRoom = room.bounds.TopLeft().ToPoint() + new Point(-padding / 2, -padding / 2);
                Point offset = rectangle.Top().ToPoint();
                topLeftRoom.Y -= map[0].bounds.Height;
            
                topLeftRoom += offset;
                WorldUtils.Gen(topLeftRoom, new Shapes.Rectangle(outlineWidth, outlineHeight),
                   Actions.Chain(
                        new Actions.ClearWall(),
                        new Actions.SetTile((ushort)ModContent.TileType<MothlightBrick>()))
                   );
            }

            for (int r = 0; r < map.Length; r++)
            {
                Room room = map[r];
                Point bottomLeft = room.bounds.BottomLeft().ToPoint();
                Point offset = rectangle.Top().ToPoint();

                int tileX = offset.X;
                int tileY = offset.Y;

                bottomLeft.X += tileX;
                bottomLeft.Y += tileY;
                bottomLeft.Y -= map[0].bounds.Height;
                Structurizer.ReadStruct(bottomLeft, room.prefab, tileBlend);
                if (r == 0)
                {
                    Rectangle rect = Structurizer.ReadRectangle(room.prefab);
                    rect.Location = bottomLeft;
                    Point start = bottomLeft;
                    for (int x = start.X; x < start.X + rect.Width; x++)
                    {
                        Point downPoint = new Point(x, start.Y + 1);
                        for (int y = 0; y < 50; y++)
                        {
                            Tile tile = Main.tile[downPoint];
                            //Checking for walls cause we don't wanna break the inside of the dungeon
                            if (tile.WallType == 0)
                            {
                                tile.ClearEverything();
                                tile.TileType = TileID.Dirt;
                                tile.HasTile = true;
                                tile.TileFrameX = -1;
                                tile.TileFrameY = -1;

                            }
                            downPoint.Y++;
                        }
                    }
                }
                Structurizer.ProtectStructure(bottomLeft, room.prefab);
            }
            placed = true;
        }
    }
    private const int Desert_Padding = 200;
    private void LockDesert(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Expanding the Desert";
        GenVars.skipDesertTileCheck = true;
        DesertBiome desertBiome = GenVars.configuration.CreateBiome<DesertBiome>();
        var genRand = WorldGen.genRand;

        int desertOffset = -1200;
        int x = (Main.maxTilesX / 2 + desertOffset);
        DesertLocation = new Point(x, (int)GenVars.worldSurfaceHigh + genRand.Next(25, 75));
        while (!desertBiome.Place(DesertLocation, GenVars.structures))
        {
            x = (Main.maxTilesX / 2 + desertOffset) + genRand.Next(-200, 0);
            DesertLocation = new Point(x, (int)GenVars.worldSurfaceHigh + genRand.Next(25, 75));
        }


        //About to give the desert an extension

        int newDesertLeft = GenVars.desertHiveLeft - Desert_Padding;
        int newDesertRight = GenVars.desertHiveRight + Desert_Padding;

        //Adding surface sands
        //This is our desert extension, we just gonna replcae dirt/stone/clay tiles


        //Actually, it should be safe to just replace solid tiles, the colosseum doesn't exist yet
        int maxDesertDepth = 150;
        float steps = newDesertRight - newDesertLeft;
        for (int dx = newDesertLeft; dx < newDesertRight; dx++)
        {
            float marker = (dx - newDesertLeft);
            float completionRatio = marker / steps;
            float ease = EasingFunction.QuadraticBump(completionRatio);
            int depth = (int)MathHelper.Lerp(1, maxDesertDepth, ease);
            int tileX = dx;
            int startY = (int)(Main.worldSurface - 100);

            //Move down until we hit a solid tile
            for (int k = 0; k < 300; k++)
            {
                if (!WorldGen.SolidTile(dx, startY))
                {
                    startY++;
                }
                else
                {
                    break;
                }
            }

            //Now we have the position we want to start from
            int bottom = startY + depth;
            for (int dy = startY; dy < bottom; dy++)
            {
                if (WorldGen.SolidTile(tileX, dy))
                {
                    WorldGen.PlaceTile(tileX, dy, TileID.Sand);
                }

                WorldGen.TileRunner(tileX, dy, 3, 10, TileID.Sand);
            }
        }
    }

    private void WorldGenMarsh(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Creating the Marsh";
        int marshTileLength = 1400;
        VeilGen.GenerateMarsh(MarshLocation, marshTileLength);
    }
    private void WorldGenMarshTrees(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Planting the Marshy Trees";
        int marshTileLength = 1400;
        VeilGen.GenerateMarshFoliage(MarshLocation, marshTileLength);

        //Place Gothivia Spot
        Point treeTile = MarshLocation + GothiviaSpawnOffset;
        while (!WorldGen.SolidTile(treeTile))
        {
            treeTile.Y++;
        }
        WorldGen.PlaceWall(treeTile.X, treeTile.Y, ModContent.WallType<TheSeededTree>());
    }
    private void WorldGenVarLocations(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Locking Snow Biome Location";
        Point marshSpot = new Point();
        marshSpot.Y = (int)(Main.worldSurface - 2000);
        marshSpot.X = 1850;
        marshSpot = FallToSolidTile(marshSpot.X, marshSpot.Y);
        marshSpot.Y += 25;
        MarshLocation = marshSpot;
        GenVars.jungleOriginX = marshSpot.X + 700;

        //Set snow biome location

        GenVars.snowOriginLeft = WitchTownLocation.X + 4400;
        GenVars.snowOriginRight = GenVars.snowOriginLeft + 1200;

        //Set dungeon and jungle sides
        GenVars.tLeft = GenVars.jungleOriginX;
        GenVars.tRight = GenVars.jungleOriginX + 100;
        GenVars.tTop = Main.maxTilesY / 2;
        GenVars.tBottom = GenVars.tTop + 100;
        GenVars.dungeonSide = 1;

        //Remove the left beach
        GenVars.leftBeachEnd = 100;
        GenVars.shellStartXLeft = 100;
        GenVars.shellStartYLeft = 100;

    }

    private void WorldGenSkullrunner(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Getting dunked on";
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
        while (!placed && attempts++ < 10000000)
        {
            // Select a place in the first 6th of the world, avoiding the oceans
            int smx = ManorLocation.X + WorldGen.genRand.Next(-200, 200);
            smx -= 600;

            int smy = ManorLocation.Y;
            Point Loc = new Point(smx, smy);

            string path = "Structures/Skullrunner";
            int[] ChestIndexs = Structurizer.ReadStruct(Loc, path, tileBlend);
            Structurizer.ProtectStructure(Loc, path);
            placed = true;
        }
    }

    private void WorldGenDarkspace(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Creating a Dark Place.";

        var genRand = WorldGen.genRand;
        int yMax = CindersparkStart - 600;
        if (CindersparkStart == 0)
        {
            throw new ArgumentException("The Cinderspark is at the top of the world for some reason.");
        }

        int yMin = yMax - 250;
        int yMid = (yMin + yMax) / 2;
        int[] wallTypes = new int[]
        {
            WallID.GraniteUnsafe,
            WallID.GraniteBlock,
            WallID.Granite
        };

        DarkspaceStart = yMin;
        DarkspaceEnd = yMax;
        //Create a wavey blotch of granite
        //Instead of using GenActions or PlaceTile we can just set the tile directly, fastest way to do it.
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            int dyMin = yMin + (int)MathF.Sin(x) * 8 + genRand.Next(-2, 2);
            int dyMax = yMax + (int)MathF.Sin(x * 0.05f) * 8 + genRand.Next(-2, 2);
            for (int y = dyMin; y < dyMax; y++)
            {
                Tile tile = Main.tile[x, y];
                tile.ClearTile();
                tile.HasTile = true;
                tile.TileFrameX = -1;
                tile.TileFrameY = -1;
                tile.TileType = TileID.Granite;
            }
        }
        progress.Set(0.33D);

        //Here's the algorithm we're going to try
        //We'll initialize a fast noise lite
        //We'll sample two points, each far from each other
        //then slowly move right and using the noise we create the variation in the caves
        FastNoiseLite topFNL = new FastNoiseLite();
        topFNL.SetSeed(genRand.Next(0, int.MaxValue));
        topFNL.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        topFNL.SetFrequency(0.15f);
        topFNL.SetDomainWarpAmp(10);
        topFNL.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);

        FastNoiseLite bottomFNL = new FastNoiseLite();
        bottomFNL.SetSeed(genRand.Next(0, int.MaxValue));
        bottomFNL.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        bottomFNL.SetFrequency(0.15f);
        bottomFNL.SetDomainWarpAmp(10);
        bottomFNL.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);

        int minCaveDistance = 35;
        int maxCaveDistance = 72;
        (int, int)[] heights = new (int, int)[Main.maxTilesX];
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            float SampleNoise(int x, int y)
            {
                return topFNL.GetNoise(x * 0.05f, y * 0.05f) * 0.5f + 0.5f;
            }
            float SampleNoise2(int x, int y)
            {
                return bottomFNL.GetNoise(x * 0.05f, y * 0.05f) * 0.5f + 0.5f;
            }
            float topNoise = SampleNoise(x, yMid);
            float bottomNoise = SampleNoise2(x, yMid);

            //Cave middle up
            int topDistance = (int)MathHelper.Lerp(minCaveDistance, maxCaveDistance, topNoise) + genRand.Next(-1, 1);
            for (int y = 0; y < topDistance; y++)
            {
                Tile tile = Main.tile[x, yMid - y];
                tile.ClearEverything();
            }

            //Cave middle down
            int bottomDistance = (int)MathHelper.Lerp(minCaveDistance, maxCaveDistance, bottomNoise) + genRand.Next(-1, 1);
            for (int y = 0; y < bottomDistance; y++)
            {
                Tile tile = Main.tile[x, yMid + y];
                tile.ClearEverything();
            }
            heights[x] = (topDistance, bottomDistance);
        }

        //Walker algorithm over the entire cave to place granite blotches and what not
        for (int x = 0; x < heights.Length; x++)
        {
            if (!genRand.NextBool(4))
                continue;
            (int, int) height = heights[x];
            int heightToUse = genRand.NextBool(2) ? -height.Item1 : height.Item2;
            VeilGen.Walker(x, yMid + heightToUse, genRand.Next(32, 128), TileID.Granite, 10);
        }

        for (int x = 0; x < Main.maxTilesX; x++)
        {
            if (!genRand.NextBool(4))
                continue;
            VeilGen.Walker(x, DarkspaceStart, genRand.Next(64, 128), TileID.Granite, 15);
            VeilGen.Walker(x, DarkspaceEnd, genRand.Next(64, 128), TileID.Granite, 15);
        }
        //Then we go back through the cave, and create blotches of shimmer water in random spots
        //Again, not going to use gen actions here
        //Just going to create squares of shimmer water since it gets settled in a later pass
        int shimmerBlotchCount = 0;
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            //1 in X chance per tile to generate shimmer pool
            if (!genRand.NextBool(128))
                continue;

            int shimmerBlotchSize = genRand.Next(8, 16);
            Rectangle shimmerRect = new Rectangle(x - shimmerBlotchSize, yMid - shimmerBlotchSize, shimmerBlotchSize * 2, shimmerBlotchSize * 2);
            shimmerRect = TileUtilities.Clamp(shimmerRect);
            for (int tx = shimmerRect.Left; tx < shimmerRect.Right; tx++)
            {
                for (int ty = shimmerRect.Top; ty < shimmerRect.Bottom; ty++)
                {
                    Tile tile = Main.tile[tx, ty];
                    tile.LiquidType = LiquidID.Shimmer;
                    tile.LiquidAmount = 255;
                }
            }
            shimmerBlotchCount++;
        }

        WriteLine($"{shimmerBlotchCount} Darkspace Shimmer Blotches Placed");
        progress.Set(0.66D);

        //Here we're placing walls and silk tiles, this is a bit slow, so maybe optimize it a bit later.
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = yMin - 100; y < yMax + 100; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;

                bool hasRight = (x + 1 < Main.maxTilesX) && !WorldGen.SolidOrSlopedTile(x + 1, y);
                bool hasLeft = (x - 1 > 0) && !WorldGen.SolidOrSlopedTile(x - 1, y);
                bool hasTop = (y + 1 < Main.maxTilesY) && !WorldGen.SolidOrSlopedTile(x, y + 1);
                bool hasBottom = (y - 1 > 0) && !WorldGen.SolidOrSlopedTile(x, y - 1);
                bool hasAny = hasRight || hasLeft || hasTop || hasBottom;

                if (WorldGen.TileIsExposedToAir(x, y) && tile.TileType == TileID.Granite)
                {

                    if (genRand.NextBool(50))
                    {
                        float strength = genRand.Next(7, 11);
                        int steps = genRand.Next(12, 20);
                        ushort tileType = (ushort)ModContent.TileType<SilkTile>();

                        TileID.Sets.CanBeClearedDuringOreRunner[TileID.Granite] = true;
                        WorldGen.OreRunner(x, y,
                           strength,
                            steps, tileType);
                        TileID.Sets.CanBeClearedDuringOreRunner[TileID.Granite] = false;
                        WorldGen.PlaceTile(x, y, ModContent.TileType<MiracleSilkTile>(), mute: true, forced: true);
                        //     SilkManager.GrowSilk(x, y, genRand);
                    }
                }
                if (hasAny && (tile.TileType == TileID.Granite))
                {
                    //WorldGen.PlaceTile(x, y, TileID.Grass, forced: true);
                    Point point = new Point(x, y);
                    int steps = genRand.Next(1, 4);
                    Vector2 baseDirection = -Vector2.UnitY;
                    int wallCaveWidth = 3;

                    for (int s = 0; s < steps; s++)
                    {
                        if (point.X - wallCaveWidth > 0 && point.X + wallCaveWidth < Main.maxTilesX
                            && point.Y + wallCaveWidth < Main.maxTilesY && point.Y - wallCaveWidth > 0)
                        {
                            WorldUtils.Gen(point, new Shapes.Circle(wallCaveWidth, wallCaveWidth),
                                new Actions.PlaceWall(WallID.GraniteUnsafe));
                        }

                        point += (baseDirection * wallCaveWidth).RotatedByRandom(MathHelper.ToRadians(30)).ToPoint();
                    }
                }
            }
        }

        progress.Set(1D);


    }
    #region Cave Formation
    private void PlaceDesertBeams(Rectangle rectangle, Point location)
    {
        rectangle.Location = location;
        for (int beamX = rectangle.Location.X;
            beamX < rectangle.Location.X + rectangle.Width; beamX += 2)
        {
            //Place beams
            int beamY = rectangle.Location.Y;
            if (beamX < Main.maxTilesX && beamY < Main.maxTilesY)
            {

                Tile tile = Main.tile[beamX, beamY];
                int solidCount = 0;
                while (solidCount < 5)
                {
                    if (!WorldGen.SolidTile(beamX, beamY))
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
    }
    private void PlaceRibbonsandBeams(Rectangle structureRect, Point tilePoint)
    {
        var genRand = WorldGen.genRand;
        //Get top left tile
        Point leftRibbon = tilePoint;


        PlaceDesertBeams(structureRect, tilePoint);
        //Structures place from the bottom left, so we need to subtract theheight to convert them
        leftRibbon.Y -= structureRect.Height;
        leftRibbon.X += 1;

        //Set the right ribbon to the left ribbon and offset it
        Point rightRibbon = leftRibbon;
        rightRibbon.X += structureRect.Width;
        rightRibbon.X -= 1;

        for (int i = 0; i < 1000; i++)

        {
            if (WorldGen.SolidTile(leftRibbon.X, leftRibbon.Y))
            {
                break;
            }
            else
            {
                leftRibbon.Y++;
            }

        }




        for (int i = 0; i < 1000; i++)
        {
            if (WorldGen.SolidTile(rightRibbon.X, rightRibbon.Y))
            {
                break;
            }
            else
            {
                rightRibbon.Y++;
            }
        }


        PlaceRibbon(leftRibbon, -1, genRand.Next(8, 15));
        PlaceRibbon(rightRibbon, 1, genRand.Next(8, 15));
    }
    private bool TryPlaceDesertHouse(Point tilePoint, StructureMap structures)
    {
        string[] houseStructureFiles = new string[]
        {
            "Structures/DesertSurhouse1",
            "Structures/DesertSurhouse2",
            "Structures/DesertSurhouse3"
        };


        var genRand = WorldGen.genRand;
        string structureFile = houseStructureFiles[genRand.Next(0, houseStructureFiles.Length)];
        int[] tileBlend = new int[]
        {
            TileID.RubyGemspark
        };
        if (Structurizer.SafePlaceAndProtectStructure(tilePoint, structureFile, structures, tileBlend, out int[] chestIndices))
        {


            Rectangle structureRect = Structurizer.ReadRectangle(structureFile);
            PlaceRibbonsandBeams(structureRect, tilePoint);
            return true;
        }
        return false;
    }

    private void PlaceRibbon(Point tilePoint, int dir, int xLength)
    {
        Point highPoint = tilePoint;
        highPoint.X += dir * xLength;
        for (int i = 0; i < 100; i++)
        {
            if (WorldGen.SolidTile(highPoint))
            {
                break;
            }
            else
            {
                highPoint.Y++;
            }
        }

        //Now that we have the ribbons we can yeah
        RibbonRenderer ribbonRenderer = ModContent.GetInstance<RibbonRenderer>();
        RibbonWandType style = (RibbonWandType)WorldGen.genRand.Next(0, 5);
        ribbonRenderer.PlaceRibbon(tilePoint.ToWorldCoordinates(), highPoint.ToWorldCoordinates(), style);
    }
    private void WorldGenColosseum(GenerationProgress progress, GameConfiguration configuration)
    {
        var genRand = WorldGen.genRand;
        progress.Message = "Gintzing all over the desert";
        int desertCenterX = (GenVars.desertHiveLeft + GenVars.desertHiveRight) / 2;
        int desertSurfaceY = 0;
        int colosseumX = desertCenterX - 71;
        colosseumX += 35;

        int colosseumY = (int)Main.worldSurface - 50;
        while (!WorldGen.SolidTile(colosseumX, colosseumY))
        {
            colosseumY++;
        }

        desertSurfaceY = colosseumY;
        colosseumY += 40;
        Point colosseumPoint = new Point(colosseumX, colosseumY);

        //Place the colosseum
        StructureMap desertStructures = new StructureMap();
        VeilGen.GenerateColosseum(colosseumPoint, desertStructures);

        //Basically we're just gonna get random points on the colosseum and palce ribbons
        //This should look aight?
        //Hopefully lol
        int ribbonPlacementRange = 50;
        int numColosseumRibbons = 18;
        int ribbons = 0;
        for (int attempts = 0; attempts < 100000; attempts++)
        {
            //Just get some random points tbh, I forgot how big the colosseum is
            int randX = desertCenterX + genRand.Next(-ribbonPlacementRange, ribbonPlacementRange);

            //We want to use where the desert surface was cause the colosseum is in the gorund
            int randY = desertSurfaceY + genRand.Next(-100, -10);
            Point placementPoint = new Point(randX, randY);
            if (WorldGen.SolidTile(placementPoint))
            {
                int dir = Math.Sign(randX - desertCenterX);
                PlaceRibbon(placementPoint, dir, genRand.Next(8, 15));
                ribbons++;
                if (ribbons >= numColosseumRibbons)
                {
                    break;
                }
            }
            else
            {
                continue;
            }
        }

        //Ok, since the desert hive is a protected structure, we need to make a local structure map to safely place things on it
        //This is a bit annoying but it'll work


        //Generate the desert hide out

        int desertWidth = GenVars.desertHiveRight - GenVars.desertHiveLeft;
        int halfDesertWidth = desertWidth / 2;
        int minJailY = (int)(Main.worldSurface + 300);

        int[] tileBlend = new int[]
        {
            TileID.RubyGemspark
        };
        string hideoutStructureFile = "Structures/UndergroundDesertHideout";
        for (int attempts = 0; attempts < 10000; attempts++)
        {
            int hideoutSpawnRadius = 50;
            int randX = colosseumPoint.X + genRand.Next(-hideoutSpawnRadius, hideoutSpawnRadius);
            int randY = minJailY + genRand.Next(0, 300);
            Point structurePoint = new Point(randX, randY);
            if (Structurizer.SafePlaceAndProtectStructure(structurePoint, hideoutStructureFile, desertStructures, tileBlend, out int[] chestIndices))
            {
                break;
            }
        }

        //Place List House
        void RandomlyPlaceStructureInSurfaceDesert(string structure)
        {
            for (int attempts = 0; attempts < 10000; attempts++)
            {
                int randDesertX = genRand.Next(GenVars.desertHiveLeft, GenVars.desertHiveRight);
                int y = (int)(Main.worldSurface - 300);
                for (int m = 0; m < 1000; m++)
                {
                    y++;
                    if (WorldGen.SolidTile(randDesertX, y))
                    {


                        break;
                    }
                }

                Point tilePoint = new Point(randDesertX, y);
                if (Structurizer.SafePlaceAndProtectStructure(tilePoint, structure, desertStructures, tileBlend, out int[] chestIndices))
                {
                    Rectangle structureRect = Structurizer.ReadRectangle(structure);
                    PlaceRibbonsandBeams(structureRect, tilePoint);
                    break;
                }
            }
        }

        RandomlyPlaceStructureInSurfaceDesert("Structures/ListsHouse");
        RandomlyPlaceStructureInSurfaceDesert("Structures/DesertOrgan");
        RandomlyPlaceStructureInSurfaceDesert("Structures/DesertEresh");

        int newDesertLeft = GenVars.desertHiveLeft - Desert_Padding;
        int newDesertRight = GenVars.desertHiveRight + Desert_Padding;

        //Place Houses
        int numHouses = genRand.Next(12, 15);
        int houseCount = 0;
        for (int attempts = 0; attempts < 10000; attempts++)
        {
            int randX = genRand.Next(newDesertLeft, newDesertRight);
            int y = (int)(Main.worldSurface - 200);
            for (int yOffset = 0; yOffset < 500; yOffset++)
            {
                y++;
                if (!WorldGen.SolidTile(randX, y))
                    continue;

                Tile tile = Main.tile[randX, y];
                if (Main.tile[randX, y - 1].LiquidAmount > 0)
                    continue;

                if (tile.TileType == TileID.Sand)
                    break;

            }

            if (TryPlaceDesertHouse(new Point(randX, y), desertStructures))
            {
                houseCount++;
            }
            if (houseCount >= numHouses)
            {
                break;
            }
        }

        //Place sand decorations
        int numSandDecorations = genRand.Next(40, 60);
        int[] wallTypesToPlace = new int[]
        {
            ModContent.WallType<SandCastle1>(),
            ModContent.WallType<SandCastle2>(),
            ModContent.WallType<SandCastle3>(),
            ModContent.WallType<SandCastle4>(),
            ModContent.WallType<SandCastle5>(),
            ModContent.WallType<SandCastle6>(),
            ModContent.WallType<SandCastle7>()
        };


        for (int n = 0; n < numSandDecorations; n++)
        {
            int randX = genRand.Next(newDesertLeft, newDesertRight);
            int y = (int)(Main.worldSurface - 200);
            for (int yOffset = 0; yOffset < 500; yOffset++)
            {
                y++;
                if (!WorldGen.SolidTile(randX, y))
                    continue;
                Tile tile = Main.tile[randX, y];
                if (tile.TileType == TileID.Sand)
                    break;
            }

            int randSandCastle = genRand.Next(0, wallTypesToPlace.Length);
            int sandCastleType = wallTypesToPlace[randSandCastle];
            WorldGen.PlaceWall(randX, y, sandCastleType);
        }
    }

    private void WorldGenDock(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Fishing for femboys";
        var genRand = WorldGen.genRand;
        int dockX = Main.maxTilesX - 1;
        int dockY = (int)Main.worldSurface - 1000;

        //Get the edge of the right ocean
        Tile dockTile = Main.tile[dockX, dockY];
        while (dockTile.LiquidAmount <= 0)
        {
            dockY++;
            dockTile = Main.tile[dockX, dockY];
        }

        while (dockTile.LiquidAmount > 0)
        {
            dockX--;
            dockTile = Main.tile[dockX, dockY];
        }

        //Place the structure
        Point dockLoc = new Point(dockX, dockY + 1);
        dockLoc.Y -= 7;

        string structure = "Struct/Overworld/TheDock";
        int[] tileBlend = new int[]
        {
            TileID.RubyGemspark
        };

        dockLoc.X += 300;
        int[] ChestIndexs = Structurizer.ReadStruct(dockLoc, structure);
        Rectangle structureRectangle = Structurizer.ReadRectangle(structure);
        structureRectangle.Location = dockLoc;
        for (int beamX = structureRectangle.Location.X;
            beamX < structureRectangle.Location.X + structureRectangle.Width; beamX++)
        {
            //Place beams
            int beamY = structureRectangle.Location.Y;
            if (beamX < Main.maxTilesX && beamY < Main.maxTilesY)
            {

                Tile tile = Main.tile[beamX, beamY];
                if (tile.TileType != TileID.Sunplate)
                    continue;
                int solidCount = 0;
                while (solidCount < 5)
                {
                    if (!WorldGen.SolidTile(beamX, beamY))
                    {
                        WorldGen.PlaceTile(beamX, beamY, TileID.WoodenBeam);
                    }
                    else
                    {
                        solidCount++;
                    }
                    beamY++;
                }
            }

        }
    }

    private void WorldGenIceCaverns(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Carving out ice-y caverns";
        var genRand = WorldGen.genRand;

        int totalX = 0;
        int numX = 0;
        int minSnowX = 0;
        int maxSnowX = 1;
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            int y = (int)Main.worldSurface - 50;
            while (y < Main.maxTilesY)
            {
                y++;
                if (WorldGen.SolidTile(x, y) &&
                    (Main.tile[x, y].TileType == TileID.SnowBlock ||
                    Main.tile[x, y].TileType == TileID.IceBlock))
                {
                    if (numX == 0)
                    {
                        minSnowX = x;
                    }
                    else
                    {
                        maxSnowX = x;
                    }

                    numX++;
                    totalX += x;
                    break;
                }
            }
        }


        //Place Main Ice Tunnel
        int snowTunnelX = totalX / numX;
        int snowTunnelY = GenVars.snowTop - 100;
        Vector2 cavePosition = new Vector2(snowTunnelX, snowTunnelY);
        Vector2 caveVelocity = Vector2.UnitX;
        Vector2 caveStrength = new Vector2(20, 30);
        Vector2 pullDirection = Vector2.UnitY;
        int caveWidth = 7;
        int caveSteps = 100;
        VeilGen.GenerateFallingIceCavern(cavePosition, caveVelocity, pullDirection, caveStrength, caveWidth, caveSteps);

        //Place Ice Cavern Layers
        int numIceCaverns = genRand.Next(15, 20);
        int iceCavernY = GenVars.snowTop + 50;
        for (int c = 0; c < numIceCaverns; c++)
        {
            for (int n = 0; n < genRand.Next(1, 3); n++)
            {
                for (int a = 0; a < 1000; a++)
                {
                    //Attempts
                    int iceCavernX = genRand.Next(minSnowX, maxSnowX);

                    //Place the cavern
                    cavePosition = new Vector2(iceCavernX, iceCavernY);
                    Point iceCavernTile = cavePosition.ToPoint();
                    if (!WorldGen.SolidTile(iceCavernTile))
                        continue;
                    if (Main.tile[iceCavernTile.X, iceCavernTile.Y].TileType != TileID.IceBlock &&
                        Main.tile[iceCavernTile.X, iceCavernTile.Y].TileType != TileID.SnowBlock)
                        continue;


                    caveVelocity = Vector2.UnitX;
                    if (cavePosition.X > snowTunnelX)
                        caveVelocity = -Vector2.UnitX;
                    caveStrength = new Vector2(20, 30);
                    caveWidth = genRand.Next(5, 8);
                    caveSteps = genRand.Next(70, 100);
                    VeilGen.GenerateIceCavern(cavePosition, caveVelocity, caveStrength, caveWidth, caveSteps);

                    //Place holes to more
                    int numTunnels = genRand.Next(15, 20);
                    for (int t = 0; t < numTunnels; t++)
                    {
                        cavePosition = new Vector2(iceCavernX, iceCavernY);
                        cavePosition += new Vector2(0, genRand.Next(0, 300));
                        caveVelocity = Vector2.UnitX;
                        if (genRand.NextBool(2))
                        {
                            caveVelocity = -Vector2.UnitX;
                        }
                        caveStrength = new Vector2(5, 10);
                        caveWidth = genRand.Next(5, 8);
                        caveSteps = genRand.Next(15, 30);

                        pullDirection = Vector2.UnitY;
                        VeilGen.GenerateFallingIceCavern(cavePosition, caveVelocity, pullDirection, caveStrength, caveWidth, caveSteps);
                    }
                    break;
                }


            }

            iceCavernY += 50;
        }

        int abyssTunnelX = genRand.Next(GenVars.snowOriginLeft, GenVars.snowOriginRight);
        cavePosition = new Vector2(abyssTunnelX, iceCavernY - 50);
        caveVelocity = Vector2.UnitY;
        caveStrength = new Vector2(15, 20);
        pullDirection = -Vector2.UnitX * 0.2f;
        caveWidth = 7;
        caveSteps = 100;
        VeilGen.GenerateFallingIceCavern(cavePosition, caveVelocity, pullDirection, caveStrength, caveWidth, caveSteps);
    }

    private void WorldGenDelgrim(GenerationProgress progress, GameConfiguration configuration)
    {

    }

    private void WorldGenGraniteCaves(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Forming granite & marble caves";
        var genRand = WorldGen.genRand;

        int y = Main.maxTilesY / 2; ;
        int centerX = Main.maxTilesX / 2;
        Point granitePoint = Point.Zero;
        granitePoint.X = centerX - 96;
        granitePoint.Y = y;
        int direction = -1;


        //Place DELGRIM
        string structure = "Struct/Underground/DelgrimShop";
        Point pointToPlaceDelgrimShop = granitePoint - new Point(0, genRand.Next(400, 500));
        while (!Structurizer.TryPlaceAndProtectStructure(pointToPlaceDelgrimShop, structure))
        {
            pointToPlaceDelgrimShop += genRand.NextVector2Circular(4, 4).ToPoint();

        }

        Structurizer.ReadStruct(pointToPlaceDelgrimShop, structure);
        Rectangle structureRectangle = Structurizer.ReadRectangle(structure);
        structureRectangle.Location = pointToPlaceDelgrimShop;
        for (int beamX = structureRectangle.Location.X;
            beamX < structureRectangle.Location.X + structureRectangle.Width; beamX += 4)
        {
            int beamY = structureRectangle.Location.Y;
            int solidCount = 0;
            while (solidCount < 5)
            {
                if (!WorldGen.SolidTile(beamX, beamY))
                {
                    WorldGen.PlaceTile(beamX, beamY, TileID.WoodenBeam);
                }
                else
                {
                    solidCount++;
                }
                beamY++;
            }
        }
        for (int n = 0; n < 20; n++)
        {
            if (granitePoint.Y >= Main.maxTilesY - 500)
                break;

            for (int a = 0; a < 1000; a++)
            {

                Vector2 radiusSize = new Vector2(24, 64);
                int caveWidth = 5;
                while (!WorldGen.SolidTile(granitePoint) && granitePoint.Y < Main.maxTilesY - 500)
                {
                    granitePoint.Y++;
                }

                if (genRand.NextBool(2))
                {
                    VeilGen.PlaceGranite(granitePoint, radiusSize, caveWidth);
                }
                else
                {
                    VeilGen.PlaceMarble(granitePoint, radiusSize, caveWidth);
                }
                granitePoint.X += direction == 1 ? -96 : 96;
                direction *= -1;
                granitePoint.Y += 80;
                break;
            }

        }
    }

    private void WorldGen_EvilCircle(Point evilPoint)
    {
        var genRand = WorldGen.genRand;
        int radius = 96;
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
            float p = n / pokey;
            float rot = p * MathHelper.TwoPi;
            Vector2 velocity = rot.ToRotationVector2() * 66;
            Point cavePoint = evilPoint + velocity.ToPoint();
            Vector2 strength = new Vector2(3, 4);

            Vector2 moveVelocity = -velocity.SafeNormalize(Vector2.Zero);
            VeilGen.GenerateSimpleCave(cavePoint.ToVector2(), moveVelocity,
                strength, moveVelocity, 2, caveSteps: 30);
        }

        for (int n = 0; n < 800; n++)
        {
            float p = n / 800f;
            float rot = p * MathHelper.TwoPi;
            Vector2 velocity = rot.ToRotationVector2() * genRand.NextFloat(50, 80);
            Point cavePoint = evilPoint + velocity.ToPoint();
            Vector2 strength = new Vector2(3, 4);

            WorldGen.TileRunner(cavePoint.X, cavePoint.Y,
                genRand.NextFloat(strength.X, strength.Y),
                genRand.Next(4, 5), -1);
        }

        for (int n = 0; n < 800; n++)
        {
            float p = n / 800f;
            float rot = p * MathHelper.TwoPi;
            Vector2 velocity = rot.ToRotationVector2() * genRand.NextFloat(50, 80);
            Point cavePoint = evilPoint + velocity.ToPoint();
            Vector2 strength = new Vector2(3, 4);


            WorldGen.TileRunner(cavePoint.X, cavePoint.Y,
                genRand.NextFloat(strength.X, strength.Y),
                genRand.Next(4, 5), decorativeBlock);
        }

        for (int n = 0; n < 800; n++)
        {
            float p = n / 800f;
            float rot = p * MathHelper.TwoPi;
            Vector2 velocity = rot.ToRotationVector2() * genRand.NextFloat(60, 100);
            Point cavePoint = evilPoint + velocity.ToPoint();
            Vector2 strength = new Vector2(3, 4);

            WorldGen.TileRunner(cavePoint.X, cavePoint.Y,
                genRand.NextFloat(strength.X, strength.Y),
                genRand.Next(4, 5), decorativeBlock);
        }

        for (int n = 0; n < 10; n++)
        {
            float p = n / 10f;
            float rot = p * MathHelper.TwoPi;
            rot += MathHelper.ToRadians(30);
            Vector2 velocity = rot.ToRotationVector2() * 10;
            Point shadowOrbPoint = evilPoint + velocity.ToPoint();
            WorldGen.AddShadowOrb(shadowOrbPoint.X, shadowOrbPoint.Y);
        }

        for (int n = 0; n < 10; n++)
        {
            float p = n / 10f;
            float rot = p * MathHelper.TwoPi;
            rot += MathHelper.ToRadians(60);
            Vector2 velocity = rot.ToRotationVector2() * 30;
            Point shadowOrbPoint = evilPoint + velocity.ToPoint();
            WorldGen.AddShadowOrb(shadowOrbPoint.X, shadowOrbPoint.Y);
        }

        for (int n = 0; n < 10; n++)
        {
            float p = n / 10f;
            float rot = p * MathHelper.TwoPi;
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

        VeilGen.GenerateStraightCaveWall((evilPoint + new Point(-16, -32)).ToVector2(), pullDirection, caveStrength * 2f, pullDirection, caveWidth, caveSteps: steps, tileToPlace: wallType);
        VeilGen.GenerateStraightCave((evilPoint + new Point(-16, -32)).ToVector2(), pullDirection, caveStrength * 2f, pullDirection, caveWidth, caveSteps: steps, tileToPlace: blockType);
        VeilGen.GenerateStraightCave((evilPoint + new Point(-16, -32)).ToVector2(), pullDirection, caveStrength, pullDirection, caveWidth, caveSteps: steps, tileToPlace: -1);

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
        for (int x = evilPoint.X - radius; x < evilPoint.X + radius; x++)
        {
            for (int y = evilPoint.Y - radius; y < evilPoint.Y + radius; y++)
            {
                if (!WorldGen.SolidTile(x, y))
                    continue;
                Tile tile = Main.tile[x, y];
                if (tile.TileType == TileID.Grass)
                {
                    ushort grassType = WorldGen.crimson ? TileID.CrimsonGrass : TileID.CorruptGrass;
                    WorldGen.PlaceTile(x, y, grassType);
                }
                if (tile.TileType == TileID.Stone)
                {
                    WorldGen.PlaceTile(x, y, blockType);
                }
            }
        }
    }

    private void WorldGenEvil(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Making the evil";
        var genRand = WorldGen.genRand;
        Point evilPoint = MistyHillEndLocation;
        evilPoint.X += 200;
        evilPoint.Y -= 300;
        evilPoint = FallToSolidTile(evilPoint);
        evilPoint.Y += 150;

    }

    private void WorldGenAshotiTemple(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Burying Ashoti";

        var genRand = WorldGen.genRand;
        int radius = 80;
        int desertCenterX = (GenVars.desertHiveLeft + GenVars.desertHiveRight) / 2;
        int desertCenterY = GenVars.desertHiveLow - 200;
        Point arenaPoint = new Point(desertCenterX, desertCenterY);
        Main.tileSolid[TileID.LihzahrdBrick] = true;

        //Building the arena
        WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius, radius), new Actions.SetTile(TileID.LihzahrdBrick));
        WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius - 2, radius - 2), new Actions.SetTile((ushort)ModContent.TileType<ChiseledStone>()));
        WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius - 4, radius - 4), new Actions.SetTile((ushort)ModContent.TileType<NoxianBlock>()));
        WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius - 6, radius - 6), new Actions.ClearTile());
        WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius / 2, radius / 2), new Actions.SetLiquid(type: LiquidID.Lava));
        string structure;


        //Place the center piece where the thing be
        structure = "Struct/AshotiTemple/TempleBottom";
        Rectangle templeBottomRect = Structurizer.ReadRectangle(structure);
        Point templeBottomToPlace = arenaPoint;
        templeBottomToPlace.X -= templeBottomRect.Width / 2;
        templeBottomToPlace.Y += templeBottomRect.Height;
        Structurizer.ReadStruct(templeBottomToPlace, structure);
        Structurizer.ProtectStructure(templeBottomToPlace, structure);


        //Decorate arena with walls
        for (int w = 0; w < 80; w++)
        {
            float progressOnCircle = w / 80f;
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
                structure = "Struct/AshotiTemple/TempleEntrance";
                Rectangle rect = Structurizer.ReadRectangle(structure);
                tileToPlaceOn.X -= rect.Width / 2;
                tileToPlaceOn.Y -= 28;
                int[] chestIndices = Structurizer.ReadStruct(tileToPlaceOn, structure);
                Structurizer.ProtectStructure(tileToPlaceOn, structure);
            }
            else
            {
                structure = "Struct/AshotiTemple/TempleMiddle";
                Rectangle rect = Structurizer.ReadRectangle(structure);
                tileToPlaceOn.X -= rect.Width / 2;
                int[] chestIndices = Structurizer.ReadStruct(tileToPlaceOn, structure);
                foreach (int chestIndex in chestIndices)
                {
                    if (chestIndex == -1)
                        continue;
                    Chest chest = Main.chest[chestIndex];
                    var itemsToAdd = new List<(int type, int stack)>();

                    //Golem Drops
                    switch (genRand.Next(8))
                    {
                        case 0:
                            itemsToAdd.Add((ItemID.Stynger, 1));
                            itemsToAdd.Add((ItemID.StyngerBolt, genRand.Next(60, 100)));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.PossessedHatchet, 1));
                            break;
                        case 2:
                            itemsToAdd.Add((ItemID.SunStone, 1));
                            break;
                        case 3:
                            itemsToAdd.Add((ItemID.EyeoftheGolem, 1));
                            break;
                        case 4:
                            itemsToAdd.Add((ItemID.EyeoftheGolem, 1));
                            break;
                        case 5:
                            itemsToAdd.Add((ItemID.HeatRay, 1));
                            break;
                        case 6:
                            itemsToAdd.Add((ItemID.StaffofEarth, 1));
                            break;
                        case 7:
                            itemsToAdd.Add((ItemID.GolemFist, 1));
                            break;
                    }

                    if (genRand.NextBool(3))
                    {
                        switch (genRand.Next(2))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<Lihh>(), 1));
                                break;
                            case 1:
                                itemsToAdd.Add((ModContent.ItemType<Relagis>(), 1));
                                break;
                        }
                    }

                    itemsToAdd.Add((ItemID.LihzahrdPowerCell, 1));
                    itemsToAdd.Add((ItemID.LihzahrdFurnace, 1));

                    if (genRand.NextBool(3))
                    {
                        switch (genRand.Next(2))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.GreaterHealingPotion, genRand.Next(2, 6)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.GreaterManaPotion, genRand.Next(2, 6)));
                                break;
                        }
                    }

                    switch (genRand.Next(2))
                    {
                        case 0:
                            itemsToAdd.Add((ItemID.SolarTablet, 1));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.LunarTabletFragment, genRand.Next(3, 8)));
                            break;
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
                Structurizer.ProtectStructure(tileToPlaceOn, structure);
            }
        }
    }


    private void WorldGenShimmerSpot(GenerationProgress progress, GameConfiguration configuration)
    {
        //If we don't do this we'll get a generation error
        progress.Message = "Faking the Shimmer";
        GenVars.shimmerPosition = new ReLogic.Utilities.Vector2D(Main.maxTilesX * 0.5f, Main.maxTilesY * 0.5f);
    }

    private void WorldGenGrassPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Grassing Caves";
        var genRand = WorldGen.genRand;
        int fluff = 10;
        int startFloweringY = (int)(Main.worldSurface - 25);
        int startGrassingY = startFloweringY - 600;
        for (int x = fluff; x < Main.maxTilesX - fluff; x++)
        {
            for (int y = startGrassingY; y < (int)Main.worldSurface + 600; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;
                if (!VeilGen.IsTileExposedToAirCardinal(x, y))
                    continue;

                if ((tile.TileType == TileID.Dirt || tile.TileType == TileID.Stone || tile.TileType == TileID.Grass))
                {
                    tile.TileType = TileID.Grass;
                    VeilGen.WallWalker(x, y, genRand.Next(2, 6) * 3, WallID.FlowerUnsafe, 3);
                }
            }
        }
    }

    private void WorldGenVirulentCaves(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Spreading the Virulent";
        var genRand = WorldGen.genRand;
        Point seedPoint = pointL;
        seedPoint.Y += 550;
        Vector2 seedPosition = seedPoint.ToVector2();
        Vector2 caveStrength = new Vector2(40, 50);
        int caveWidth = 20;
        int caveSteps = 500;
        for (int x = 0; x < 8; x++)
        {
            Vector2 openSeedPosition = seedPosition + genRand.NextVector2Circular(32, 32);
            VeilGen.GenerateOpenCaveClearing(openSeedPosition, -Vector2.UnitY,
                caveStrength, caveWidth, caveSteps);
        }


        for (int y = pointL.Y - 500; y < seedPoint.Y; y += genRand.Next(50, 100))
        {
            int leftX = pointL.X - genRand.Next(150, 250);
            int rightX = pointL.X + genRand.Next(150, 250);
            Vector2 leftCavePosition = new Vector2(leftX, y);
            Vector2 rightCavePosition = new Vector2(rightX, y);

            Vector2 virulentCaveStrength = new Vector2(7, 15);
            int virulentCaveWidth = genRand.Next(5, 10);
            int virulentCaveSteps = genRand.Next(200, 300);

            VeilGen.GenerateVirulentCave(leftCavePosition, seedPosition, Vector2.UnitX,
                virulentCaveStrength,
                virulentCaveWidth,
                virulentCaveSteps);


            VeilGen.GenerateVirulentCave(rightCavePosition, seedPosition, -Vector2.UnitX,
                virulentCaveStrength,
                virulentCaveWidth,
                virulentCaveSteps);
        }
    }

    private void WorldGenMineshafts(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Shafting";
        var genRand = WorldGen.genRand;

        int numMineshafts = 18;
        for (int n = 0; n < numMineshafts; n++)
        {
            for (int a = 0; a < 10000; a++)
            {
                int x = genRand.Next(250, Main.maxTilesX - 250);
                int y = genRand.Next((int)GenVars.rockLayerHigh, GenVars.lavaLine);
                Tile tile = Main.tile[x, y];
                if (tile.TileType != TileID.Stone)
                    continue;
                Point tilePoint = new Point(x, y);
                Point tileDirection = new Point(1, 0);
                int tunnel = genRand.Next(7, 25);
                VeilGen.GenerateMineshaftTunnel(tilePoint, tileDirection, tunnel);
                break;
            }
        }

    }

    private void HardWallsPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Hardening Walls";
        var genRand = WorldGen.genRand;
        int start = DarkspaceStart - 700;
        int end = Main.UnderworldLayer;
        int[] wallTypes = new int[]
        {
            WallID.ObsidianBackUnsafe,
            WallID.RocksUnsafe1,
            WallID.Cave4Unsafe,
            WallID.Cave5Unsafe
        };



        int charredStoneTypeInt = ModContent.TileType<CharredStone>();
        int padding = 2;
        for (int x = padding; x < Main.maxTilesX - padding; x++)
        {
            for (int y = start; y < end; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.TileType == charredStoneTypeInt && tile.HasTile && VeilGen.IsTileExposedToAirCardinal(x, y))
                {
                    if (genRand.NextBool(3))
                    {
                        int steps = genRand.Next(30, 90);
                        int maxDist = 3;
                        VeilGen.WallWalker(x, y, steps, wallTypes[genRand.Next(4)], maxDist, PaintID.BlackPaint);
                    }
                }
            }

            progress.Set((x - (float)padding) / (Main.maxTilesX - (float)padding));
        }
    }

    private void HardRocksPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Hardening Stones";
        var genRand = WorldGen.genRand;
        int start = DarkspaceEnd;
        int end = Main.UnderworldLayer;
        HeatedDepthsStart = start;
        HeatedDepthsEnd = CindersparkStart;
        ushort charredStoneType = (ushort)ModContent.TileType<CharredStone>();
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = start; y < end; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.TileType == TileID.Stone || tile.TileType == TileID.Dirt)
                    tile.TileType = charredStoneType;
            }
        }

        int charredStoneTypeInt = ModContent.TileType<CharredStone>();
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            if (!genRand.NextBool(8))
                continue;

            int y = start + genRand.Next(-3, 3);
            Tile tile = Main.tile[x, y];
            if (tile.TileType != charredStoneTypeInt)
                continue;

            int steps = genRand.Next(400, 600);
            int maxDist = 8;
            VeilGen.Walker(x, y, steps, charredStoneTypeInt, maxDist);

            //Place at bottom of layer too
            y = end + genRand.Next(-3, 3);
            tile = Main.tile[x, y];
            if (tile.TileType != charredStoneTypeInt)
                continue;
            VeilGen.Walker(x, y, steps, charredStoneTypeInt, maxDist);
        }

        //Turn some of the charred stones to obsidian
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            if (!genRand.NextBool(16))
                continue;

            int y = (int)MathHelper.Lerp(start, end, genRand.NextFloat());
            Tile tile = Main.tile[x, y];
            if (tile.TileType == charredStoneTypeInt)
            {
                int steps = genRand.Next(30, 90);
                int maxDist = 4;
                VeilGen.Walker(x, y, steps, TileID.Obsidian, maxDist);
            }
        }


    }

    private void CavernWaters(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Cave Waters...";
        var genRand = WorldGen.genRand;
        float maxCount = Main.maxTilesX * Main.maxTilesY * 0.0000003f;
        float maxAttemptCount = maxCount * 10;
        float placed = 0;
        int padding = 250;

        for (int i = 0; i < maxAttemptCount; i++)
        {
            int x = genRand.Next(padding, Main.maxTilesX - padding);
            int y = genRand.Next((int)GenVars.rockLayerHigh, DarkspaceStart);
            if (VeilGen.IsTileNearby(x, y, distance: 50, TileSets.BlockMineshafts))
                continue;

            Tile startTile = Main.tile[x, y];
            if (!startTile.HasTile)
            {
                int waterBlotchSize = genRand.Next(12, 20);
                Rectangle placementRect = new Rectangle(x - waterBlotchSize, y - waterBlotchSize, waterBlotchSize * 2, waterBlotchSize * 2);
                placementRect = TileUtilities.Clamp(placementRect);
                for (int tx = placementRect.Left; tx < placementRect.Right; tx++)
                {
                    for (int ty = placementRect.Top; ty < placementRect.Bottom; ty++)
                    {
                        Tile tile = Main.tile[tx, ty];
                        tile.LiquidType = LiquidID.Water;
                        tile.LiquidAmount = 255;
                    }
                }

                placed++;
                if (placed >= maxCount)
                    break;
            }
        }
    }

    private void RavinesPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Giant Ravines";

        int walkerWidth = 64;
        int walkerSteps = 4000;
        var genRand = WorldGen.genRand;
        void Carve(int x, int y)
        {
            Point walkerPoint = new Point(x, y);
            Point originalPoint = walkerPoint;
            for (int s = 0; s < walkerSteps; s++)
            {
                switch (genRand.Next(4))
                {
                    case 0:
                        walkerPoint.X--;
                        break;
                    case 1:
                        walkerPoint.X++;
                        break;
                    case 2:
                        walkerPoint.Y++;
                        break;
                    case 3:
                        walkerPoint.Y--;
                        break;
                }
                walkerPoint = TileUtilities.Clamp(walkerPoint);
                Tile tile = Main.tile[walkerPoint];
                tile.ClearTile();

                //Reset if walking too far
                int dx = Math.Abs(walkerPoint.X - originalPoint.X);
                int dy = Math.Abs(walkerPoint.Y - originalPoint.Y);
                if (dx > walkerWidth || dy > walkerWidth)
                {
                    walkerPoint = originalPoint;
                }
            }
        }

        float numRavines = 5;
        for (float ravines = 0; ravines < numRavines; ravines++)
        {
            for (int s = 0; s < Main.maxTilesX; s += 4)
            {
                float p = ravines / numRavines;
                int x = s;
                int y = (int)MathHelper.Lerp(HeatedDepthsEnd, HeatedDepthsStart, p);
                walkerWidth = (int)MathHelper.Lerp(16, 3, p);
                Carve(x, y);
                //Random chance to skip several steps, which will create gaps in the caves
                if (genRand.NextBool(128))
                {
                    s += 144;
                }
            }
        }



        //Vertical Caves

        //Here we're going to use the same technique i used in the darkspace
        FastNoiseLite topFNL = new FastNoiseLite();
        topFNL.SetSeed(genRand.Next(0, int.MaxValue));
        topFNL.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        topFNL.SetFrequency(0.15f);
        topFNL.SetDomainWarpAmp(10);
        topFNL.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);

        FastNoiseLite bottomFNL = new FastNoiseLite();
        bottomFNL.SetSeed(genRand.Next(0, int.MaxValue));
        bottomFNL.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        bottomFNL.SetFrequency(0.15f);
        bottomFNL.SetDomainWarpAmp(10);
        bottomFNL.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);

        float numCaves = Main.maxTilesX * (float)Main.maxTilesY * 0.00012f;
        for (float f = 0; f < numCaves; f++)
        {
            //Reset the seed for each cave
            topFNL.SetSeed(genRand.Next(0, int.MaxValue));
            bottomFNL.SetSeed(genRand.Next(0, int.MaxValue));

            int sx = genRand.Next(0, Main.maxTilesX);
            int sy = genRand.Next(HeatedDepthsStart, HeatedDepthsEnd);
            Tile startTile = Main.tile[sx, sy];

            //Only place on air, guaranteeing that the cave connects to another cave
            if (startTile.HasTile)
                continue;

            int minCaveDistance = genRand.Next(3, 4);
            int maxCaveDistance = genRand.Next(6, 8);
            int steps = genRand.Next(72, 154);
            int dir = genRand.NextBool(2) ? -1 : 1;
            for (int s = 0; s < steps; s++)
            {
                float SampleNoise(int x, int y)
                {
                    return topFNL.GetNoise(x * 0.05f, y * 0.05f) * 0.5f + 0.5f;
                }
                float SampleNoise2(int x, int y)
                {
                    return bottomFNL.GetNoise(x * 0.05f, y * 0.05f) * 0.5f + 0.5f;
                }

                int y = sy + s * dir;
                if (y <= 0 || y >= Main.maxTilesY)
                    break;

                float topNoise = SampleNoise(sx, y);
                float bottomNoise = SampleNoise2(sx, y);

                //Cave middle up
                int topDistance = (int)MathHelper.Lerp(minCaveDistance, maxCaveDistance, topNoise) + genRand.Next(-1, 1);
                for (int x = 0; x < topDistance; x++)
                {
                    int newX = sx - x;
                    if (newX <= 0)
                        break;

                    Tile tile = Main.tile[newX, y];
                    tile.ClearEverything();
                }

                //Cave middle down
                int bottomDistance = (int)MathHelper.Lerp(minCaveDistance, maxCaveDistance, bottomNoise) + genRand.Next(-1, 1);
                for (int x = 0; x < bottomDistance; x++)
                {
                    int newX = sx + x;
                    if (newX >= Main.maxTilesX)
                        break;
                    Tile tile = Main.tile[newX, y];
                    tile.ClearEverything();
                }
            }
        }


        //Place Lava Bowls
        float numLavaBowls = numCaves * 2;
        int padding = 30;
        for (float f = 0; f < numLavaBowls; f++)
        {
            //Reset the seed for each cave
            int sx = genRand.Next(padding, Main.maxTilesX - padding);
            int sy = genRand.Next(HeatedDepthsStart, HeatedDepthsEnd);
            Tile startTile = Main.tile[sx, sy];

            //Only place on air, guaranteeing that the lava is inside of a cave/exposed to air
            if (startTile.HasTile)
                continue;

            //Gotta land on a solid tile
            while (!startTile.HasTile && sy < Main.UnderworldLayer)
            {
                sy++;
                startTile = Main.tile[sx, sy];
            }

            //Dimensions of the lava bowl
            int width = genRand.Next(5, 12);
            int depth = genRand.Next(5, 12);
            int left = sx - width / 2;
            int right = sx + width / 2;
            for (int x = left; x < right; x++)
            {
                float numSteps = right - left;
                int d = (int)MathHelper.Lerp(0, depth, EasingFunction.QuadraticBump((x - left) / numSteps));
                for (int y = sy; y < sy + d; y++)
                {
                    Tile tile = Main.tile[x, y];
                    tile.ClearTile();
                    tile.LiquidAmount = 255;
                    tile.LiquidType = LiquidID.Lava;
                }
            }
        }


        CellularAutomataParams @params = new CellularAutomataParams() with { Steps = 2, RandomFill = 55, BirthLimit = 4, DeathLimit = 4 };
        Rectangle smoothRectangle = new Rectangle(0, HeatedDepthsStart, Main.maxTilesX, HeatedDepthsEnd - HeatedDepthsStart);
        VeilGen.AutomataSmoothErase(smoothRectangle, in @params);
    }

    private void WorldGenDarkstone(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Blackening Stones for racist effect";
        var genRand = WorldGen.genRand;
        float maxCaveCount = Main.maxTilesX * Main.maxTilesY * 0.00008f;
        for (int k = 0; k < maxCaveCount; k++)
        {
            int x = genRand.Next(0, Main.maxTilesX);
            int y = genRand.Next((int)GenVars.rockLayerHigh, DarkspaceStart);
            if (!TileID.Sets.Stone[Main.tile[x, y].TileType])
                continue;

            VeilGen.Walker(x, y, WorldGen.genRand.Next(128, 256), ModContent.TileType<DiminishedStone>(), 24);
        }
    }

    private void ExtraCavesPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Simple Caves";
        var genRand = WorldGen.genRand;
        float maxCaveCount = Main.maxTilesX * Main.maxTilesY * 0.00001f;
        float maxAttemptCount = maxCaveCount * 10;
        float placedCaves = 0;
        int padding = 2000;
        for (int i = 0; i < maxAttemptCount; i++)
        {
            int x = genRand.Next(padding, Main.maxTilesX - padding);
            int y = genRand.Next((int)GenVars.rockLayerHigh, DarkspaceStart);
            if (VeilGen.IsTileNearby(x, y, distance: 50, TileSets.BlockMineshafts))
                continue;

            Tile tile = Main.tile[x, y];
            if (Main.tileSolid[tile.TileType] && tile.HasTile && TileID.Sets.Stone[tile.TileType])
            {
                WorldGen.Caverer(x, y);
                placedCaves++;
                if (placedCaves >= maxCaveCount)
                    break;
            }
        }
    }

    private void ReplaceLavaWithShimmerPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Stay Shimmering";
        Rectangle rec = new Rectangle(0, DarkspaceStart, Main.maxTilesX, DarkspaceEnd - DarkspaceStart);
        for (int x = rec.Left; x < rec.Right; x++)
        {
            for (int y = rec.Top; y < rec.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                bool isAethirumBlock = (tile.HasTile && tile.TileType == TileID.ShimmerBlock);
                if (tile.LiquidType == LiquidID.Lava || tile.LiquidType == LiquidID.Water)
                {
                    tile.LiquidType = LiquidID.Shimmer;
                    // tile.LiquidAmount = (byte)WorldGen.genRand.Next(125, 255);
                }
            }
        }
    }

    private void DeepCavesPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Caves cut deep...";
        //Bottom Ravines


        //First we should generate corridors starting from the top of the stone layer all the way to darkspace
        //Actually they just cut through the whole world, ignoring ice and jungle / desert
        var genRand = WorldGen.genRand;
        float maxCaveCount = Main.maxTilesX * Main.maxTilesY * 0.000005f;
        float maxAttemptCount = maxCaveCount * 10;
        float placedCaves = 0;
        int padding = 1000;

        FastNoiseLite fnl = new FastNoiseLite();
        fnl.SetSeed(genRand.Next(0, int.MaxValue));
        fnl.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        fnl.SetFrequency(0.15f);
        fnl.SetDomainWarpAmp(10);
        fnl.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
        for (int i = 0; i < maxAttemptCount; i++)
        {
            int x = genRand.Next(padding + 1700, Main.maxTilesX - padding);
            int y = genRand.Next((int)GenVars.rockLayerHigh, DarkspaceStart);
            if (VeilGen.IsTileNearby(x, y, distance: 50, TileSets.BlockMineshafts))
                continue;

            Tile tile = Main.tile[x, y];
            if (Main.tileSolid[tile.TileType] && tile.HasTile && TileID.Sets.Stone[tile.TileType])
            {
                fnl.SetSeed(genRand.Next(0, int.MaxValue));
                Vector2 initialDirection = Vector2.UnitY.RotateRandom(MathHelper.Pi);
                int caveSteps = 800;
                int walkerSteps = genRand.Next(200, 400);
                int walkerWidth = (int)MathHelper.Lerp(2, 5, (float)(y - (float)GenVars.rockLayerHigh) / (DarkspaceStart - (float)GenVars.rockLayerHigh));
                VeilGen.PlaceDeepCuttingCave(new Point(x, y).ToWorldCoordinates(), initialDirection, caveSteps, walkerSteps, walkerWidth, genRand, fnl);
                placedCaves++;
                if (placedCaves >= maxCaveCount)
                    break;
            }
        }

    }
    private void MineshaftsPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Enriching the underground...";
        var genRand = WorldGen.genRand;

        //Alright so here's our algorithm
        int padding = 1700;
        float placedShafts = 0;
        float shaftCount = Main.maxTilesX * Main.maxTilesY * 0.0000005f;
        float maxAttemptCount = shaftCount * 10;

        //Generate all mienshafts in advance, generating them late is much slower
        //If you're going to do prepare rooms, have them all at the same time
        Queue<(Rectangle mapBounds, Room[] map)> mineshaftQueue = new Queue<(Rectangle mapBounds, Room[] map)>();
        for (int i = 0; i < shaftCount; i++)
        {
            mineshaftQueue.Enqueue(VeilGen.GenerateMineshaft(genRand));
        }

        (Rectangle mapBounds, Room[] map) = mineshaftQueue.Dequeue();
        for (float n = 0; n < maxAttemptCount; n++)
        {
            int x = genRand.Next(padding, Main.maxTilesX - padding);
            int y = genRand.Next((int)GenVars.rockLayerHigh, DarkspaceStart - 200);
            if (VeilGen.IsTileNearby(x, y, distance: 200, TileSets.BlockMineshafts))
                continue;

            Tile tile = Main.tile[x, y];
            if (Main.tileSolid[tile.TileType] && tile.HasTile && TileID.Sets.Stone[tile.TileType])
            {
                if (VeilGen.PlaceMineshaft(new Point(x, y), mapBounds, map))
                {
                    placedShafts++;
                    if (placedShafts >= shaftCount)
                        break;

                    (mapBounds, map) = mineshaftQueue.Dequeue();
                }
            }

            progress.Set((double)n / placedShafts);
        }
        WriteLine($"{placedShafts} Mineshafts Placed");
    }


    private void TreeCavesPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Tree-like Caves carve deeply...";
        var genRand = WorldGen.genRand;
        //High Tree Caves
        int worldsEndEdge = 3300;
        int maxX = (int)(Main.maxTilesX * 0.8f);
        for (int x = worldsEndEdge; x < maxX; x++)
        {
            int caveMakerSteps = 32;
            for (int j = 0; j < caveMakerSteps; j++)
            {
                int y = genRand.Next((int)GenVars.worldSurfaceLow - 25, (int)GenVars.rockLayerHigh);
                Tile tile = Main.tile[x, y];
                if (tile.TileType == TileID.Sand ||
                    tile.TileType == TileID.Mud ||
                    tile.TileType == TileID.SnowBlock ||
                    tile.TileType == TileID.IceBlock)
                    continue;
                if (!genRand.NextBool(1512))
                    continue;
                int caveWidth = genRand.Next(4, 7);
                int caveSteps = genRand.Next(50, 80);

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
        }
    }

    private void JungleCavesPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "The Jungle Branches Out...";
        var genRand = WorldGen.genRand;

        int num = genRand.Next(120, 150);
        for (int n = 0; n < num; n++)
        {
            int originX = GenVars.jungleOriginX;
            int x = genRand.Next(originX - 1000, originX + 1000);
            int yMax = Main.maxTilesY;
            int yMin = yMax - 500;
            int y = genRand.Next(yMin, yMax);

            int caveWidth = genRand.Next(3, 6);
            int caveSteps = genRand.Next(300, 700);

            //Cave position in tiles
            Vector2 cavePosition = new Vector2(x, y);

            //Starting cave direction
            Vector2 baseCaveDirection = genRand.NextBool(2) ? Vector2.UnitX : -Vector2.UnitX;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

            //How much the tile runner is gonna carve out
            Vector2 caveStrength = new Vector2(genRand.Next(10, 12), genRand.Next(13, 15));
            caveStrength *= 0.66f;

            //Chance to open up
            VeilGen.GenerateLongCurveCave(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
        }

        int numJungleTreeCaves = genRand.Next(126, 150);
        for (int n = 0; n < numJungleTreeCaves; n++)
        {
            int maxTreeAttempts = 20000;
            for (int a = 0; a < maxTreeAttempts; a++)
            {
                int originX = GenVars.jungleOriginX;
                int x = genRand.Next(originX - 1000, originX + 1000);
                int y = genRand.Next((int)GenVars.worldSurfaceLow - 25, Main.maxTilesY);
                if (x < 0 || x >= Main.maxTilesX)
                    continue;

                Tile tile = Main.tile[x, y];
                Point tilePoint = new Point(x, y);
                int rectWidth = 50;

                if (tilePoint.X - rectWidth > 0 &&
                    tilePoint.X + rectWidth < Main.maxTilesX &&
                    tilePoint.Y + rectWidth < Main.maxTilesY &&
                    tilePoint.Y - rectWidth > 0)
                {

                    Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
                    WorldUtils.Gen(tilePoint, new Shapes.Rectangle(50, 50), new Actions.TileScanner(
                        TileID.Mud).Output(dictionary));
                    int mudCount = dictionary[TileID.Mud];
                    int maxCount = 900;
                    float percent = mudCount / (float)maxCount;
                    if (percent < 0.75f)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                int caveWidth = genRand.Next(4, 7);
                int caveSteps = genRand.Next(80, 120);

                //Cave position in tiles
                Vector2 cavePosition = new Vector2(x, y);

                //Starting cave direction
                Vector2 baseCaveDirection = Vector2.UnitY;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

                //How much the tile runner is gonna carve out
                Vector2 caveStrength = new Vector2(genRand.Next(8, 10), genRand.Next(12, 15));

                //Chance to open up
                int splitDenominator = 128;
                VeilGen.GenerateTreeCaves(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps,
                    splitDenominator);
                break;
            }
        }
    }



    #endregion


    private void WorldGenDungeonLocation(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Moving the dungeon, smh";

        //GenVars.dungeonLocation is the x value of the dungeon

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
        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next(0, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Dirt)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.OwlTrunck1>());
            }
        }

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next(0, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Dirt)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.OwlTrunck2>());
            }
        }

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next(0, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Dirt)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.OwlTrunck3>());
            }
        }


        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Stone ||
                Main.tile[X, yBelow].TileType == TileID.ClayBlock)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.BigRock1>());
            }
        }

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Stone ||
                Main.tile[X, yBelow].TileType == TileID.ClayBlock)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.BigRock2>());
            }
        }

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Stone ||
                Main.tile[X, yBelow].TileType == TileID.ClayBlock)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.BigRock3>());
            }
        }

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Stone ||
                Main.tile[X, yBelow].TileType == TileID.ClayBlock)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.BigRock4>());
            }
        }

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Stone ||
                Main.tile[X, yBelow].TileType == TileID.ClayBlock)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.Stalagmite1>());
            }
        }

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Stone ||
                Main.tile[X, yBelow].TileType == TileID.ClayBlock)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.Stalagmite2>());
            }
        }

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Stone || Main.tile[X, yBelow].TileType == TileID.ClayBlock)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.Stalagmite3>());
            }
        }

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Stone || Main.tile[X, yBelow].TileType == TileID.ClayBlock)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.Mushroom3>());
            }
        }

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Stone || Main.tile[X, yBelow].TileType == TileID.ClayBlock)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.Mushroom2>());
            }
        }

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Stone ||
                Main.tile[X, yBelow].TileType == TileID.ClayBlock)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.Mushroom1>());
            }
        }

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Stone ||
                Main.tile[X, yBelow].TileType == TileID.ClayBlock)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.Stalagmite4>());
            }
        }

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
            int Y = WorldGen.genRand.Next(0, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Mud ||
                Main.tile[X, yBelow].TileType == TileID.JungleGrass)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Structures.LogS>());
            }
        }
        //

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY / 2);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Dirt)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.TreeOver1>());
            }
        }

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next(0, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Dirt)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.TreeOver2>());

            }
        }

        //Purple Tree

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
            int Y = WorldGen.genRand.Next(0, Main.UnderworldLayer);
            int yBelow = Y + 1;
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.Dirt)
            {
                WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.TreeOver3>());

            }
        }



    }

    Point pointAlcadthingy;
    private float GetFableHillHeight(float x)
    {
        float bump = x * (4 - x * 4);
        float mountains = MathF.Sin(x * 1) * 0.5f + 0.5f;
        float mountains2 = MathF.Sin(x * 1) * 0.5f + 0.7f;
        float dips = MathF.Sin(x * 16) * 0.1f;
        float roughness = MathF.Sin(x * 76) * 0.01f;
        float roughness2 = MathF.Sin(x * 101) * 0.005f;
        float y = bump * mountains * mountains2 - dips - roughness - roughness2;
        return y + 0.1f;
    }

    public void WorldGenMistyDungeonHill(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "A Mysterious Hill...";

        //Calculate the starting location
        Point startHillTile = FableFarEdgeLocation;
        startHillTile.X += 150;
        startHillTile.Y -= 200;
        startHillTile = FallToSolidTile(startHillTile.X, startHillTile.Y);
        startHillTile.Y += 36;
        MistyHillStartLocation = startHillTile;

        //Calculate the ending location
        Point endHillTile = startHillTile;
        endHillTile.X += 2200;
        endHillTile.Y -= 200;
        endHillTile = FallToSolidTile(endHillTile.X, endHillTile.Y);
        endHillTile.Y += 10;
        MistyHillEndLocation = endHillTile;

        float hillHeight = 350;
        float width = endHillTile.X - startHillTile.X;
        for (int x = startHillTile.X; x < endHillTile.X; x++)
        {
            float ratio = (x - startHillTile.X) / width;
            float height = (int)(GetFableHillHeight(ratio) * hillHeight);
            for (int y = 0; y < height; y++)
            {
                WorldGen.PlaceTile(x, startHillTile.Y - y, TileID.Dirt);
            }
        }

        //Place the fable
        Point placementTile = new Point();
        placementTile.X = (int)MathHelper.Lerp(startHillTile.X, endHillTile.X, 0.65f);
        placementTile.Y = (int)(Main.worldSurface - 400);
        placementTile = FallToSolidTile(placementTile.X, placementTile.Y);
        MistyDungeonLocation = placementTile;
    }



    public void WorldGenFableTerrain(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Fable Terrain";
        //Calculate the starting location
        Point startHillTile = WitchTownLocation;
        startHillTile.X += 300;
        startHillTile.Y -= 200;
        startHillTile = FallToSolidTile(startHillTile.X, startHillTile.Y);
        startHillTile.Y += 36;
        FableHillStartLocation = startHillTile;

        //Calculate the ending location
        Point endHillTile = startHillTile;
        endHillTile.X += 1000;
        endHillTile.Y -= 200;
        endHillTile = FallToSolidTile(endHillTile.X, endHillTile.Y);
        endHillTile.Y += 10;
        FableHillEndLocation = endHillTile;

        float hillHeight = 200;
        float width = endHillTile.X - startHillTile.X;
        for (int x = startHillTile.X; x < endHillTile.X; x++)
        {
            float ratio = (x - startHillTile.X) / width;
            float height = (int)(GetFableHillHeight(ratio) * hillHeight);
            for (int y = 0; y < height; y++)
            {
                WorldGen.PlaceTile(x, startHillTile.Y - y, TileID.Dirt);
                //  WorldGen.PlaceTile(x, y, TileID.Dirt);
            }
        }
        //  WorldGen

        //Place the fable
        Point placementTile = new Point();
        placementTile.X = (int)MathHelper.Lerp(startHillTile.X, endHillTile.X, 0.6f);
        placementTile.Y = (int)(Main.worldSurface - 400);
        placementTile = FallToSolidTile(placementTile.X, placementTile.Y);
        placementTile += new Point(10, 53);

        FableLocation = placementTile;




        //Placing a falling off slope at the end of the structure
        Rectangle fableRect = Structurizer.ReadRectangle(StructureAssets.Fable);
        Point fableFalloffStart = FableLocation + new Point(fableRect.Width, 0);
        fableFalloffStart.Y -= 54;
        fableFalloffStart.X -= 20;

        Point fableFalloffEnd = fableFalloffStart;
        fableFalloffEnd.X += 150;
        fableFalloffEnd = FallToSolidTile(fableFalloffEnd.X, fableFalloffEnd.Y);
        fableFalloffEnd.Y += 10;

        width = fableFalloffEnd.X - fableFalloffStart.X;
        for (int x = fableFalloffStart.X; x < fableFalloffEnd.X; x++)
        {
            float ratio = (x - fableFalloffStart.X) / width;
            int startY = (int)MathHelper.SmoothStep(fableFalloffStart.Y, fableFalloffEnd.Y, ratio);
            Point tilePlace = new Point(x, startY);
            for (int y = startY; y < fableFalloffEnd.Y; y++)
            {
                WorldGen.PlaceTile(tilePlace.X, y, TileID.Dirt);
            }
        }

        FableFarEdgeLocation = fableFalloffEnd;

    }
    public void WorldGenFabiliaRuin(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Daedus is Reading Books...";
        Structurizer.PlaceAndProtect(new StructurePlacementParams
        {
            tile = FableLocation,
            structurePath = StructureAssets.Fable,
            tileBlend = Structurizer.DefaultTileBlend
        });

        //Placing a falling off slope at the end of the structure
        Rectangle fableRect = Structurizer.ReadRectangle(StructureAssets.Fable);
        Point fableFalloffStart = FableLocation + new Point(fableRect.Width, 0);
        fableFalloffStart.Y -= 54;
        fableFalloffStart.X -= 20;

        Point fableFalloffEnd = fableFalloffStart;
        fableFalloffEnd.X += 150;
        fableFalloffEnd = FallToSolidTile(fableFalloffEnd.X, fableFalloffEnd.Y);
        fableFalloffEnd.Y += 10;

        float width = fableFalloffEnd.X - fableFalloffStart.X;
        for (int x = fableFalloffStart.X; x < fableFalloffEnd.X; x++)
        {
            float ratio = (x - fableFalloffStart.X) / width;
            int startY = (int)MathHelper.SmoothStep(fableFalloffStart.Y, fableFalloffEnd.Y, ratio);
            Point tilePlace = new Point(x, startY);
            for (int y = startY; y < fableFalloffEnd.Y; y++)
            {
                WorldGen.PlaceTile(tilePlace.X, y, TileID.Dirt);
                // WorldGen.PlaceTile(tilePlace.X, y, TileID.Dirt);
            }
        }

        FableFarEdgeLocation = fableFalloffEnd;

        /*
Point startCaveTile = new Point();
startCaveTile.X = (int)MathHelper.Lerp(FableHillStartLocation.X, FableHillEndLocation.X, 0.2f);
startCaveTile.Y = (int)(Main.worldSurface - 400);

Point endCaveTile = new Point();
endCaveTile.X = (int)MathHelper.Lerp(FableHillStartLocation.X, FableHillEndLocation.X, 0.4f);
endCaveTile.Y = (int)(Main.worldSurface - 400);

startCaveTile = FallToSolidTile(startCaveTile.X, startCaveTile.Y);
endCaveTile = FallToSolidTile(endCaveTile.X, endCaveTile.Y);


width = endCaveTile.X - startCaveTile.X;
float maxCaveDepth = 66;
var genRand = WorldGen.genRand;
Vector2 caveStrength = new Vector2(15, 20);

for (int x = startCaveTile.X; x < endCaveTile.X; x++)
{
    float ratio = (x - startCaveTile.X) / width;
    float bump = EasingFunction.QuadraticBump(ratio);
    int y = (int)MathHelper.Lerp(startCaveTile.Y, endCaveTile.Y, ratio);
    y += (int)MathHelper.Lerp(0, maxCaveDepth, bump);

    WorldGen.TileRunner(x, y,
        genRand.NextFloat(caveStrength.X, caveStrength.Y),
        genRand.Next(12, 30), -1);
}


//Place Telegrim
//Place DELGRIM
Point delgrimPoint = new Point();
delgrimPoint.X = (int)MathHelper.Lerp(startCaveTile.X, endCaveTile.X, 0.5f);
delgrimPoint.Y = (int)MathHelper.Lerp(startCaveTile.Y, endCaveTile.Y, 0.5f) + (int)MathHelper.Lerp(0, maxCaveDepth, EasingFunction.QuadraticBump(0.5f)); ;

string structure = "Struct/Underground/DelgrimShop";
Point pointToPlaceDelgrimShop = delgrimPoint;
while (!Structurizer.TryPlaceAndProtectStructure(pointToPlaceDelgrimShop, structure))
{
    pointToPlaceDelgrimShop += genRand.NextVector2Circular(4, 4).ToPoint();
}

Structurizer.ReadStruct(pointToPlaceDelgrimShop, structure);
Rectangle structureRectangle = Structurizer.ReadRectangle(structure);
structureRectangle.Location = pointToPlaceDelgrimShop;
for (int beamX = structureRectangle.Location.X;
    beamX < structureRectangle.Location.X + structureRectangle.Width; beamX += 4)
{
    int beamY = structureRectangle.Location.Y;
    int solidCount = 0;
    while (solidCount < 5)
    {
        if (!WorldGen.SolidTile(beamX, beamY))
        {
            WorldGen.PlaceTile(beamX, beamY, TileID.WoodenBeam);
        }
        else
        {
            solidCount++;
        }
        beamY++;
    }
}*/
    }




    #region Manor N Cinderpark

    private void WorldGenCinderspark(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Searing the deepest caverns";
        ushort dirtTile = (ushort)ModContent.TileType<CindersparkDirt>();
        var genRand = WorldGen.genRand;

        CindersparkStart = Main.maxTilesY - 10;
        CindersparkEnd = 0;
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            int yMax = (Main.UnderworldLayer - (Main.maxTilesY / 20));
            int yMin = yMax - 150;

            CindersparkStart = Math.Min(CindersparkStart, yMin);
            CindersparkEnd = Math.Max(CindersparkEnd, yMax);


            float ratio = x / (float)Main.maxTilesX;

            float y = yMin;
            y += MathF.Sin(ratio * 64) * 10;
            y += MathF.Sin(ratio * 64) * 4;
            int startY = (int)y;
            int endY = startY;
            // We go down until we hit a solid tile or go under the world's surface
            while (endY <= Main.UnderworldLayer)
            {
                endY++;
            }


            for (int j = startY; j < endY; j++)
            {
                Tile t = Main.tile[x, j];
                t.ClearEverything();
                t.TileType = dirtTile;
                t.HasTile = true;
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
        while (!placed && attempts++ < 10000000)
        {
            // Select a place in the first 6th of the world, avoiding the oceans
            int smx = WorldGen.genRand.Next(((Main.maxTilesX) / 2) - 200, (Main.maxTilesX / 2) + 50); // from 50 since there's a unaccessible area at the world's borders
                                                                                                      // 50% of choosing the last 6th of the world
                                                                                                      // Choose which side of the world to be on randomly
            ///if (WorldGen.genRand.NextBool())
            ///{
            ///	towerX = Main.maxTilesX - towerX;
            ///}

            //Start at 200 tiles above the surface instead of 0, to exclude floating islands
            int smy = Main.UnderworldLayer - 400;

            // We go down until we hit a solid tile or go under the world's surface
            Tile tile = Main.tile[smx, smy];

            while (!WorldGen.SolidTile(smx, smy) && smy <= Main.UnderworldLayer && (!(tile.TileType == ModContent.TileType<CindersparkDirt>())))
            {
                smy++;
                tile = Main.tile[smx, smy];
            }

            // If we went under the world's surface, try again
            if (smy > Main.UnderworldLayer - 20)
            {
                continue;
            }

            // If the type of the tile we are placing the tower on doesn't match what we want, try again



            // place the Rogue
            //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
            //Main.npc[num].homeTileX = -1;
            //	Main.npc[num].homeTileY = -1;
            //	Main.npc[num].direction = 1;
            //	Main.npc[num].homeless = true;



            for (int da = 0; da < 1; da++)
            {
                Point Loc = new Point(smx, smy + 350);
                Point Loc2 = new Point(smx, smy + 100);
                //StructureLoader.ReadStruct(Loc, "Struct/Underground/Manor", tileBlend);
                string path = "Struct/Underground/Manor";//

                int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path, tileBlend);
                ManorLocation = Loc;
                StructureLoader.ProtectStructure(Loc, path);
                foreach (int chestIndex in ChestIndexs)
                {
                    if (chestIndex >= Main.chest.Length)
                        continue;

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
                            itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner5>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                            //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                            break;
                        case 1:
                            itemsToAdd.Add((ModContent.ItemType<Volcant>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianBar>(), Main.rand.Next(1, 10)));
                            itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner5>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                            //   itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                            break;
                        case 2:
                            itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                            //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                            break;
                        case 3:
                            itemsToAdd.Add((ModContent.ItemType<CinderNeedle>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                            itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner5>(), Main.rand.Next(1, 1)));
                            // itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
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


                GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));
                //WorldGen.TileRunner(Loc2.X - 10, Loc2.Y - 60, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(120, 120), TileID.Grass);
                //WorldGen.TileRunner(Loc3.X - 20, Loc2.Y, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);
                //WorldGen.TileRunner(Loc3.X - 20, Loc3.Y + 20, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);


                /*
					string path2 = "Struct/Underground/Ishtar";//
					int[] ChestIndexs2 = StructureLoader.ReadStruct(Loc2, path2, tileBlend2);
					NPCs.Town.AlcadSpawnSystem.IshPinTile = Loc2;
					NPCs.Town.AlcadSpawnSystem.EreshTile = Loc2;
					NPCs.Town.AlcadSpawnSystem.PULSETile = Loc2;

					StructureLoader.ProtectStructure(Loc2, path2);
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


					*/




                placed = true;
            }



        }

    }


    #endregion

    #region Small Surface Structures

    private void AddChestLoot(Chest chest, List<(int type, int stack)> itemsToAdd)
    {
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

    private void GenerateFallingWoodenBeams(Rectangle structureRectangle, Point Loc)
    {
        structureRectangle.Location = Loc;
        for (int beamX = structureRectangle.Location.X;
            beamX < structureRectangle.Location.X + structureRectangle.Width; beamX += 4)
        {
            int beamY = structureRectangle.Location.Y;
            int solidCount = 0;
            while (solidCount < 5)
            {
                if (!WorldGen.SolidTile(beamX, beamY))
                {
                    WorldGen.PlaceTile(beamX, beamY, TileID.WoodenBeam);
                }
                else
                {
                    solidCount++;
                }
                beamY++;
            }
        }
    }
    private void GenerateFallingWoodenBeams(Rectangle structureRectangle, Point Loc, int onTileType)
    {
        //Need to substract the height of the rectangle here because of how we place structures
        //They place from the bottom left.
        structureRectangle.Location = Loc - new Point(0, structureRectangle.Height);
        List<Point> tilesToFallFrom = new List<Point>();
        for (int x = structureRectangle.Location.X;
          x < structureRectangle.Location.X + structureRectangle.Width; x++)
        {
            for (int y = structureRectangle.Location.Y; y < structureRectangle.Location.Y + structureRectangle.Height; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.HasTile && tile.TileType == onTileType)
                {
                    tilesToFallFrom.Add(new Point(x, y));
                }
            }
        }

        foreach (var point in tilesToFallFrom)
        {
            int beamX = point.X;
            int beamY = point.Y;
            int solidCount = 0;
            while (solidCount < 5)
            {
                if (!WorldGen.SolidTile(beamX, beamY))
                {
                    WorldGen.PlaceTile(beamX, beamY, TileID.WoodenBeam);
                }
                else
                {
                    solidCount++;
                }
                beamY++;
            }
        }
    }
    private void WorldGenRysaHouse(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Rysa is Moving In!";
        bool placed = false;
        int attempts = 0;
        var genRand = WorldGen.genRand;
        int[] tileBlend = new int[]
        {
            TileID.RubyGemspark
        };
        Point rysaHousePoint = new Point();
        rysaHousePoint = FableFarEdgeLocation;
        rysaHousePoint.X += 400;
        rysaHousePoint.Y -= 300;
        rysaHousePoint = FallToSolidTile(rysaHousePoint.X, rysaHousePoint.Y);
        while (!placed && attempts++ < 10000000)
        {
            string structure = "Structures/Rysahouse";
            Rectangle rectangle = Structurizer.ReadRectangle(structure);
            int[] ChestIndexs = Structurizer.ReadStruct(rysaHousePoint, structure, tileBlend);
            GenerateFallingWoodenBeams(rectangle, rysaHousePoint);

            foreach (int chestIndex in ChestIndexs)
            {
                if (chestIndex == -1)
                    continue;
                var chest = Main.chest[chestIndex];
                var itemsToAdd = new List<(int type, int stack)>();
                itemsToAdd.Add((ModContent.ItemType<ZuisGiftedWand>(), 1));
                AddChestLoot(chest, itemsToAdd);
            }

            placed = true;
        }


        Point gilatineHousePoint = new Point();
        gilatineHousePoint = FableFarEdgeLocation;
        gilatineHousePoint.X += 800;
        gilatineHousePoint.Y -= 330;
        gilatineHousePoint = FallToSolidTile(gilatineHousePoint.X, gilatineHousePoint.Y);


        string path = "Structures/GilatineCave";
        gilatineHousePoint.X -= 80;
        gilatineHousePoint.Y += 300;

        gilatineHousePoint.X -= 7;
        gilatineHousePoint.Y += 7;
        gilatineHousePoint.X -= 25;
        Structurizer.ReadStruct(gilatineHousePoint, path, tileBlend);
        Structurizer.ProtectStructure(gilatineHousePoint, path);
        /*
        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab("GilatineCave");
        prefab.PasteErase(gilatineHousePoint.X, gilatineHousePoint.Y, new Point(55, 0));*/
        progress.Message = "I'm Racist.";
    }

    public void WorldGenStoneGolemCave(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Stone Golem Cave";

        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 10000000)
        {
            // Select a place in the first 6th of the world, avoiding the oceans
            int smx = WitchTownLocation.X;
            smx -= 500;

            //Start at 200 tiles above the surface instead of 0, to exclude floating islands
            int smy = ((int)(Main.worldSurface - 300));

            // We go down until we hit a solid tile or go under the world's surface
            while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
            {
                smy++;
            }
            smy += 45;
            Point Loc = new Point(smx, smy + 15);
            Point Loc22 = new Point(smx, smy + 58);
            string path = "Structures/Overworld/StoneGolemCave";


            var stoneGolemCaveRectangle = Structurizer.ReadRectangle(path);
            int[] ChestIndexs = Structurizer.ReadStruct(Loc, path, null);
            Structurizer.ProtectStructure(Loc, path);
            placed = true;


            //Set the default spawn point of the world
            Point spawnLocation = Loc;
            spawnLocation.X += 92;
            spawnLocation.Y -= 44;
            Main.spawnTileX = spawnLocation.X;
            Main.spawnTileY = spawnLocation.Y;

            //Place the Training Grounds
            string trainingGroundsPath = "Structures/TrainingGrounds";
            Point trainingGroundsSpawnPoint = Loc - new Point(0, stoneGolemCaveRectangle.Height);
            ChestIndexs = Structurizer.ReadStruct(trainingGroundsSpawnPoint, trainingGroundsPath, null);
            Structurizer.ProtectStructure(trainingGroundsSpawnPoint, path);

            //Place the Jiitas Bridge
            string jiitasPath = "Structures/TrainingbridgeJiitas";
            var jiitasRectangle = Structurizer.ReadRectangle(jiitasPath);


            Point jiitasSpawnPoint = trainingGroundsSpawnPoint - new Point(jiitasRectangle.Width, 0);

            //Offset it down by 10 tiles so it's level with the training ground
            jiitasSpawnPoint.Y += 10;

            ChestIndexs = Structurizer.ReadStruct(jiitasSpawnPoint, jiitasPath, null);
            Structurizer.ProtectStructure(jiitasSpawnPoint, path);
            GenerateFallingWoodenBeams(jiitasRectangle, jiitasSpawnPoint, TileID.BoneBlock);
        }
    }

    private void WriteLine(string? value)
    {
        Console.WriteLine(value);
    }
    public void SetXixVillageLocation(GenerationProgress progress, GameConfiguration configuration)
    {
        Stopwatch sw = Stopwatch.StartNew();
        progress.Message = "Set Xix Village";
        string path = "Structures/WitchTown";
        var rectangle = Structurizer.ReadRectangle(path);
        // int yOffset = Structurizer.OffsetToGround(path);
        //  Mod.Logger.Debug($"Witch Town Offset to Ground {yOffset}");

        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 10000)
        {
            int centerX = Main.maxTilesX / 2;
            int maxRange = attempts;
            maxRange = Math.Min(1000, maxRange);
            int smx = WorldGen.genRand.Next(centerX - maxRange, centerX + maxRange);
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

            //We're checking for surrounding dirt and grass so it doesn't place near ice or desert biomes
            //Rectangles are placed from the bottom left, so subtract half the width to check tiles evenly on both sides
            int width = rectangle.Width * 2;
            Point point = new Point(smx - width / 2, smy + 50);
            Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
            WorldUtils.Gen(point, new Shapes.Rectangle(width, rectangle.Height), new Actions.TileScanner(TileID.Dirt, TileID.Stone).Output(dictionary));
            int stoneAndDirtCount = dictionary[TileID.Dirt] + dictionary[TileID.Stone];
            // 20 * 10 == 200. This is checking that at least 75% of the area is Stone or Dirt
            if (stoneAndDirtCount < 10000)
                continue;

            //Check if sand or snow
            width = rectangle.Width * 4;
            point = new Point(smx - width / 2, smy + 50);
            Dictionary<ushort, int> dictionary2 = new Dictionary<ushort, int>();
            WorldUtils.Gen(point, new Shapes.Rectangle(width, rectangle.Height), new Actions.TileScanner(TileID.Sand, TileID.SnowBlock).Output(dictionary2));
            int sandAndSnow = dictionary2[TileID.Sand] + dictionary2[TileID.SnowBlock];
            if (sandAndSnow >= 1)
                continue;


            Point Loc = new Point(smx, smy + 57);
            WitchTownLocation = Loc;
            break;
        }
        sw.Stop();
        WriteLine($"Witch Town Location Generation Time {sw.ElapsedMilliseconds}ms");
        WriteLine($"Witch Town Location: {WitchTownLocation}");
    }
    public void WorldGenXixVillage(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Witches spreading love all inside you!";
        string path = "Structures/WitchTown";
        var rectangle = Structurizer.ReadRectangle(path);
        var tileBlend = new int[]
        {
            TileID.RubyGemspark
        };

        int[] ChestIndexs = Structurizer.ReadStruct(WitchTownLocation, path, tileBlend);
        Structurizer.ProtectStructure(WitchTownLocation, path);
        for (int x = WitchTownLocation.X; x < WitchTownLocation.X + rectangle.Width; x++)
        {
            for (int y = WitchTownLocation.Y; y < WitchTownLocation.Y + 40; y++)
            {
                if (!WorldGen.SolidTile(x, y))
                {
                    WorldGen.PlaceTile(x, y, TileID.Dirt);
                }
            }
        }

        foreach (int chestIndex in ChestIndexs)
        {
            var chest = Main.chest[chestIndex];

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
                    // itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                    itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                    itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                    break;
                case 1:
                    itemsToAdd.Add((ItemID.CordageGuide, Main.rand.Next(1, 1)));
                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                    //    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                    itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                    break;
                case 2:
                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                    //   itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                    itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 50)));
                    itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                    break;
                case 3:
                    itemsToAdd.Add((ItemID.SlimeStaff, Main.rand.Next(1, 1)));
                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                    // itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
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

        path = "Structures/DelgrimHill";
        Point delgrimHillPoint = FableHillStartLocation;
        delgrimHillPoint.X += 130;
        delgrimHillPoint.Y -= 10;
        Structurizer.ReadStruct(delgrimHillPoint, path, tileBlend);
        Structurizer.ProtectStructure(delgrimHillPoint, path);

        path = "Structures/EveroseVillage";
        Point everosePoint = FableHillEndLocation;
        everosePoint.X += 8;
        everosePoint = FallToSolidTile(everosePoint);
        everosePoint.Y += 19;
        Structurizer.ReadStruct(everosePoint, path, tileBlend);
        Structurizer.ProtectStructure(everosePoint, path);
    }

    private Point FallToSolidTile(Point tile)
    {
        return FallToSolidTile(tile.X, tile.Y);
    }
    private Point FallToSolidTile(int x, int y)
    {
        Point start = new Point(x, y);
        Point current = start;
        for (int i = 0; i < Main.maxTilesY; i++)
        {

            if (WorldGen.InWorld(current.X, current.Y) && WorldGen.SolidTile(current.X, current.Y))
                return current;
            current.Y += 1;
        }
        return Point.Zero;
    }

    private void WorldGenVeizalManor(GenerationProgress progress, GameConfiguration configuration)
    {
        StructureMap structures = GenVars.structures;
        string structure = "Struct/Overworld/VeizalManor";
        Rectangle rectangle = Structurizer.ReadRectangle(structure);
        progress.Message = "WE'RE RICH!";
        int[] tileBlend = new int[]
        {
            TileID.RubyGemspark
        };
        int maxAttemptCount = 10000;
        for (int a = 0; a < maxAttemptCount; a++)
        {
            // Select a place in the first 6th of the world, avoiding the oceans
            int x = GenVars.snowOriginRight + WorldGen.genRand.Next(0, 300);
            int y = (int)(Main.worldSurface - 200);
            Point tileToPlaceOn = FallToSolidTile(x, y);
            int cathedralY = tileToPlaceOn.Y;

            //Start at 200 tiles above the surface instead of 0, to exclude floating islands
            Point Loc = tileToPlaceOn;
            if (!Structurizer.TryPlaceAndProtectStructure(Loc, structure))
                continue;
            Structurizer.ReadStruct(Loc, structure, tileBlend);
            Rectangle structureRectangle = Structurizer.ReadRectangle(structure);
            structureRectangle.Location = Loc;
            for (int beamX = structureRectangle.Location.X;
                beamX < structureRectangle.Location.X + structureRectangle.Width; beamX += 4)
            {
                //Place beams
                int beamY = structureRectangle.Location.Y;
                Tile tile = Main.tile[beamX, beamY];
                int solidCount = 0;
                while (solidCount < 5)
                {
                    if (!WorldGen.SolidTile(beamX, beamY))
                    {
                        WorldGen.PlaceTile(beamX, beamY, TileID.BorealBeam);
                    }
                    else
                    {
                        solidCount++;
                    }
                    beamY++;
                }
            }
            break;
        }

        //Place verlia cathedral
        /*
        structure = "Struct/Overworld/VerliaBridge";
        for (int a = 0; a < maxAttemptCount; a++)
        {
            int x = manorX + genRand.Next(-350, -150);
            int y = ((int)(Main.worldSurface - 200));

            // We go down until we hit a solid tile or go under the world's surface
            while (!WorldGen.SolidTile(x, y) && y <= Main.worldSurface)
            {
                y++;
            }

            Tile tileToPlaceOn = Main.tile[x, y];
            if (tileToPlaceOn.TileType != TileID.SnowBlock && tileToPlaceOn.TileType != TileID.IceBlock)
                continue;

            int cathedralY = y;

            //Start at 200 tiles above the surface instead of 0, to exclude floating islands
            Point Loc = new Point(x, cathedralY);
            if (!Structurizer.TryPlaceAndProtectStructure(Loc, structure))
                continue;
            Structurizer.ReadStruct(Loc, structure, tileBlend);
            Rectangle structureRectangle = Structurizer.ReadRectangle(structure);
            structureRectangle.Location = Loc;
            for (int beamX = structureRectangle.Location.X;
                beamX < structureRectangle.Location.X + structureRectangle.Width; beamX += 4)
            {
                //Place beams
                int beamY = structureRectangle.Location.Y;
                Tile tile = Main.tile[beamX, beamY];
                int solidCount = 0;
                while (solidCount < 5)
                {
                    if (!WorldGen.SolidTile(beamX, beamY))
                    {
                        WorldGen.PlaceTile(beamX, beamY, TileID.Titanstone);
                    }
                    else
                    {
                        solidCount++;
                    }
                    beamY++;
                }
            }

            break;
        }*/
    }


    private void WorldGenBloodCathedral(GenerationProgress progress, GameConfiguration configuration)
    {
        StructureMap structures = GenVars.structures;
        string structure = "Struct/Overworld/BloodCathedral";
        Rectangle rectangle = Structurizer.ReadRectangle(structure);
        progress.Message = "Building a Bloody Cathedral";

        int[] tileBlend = new int[]
        {
            TileID.RubyGemspark
        };

        int totalX = 0;
        int numX = 0;

        int minJungleX = 0;
        int maxJungleX = 0;
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            int y = (int)Main.worldSurface - 50;
            while (y <= Main.worldSurface)
            {
                y++;
                if (WorldGen.SolidTile(x, y) && Main.tile[x, y].TileType == TileID.Mud)
                {
                    if (numX == 0)
                    {
                        minJungleX = x;
                    }
                    maxJungleX = x;
                    numX++;
                    totalX += x;
                    break;
                }
            }
        }
        int jungleX = totalX / numX;
        int maxAttemptCount = 10000;
        var genRand = WorldGen.genRand;
        for (int a = 0; a < maxAttemptCount; a++)
        {
            // Select a place in the first 6th of the world, avoiding the oceans
            int cathedralX = jungleX;
            cathedralX += 220 + genRand.Next(0, 50);
            int y = ((int)(Main.worldSurface - 200));

            // We go down until we hit a solid tile or go under the world's surface
            while (!WorldGen.SolidTile(cathedralX, y) && y <= Main.worldSurface)
            {
                y++;
            }

            int cathedralY = y - 150;

            //Start at 200 tiles above the surface instead of 0, to exclude floating islands
            Point Loc = new Point(cathedralX, cathedralY);
            if (!Structurizer.TryPlaceAndProtectStructure(Loc, structure))
                continue;
            Structurizer.ReadStruct(Loc, structure, tileBlend);
            break;
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
                            //  itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.CordageGuide, Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            //   itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                            break;
                        case 2:
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            //  itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 50)));
                            itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                            break;
                        case 3:
                            itemsToAdd.Add((ItemID.SlimeStaff, Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            //   itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
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
        bool placed = false;
        int attempts = 0;
        var genRand = WorldGen.genRand;
        int[] tileBlend = new int[]
        {
            TileID.RubyGemspark
        };

        Point windmillPlacementTile = new Point();
        windmillPlacementTile.X = (int)MathHelper.Lerp(MistyHillStartLocation.X, MistyHillEndLocation.X, 0.45f);
        windmillPlacementTile.Y = (int)(Main.worldSurface - 1200);
        windmillPlacementTile = FallToSolidTile(windmillPlacementTile.X, windmillPlacementTile.Y);
        while (!placed && attempts++ < 10000000)
        {
            string structure = "Struct/Overworld/Windmill";
            int[] ChestIndexs = Structurizer.ReadStruct(windmillPlacementTile, structure, tileBlend);
            Rectangle structureRectangle = Structurizer.ReadRectangle(structure);
            structureRectangle.Location = windmillPlacementTile;
            for (int beamX = structureRectangle.Location.X;
                beamX < structureRectangle.Location.X + structureRectangle.Width; beamX += 4)
            {
                int beamY = structureRectangle.Location.Y;
                int solidCount = 0;
                while (solidCount < 5)
                {
                    if (!WorldGen.SolidTile(beamX, beamY))
                    {
                        WorldGen.PlaceTile(beamX, beamY, TileID.WoodenBeam);
                    }
                    else
                    {
                        solidCount++;
                    }
                    beamY++;
                }
            }

            foreach (int chestIndex in ChestIndexs)
            {
                if (chestIndex == -1)
                    continue;
                var chest = Main.chest[chestIndex];
                var itemsToAdd = new List<(int type, int stack)>();

                // Using a switch statement and a random choice to add sets of items.
                switch (Main.rand.Next(4))
                {
                    case 0:
                        itemsToAdd.Add((ModContent.ItemType<WindmillShuriken>(), genRand.Next(1, 1)));
                        break;
                    case 1:
                        itemsToAdd.Add((ModContent.ItemType<WindmillionRobe>(), genRand.Next(1, 1)));
                        itemsToAdd.Add((ModContent.ItemType<WindmillionHat>(), genRand.Next(1, 1)));
                        itemsToAdd.Add((ModContent.ItemType<WindmillionBoots>(), genRand.Next(1, 1)));
                        break;

                    case 2:
                    
                        break;

                    case 3:
                        itemsToAdd.Add((ItemID.BabyBirdStaff, genRand.Next(1, 1)));
                        break;
                }

                itemsToAdd.Add((ItemID.IronOre, genRand.Next(9, 15)));
                if (genRand.NextBool(2))
                {
                    itemsToAdd.Add((ItemID.EndurancePotion, genRand.Next(1, 3)));
                    itemsToAdd.Add((ItemID.WormholePotion, genRand.Next(1, 2)));
                }
                else
                {
                    itemsToAdd.Add((ItemID.SwiftnessPotion, genRand.Next(1, 3)));
                    itemsToAdd.Add((ItemID.WormholePotion, genRand.Next(1, 2)));
                    itemsToAdd.Add((ItemID.SpelunkerPotion, genRand.Next(1, 3)));
                }

                itemsToAdd.Add((ModContent.ItemType<Ivythorn>(), genRand.Next(3, 5)));
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
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(9))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<Gutinier>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                //  itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.WandofFrosting, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                //  itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;
                            case 2:
                                itemsToAdd.Add((ItemID.EndlessQuiver, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                //  itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;
                            case 3:
                                itemsToAdd.Add((ItemID.SlimeStaff, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                //   itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));

                                break;
                            case 4:
                                itemsToAdd.Add((ItemID.Diamond, Main.rand.Next(1, 20)));
                                itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                // itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;

                            case 5:
                                itemsToAdd.Add((ItemID.CloudinaBottle, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                //   itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;

                            case 6:
                                itemsToAdd.Add((ItemID.ShinyRedBalloon, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                //  itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;

                            case 7:
                                itemsToAdd.Add((ItemID.BandofRegeneration, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                //   itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;

                            case 8:
                                itemsToAdd.Add((ItemID.BandofStarpower, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                // itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
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
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(11))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<Gutinier>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                //   itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.WandofFrosting, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                //  itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;
                            case 2:
                                itemsToAdd.Add((ItemID.EndlessQuiver, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                //  itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;
                            case 3:
                                itemsToAdd.Add((ItemID.SlimeStaff, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                //   itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));

                                break;
                            case 4:
                                itemsToAdd.Add((ItemID.Diamond, Main.rand.Next(1, 20)));
                                itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                //    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;

                            case 5:
                                //   itemsToAdd.Add((ModContent.ItemType<IronCrossbow>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                //   itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;

                            case 6:
                                //itemsToAdd.Add((ModContent.ItemType<EaglesGrace>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                //  itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;

                            case 7:
                                itemsToAdd.Add((ItemID.ShinyRedBalloon, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                // itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;

                            case 8:
                                itemsToAdd.Add((ItemID.BandofRegeneration, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                //   itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;

                            case 9:
                                itemsToAdd.Add((ItemID.BandofStarpower, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                //  itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;

                            case 10:
                                itemsToAdd.Add((ItemID.PlatinumBar, Main.rand.Next(1, 20)));
                                itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
                                //    itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
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

    private void WorldGenColeseum(GenerationProgress progress, GameConfiguration configuration)
    {
        StructureMap structures = GenVars.structures;
        Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Acid/GiaHouse");
        progress.Message = "Commanders having fun in their village";


        for (int k = 0; k < 1; k++)
        {
            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 1000000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int smx = (GenVars.desertHiveLeft + GenVars.desertHiveRight) / 2; // from 50 since there's a unaccessible area at the world's borders
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

                /*if (!(tile.TileType == ModContent.TileType<AcidialDirt>()))
                {
                    continue;
                }
					*/



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
                //int smx = (GenVars.desertHiveLeft + GenVars.desertHiveRight) / 2; // from 50 since there's a unaccessible area at the world's borders
                // 50% of choosing the last 6th of the world

                int smx = WorldGen.genRand.Next(250, (Main.maxTilesX) - 250);     // Choose which side of the world to be on randomly
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

                }

                placed = true;
            }
        }

    }

    #endregion


    #region Virulent N Govheil


    public void WorldGenVirulent(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Virulifying the Morrow";

        int totalX = 0;
        int numX = 0;
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            int y = (int)Main.worldSurface - 50;
            while (y <= Main.worldSurface)
            {
                y++;
                if (WorldGen.SolidTile(x, y) && Main.tile[x, y].TileType == TileID.Mud)
                {
                    numX++;
                    totalX += x;
                    break;

                }

            }

        }
        int jungleX = totalX / numX;
        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 100000)
        {
            // Select a place in the first 6th of the world, avoiding the oceans
            int abysmx = jungleX; // from 50 since there's a unaccessible area at the world's borders

            //Start at 200 tiles above the surface instead of 0, to exclude floating islands
            int abysmy = (int)(Main.worldSurface - 50);

            // We go down until we hit a solid tile or go under the world's surface
            while (!WorldGen.SolidTile(abysmx, abysmy) && abysmy <= Main.worldSurface)
            {
                abysmy++;
            }


            for (int da = 0; da < 1; da++)
            {
                Point Loc7 = new Point(abysmx, abysmy);
                WorldGen.TileRunner(Loc7.X, Loc7.Y, 500, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), false, 0f, 0f, true, true);
                WorldGen.TileRunner(Loc7.X, Loc7.Y + 200, 600, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), false, 0f, 0f, true, true);
                WorldGen.TileRunner(Loc7.X, Loc7.Y + 400, 600, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), true, 0f, 0f, true, true);
                WorldGen.TileRunner(Loc7.X, Loc7.Y + 600, 700, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), true, 0f, 0f, true, true);
                WorldGen.TileRunner(Loc7.X, Loc7.Y + 800, 700, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), true, 0f, 0f, true, true);
                WorldGen.TileRunner(Loc7.X, Loc7.Y + 1000, 700, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), true, 0f, 0f, true, true);

                pointL = new Point(abysmx, abysmy + 255);
                WorldGen.DirtyRockRunner(0, Main.maxTilesX - 50);
                placed = true;
            }
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
                string path = "Struct/Huntria/Govheil2";

                Point pointToPlaceOn = pointL;
                pointToPlaceOn.X -= rectangle.Width / 2;
                int[] ChestIndexs = StructureLoader.ReadStruct(pointToPlaceOn, path, tileBlend);
                rectangle.Location = pointL;
                StructureLoader.ProtectStructure(pointL, path);
                foreach (int chestIndex in ChestIndexs)
                {
                    var chest = Main.chest[chestIndex];
                    // etc

                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                    var itemsToAdd = new List<(int type, int stack)>();

                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                        Tuple.Create(ModContent.ItemType<AlcadizScrap>(), 0.5),
                        // Tuple.Create(ModContent.ItemType<LostScrap>(), 0.4),
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
                            //   itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                            break;
                        case 1:
                            itemsToAdd.Add((ModContent.ItemType<GreekLantern>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));

                            //  itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 30)));
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
                            //  itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                            itemsToAdd.Add((ModContent.ItemType<AlcadizScrap>(), Main.rand.Next(5, 20)));
                            //  itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 30)));
                            itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                            break;
                        case 4:
                            itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.JungleSpores, Main.rand.Next(3, 7)));

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
                            //itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
                            itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
                            break;


                        case 7:
      
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 15)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            //itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
                            itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
                            break;

                        case 8:

                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));

                            //itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 30)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
                            break;

                        case 9:
                            itemsToAdd.Add((ModContent.ItemType<SrTetanus>(), Main.rand.Next(1, 1)));
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 15)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                            // itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
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


                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));

                            itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.JungleSpores, 7));

                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));


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


                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));

                            itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.JungleSpores, 7));

                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                            itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));


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

                            //   itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(30, 55)));
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

    #endregion


    #region Ice Biome Generation
    private void IceClump(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Ice biome mounding";
        int smx = 0;
        int smy = 0;
        int contdown = 0;
        int contdownx = 0;

        smx = GenVars.snowOriginLeft + GenVars.snowOriginRight;
        smx /= 2;
        smy = (int)GenVars.worldSurfaceHigh - 600;
        while (!WorldGen.SolidTile(smx, smy) && smy <= Main.UnderworldLayer)
        {
            //seperation
            smy += 1;
        }


        Point Loc7 = new Point(smx, smy);
        SnowClumpOriginPoint = new Point(smx, smy + 100);

        WorldUtils.Gen(SnowClumpOriginPoint, new Shapes.Mound(450, 150), Actions.Chain(new GenAction[]
            {
                    new Actions.ClearWall(true),
                    new Actions.SetTile(TileID.SnowBlock),
                    new Actions.Smooth(true)
            }));


        // Spawn in Ice Chunks
        WorldGen.TileRunner(Loc7.X, Loc7.Y, 1000, 6, TileID.SnowBlock, false, 0f, 0f, true, true);
        WorldGen.TileRunner(Loc7.X, Loc7.Y + 300, 1200, 7, TileID.IceBlock, false, 0f, 0f, true, true);
        WorldGen.TileRunner(Loc7.X, Loc7.Y + 600, 1000, 2, TileID.IceBlock, false, 0f, 0f, true, true);
        WorldGen.TileRunner(Loc7.X, Loc7.Y + 900, 500, 2, TileID.IceBlock, false, 0f, 0f, true, true);
        WorldGen.TileRunner(Loc7.X, Loc7.Y + 1200, 500, 2, TileID.IceBlock, false, 0f, 0f, true, true);


        WorldUtils.Gen(Loc7, new Shapes.Circle(500, 300), Actions.Chain(new GenAction[]
        {
                new Actions.ClearWall(true),
                new Actions.PlaceWall(WallID.SnowWallUnsafe)
        }));

        // Dig big chasm at top




        for (int daa = 0; daa < 30; daa++)
        {
            contdown -= 10;
            contdownx -= 20;
            // Dig big chasm at top
            WorldGen.digTunnel(smx - Main.rand.Next(10), smy - 250 - contdown, 0, 1, 1, 15, false);

            WorldGen.digTunnel(smx - 300 - contdownx, smy + 1200, 0, 1, 1, Main.rand.Next(40) + 10, true);

            WorldGen.digTunnel(smx - 300 - contdownx, smy + 1500, 0, 1, 1, Main.rand.Next(40) + 10, true);

            WorldGen.digTunnel(smx - 300 - contdownx, smy + 1800, 0, 1, 1, Main.rand.Next(40) + 10, true);
        }
    }
    private void RuneBridges(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "The frozen folk creating bridges";

        for (int k = 0; k < 25; k++)
        {
            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 10000000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int smx = WorldGen.genRand.Next(1000, (Main.maxTilesX - 1000)); // from 50 since there's a unaccessible area at the world's borders
                                                                                // 50% of choosing the last 6th of the world
                                                                                // Choose which side of the world to be on randomly
                ///if (WorldGen.genRand.NextBool())
                ///{
                ///	towerX = Main.maxTilesX - towerX;
                ///}

                //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                int smy = (int)GenVars.worldSurfaceHigh - 500;

                // We go down until we hit a solid tile or go under the world's surface
                Tile tile = Main.tile[smx, smy];

                while (!WorldGen.SolidTile(smx, smy) && smy <= Main.UnderworldLayer || (!(tile.TileType == TileID.SnowBlock) && WorldGen.SolidTile(smx, smy)))
                {
                    smy++;
                    tile = Main.tile[smx, smy];
                }

                // If we went under the world's surface, try again
                if (smy > Main.worldSurface + 500)
                {
                    continue;
                }

                // If the type of the tile we are placing the tower on doesn't match what we want, try again



                // place the Rogue
                //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                //Main.npc[num].homeTileX = -1;
                //	Main.npc[num].homeTileY = -1;
                //	Main.npc[num].direction = 1;
                //	Main.npc[num].homeless = true;
                if (Main.tile[smx, smy].TileType == TileID.SnowBlock)
                {
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            //Start Left
                            for (int da = 0; da < 1; da++)
                            {
                                Point Loc = new Point(smx - 15, smy + 10);
                                //StructureLoader.ReadStruct(Loc, "Struct/Underground/Manor", tileBlend);
                                string path = "Struct/IceStruct/BridgeIce1";//


                                int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path);
                                //StructureLoader.ProtectStructure(Loc, path);
                                foreach (int chestIndex in ChestIndexs)
                                {
                                    var chest = Main.chest[chestIndex];
                                    // etc

                                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                                    var itemsToAdd = new List<(int type, int stack)>();

                                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                        Tuple.Create(ModContent.ItemType<RainforestGrassBlock>(), 0.5)


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
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 1:
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 2:
                                            //   itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                                            //     itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                            //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 3:
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));

                                            break;
                                        case 4:
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












                                // GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));


                                placed = true;
                            }
                            break;
                        case 1:
                            for (int da = 0; da < 1; da++)
                            {
                                Point Loc = new Point(smx - 20, smy + 20);
                                //StructureLoader.ReadStruct(Loc, "Struct/Underground/Manor", tileBlend);
                                string path = "Struct/IceStruct/BridgeIce2";//


                                int[] ChestIndexs = Structurizer.ReadStruct(Loc, path);
                                //StructureLoader.ProtectStructure(Loc, path);
                                foreach (int chestIndex in ChestIndexs)
                                {
                                    var chest = Main.chest[chestIndex];
                                    // etc

                                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                                    var itemsToAdd = new List<(int type, int stack)>();

                                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                        Tuple.Create(ModContent.ItemType<RainforestGrassBlock>(), 0.5)


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
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 1:
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 2:
                                            //   itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                                            //     itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                            //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 3:
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));

                                            break;
                                        case 4:
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












                                // GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));


                                placed = true;
                            }
                            break;
                        case 2:
                            for (int da = 0; da < 1; da++)
                            {
                                Point Loc = new Point(smx - 15, smy + 10);
                                //StructureLoader.ReadStruct(Loc, "Struct/Underground/Manor", tileBlend);
                                string path = "Struct/IceStruct/BridgeIce3";//

                                int[] ChestIndexs = Structurizer.ReadStruct(Loc, path);
                                //StructureLoader.ProtectStructure(Loc, path);
                                foreach (int chestIndex in ChestIndexs)
                                {
                                    var chest = Main.chest[chestIndex];
                                    // etc

                                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                                    var itemsToAdd = new List<(int type, int stack)>();

                                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                        Tuple.Create(ModContent.ItemType<RainforestGrassBlock>(), 0.5)


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
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 1:
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 2:
                                            //   itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                                            //     itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                            //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 3:
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));

                                            break;
                                        case 4:
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












                                // GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));


                                placed = true;
                            }
                            break;
                        case 3:
                            for (int da = 0; da < 1; da++)
                            {
                                Point Loc = new Point(smx - 20, smy + 10);
                                //StructureLoader.ReadStruct(Loc, "Struct/Underground/Manor", tileBlend);
                                string path = "Struct/IceStruct/BridgeIce3";//

                                int[] ChestIndexs = Structurizer.ReadStruct(Loc, path);
                                //StructureLoader.ProtectStructure(Loc, path);
                                foreach (int chestIndex in ChestIndexs)
                                {
                                    var chest = Main.chest[chestIndex];
                                    // etc

                                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                                    var itemsToAdd = new List<(int type, int stack)>();

                                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                        Tuple.Create(ModContent.ItemType<RainforestGrassBlock>(), 0.5)


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
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 1:
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 2:
                                            //   itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                                            //     itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                            //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 3:
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));

                                            break;
                                        case 4:
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












                                // GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));


                                placed = true;
                            }
                            break;
                    }

                }

            }
        }
    }
    private void MakingIcyRandomness(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Ice settling in the ground";




        // Select a place in the first 6th of the world, avoiding the oceans
        int numSpikes = 40;
        for (int k = 0; k < numSpikes; k++)
        {
            int X = WorldGen.genRand.Next(GenVars.snowOriginLeft, GenVars.snowOriginRight);
            int Y = (int)(Main.worldSurface - 200);
            int yBelow = Y + 1;
            for (int yOffset = 0; yOffset < 1000; yOffset++)
            {
                yBelow++;
                if (WorldGen.SolidTile(X, yBelow))
                    break;
            }

            Vector2 startPoint = new Vector2(X, yBelow);
            Vector2D endPoint = new Vector2D(WorldGen.genRand.Next(-10, 10), WorldGen.genRand.Next(-20, -8));
            if (Main.tile[X, yBelow].TileType == TileID.SnowBlock)
            {
                StructureMap structures = GenVars.structures;
                Rectangle areaToPlaceIn = new Rectangle(
                    (int)startPoint.X - 5,
                    (int)startPoint.Y - 10,
                    10, 20);
                if (!structures.CanPlace(areaToPlaceIn))
                    continue;

                WorldUtils.Gen(startPoint.ToPoint(), new Shapes.Tail(10, endPoint), Actions.Chain(new GenAction[]
                {
                    new Actions.SetTile(TileID.IceBlock),
                }));
            }
        }

        int numCircles = 12;
        for (int s = 0; s < numCircles; s++)
        {
            int X = WorldGen.genRand.Next(GenVars.snowOriginLeft, GenVars.snowOriginRight);
            int Y = (int)(Main.worldSurface - 100);
            int yBelow = Y + 1;
            Vector2 WallPosition = new Vector2(X, yBelow);
            for (int yOffset = 0; yOffset < 1000; yOffset++)
            {
                yBelow++;
                if (WorldGen.SolidTile(X, yBelow))
                {
                    break;
                }
            }

            if (Main.tile[X, yBelow].TileType == TileID.SnowBlock)
            {
                StructureMap structures = GenVars.structures;
                Rectangle areaToPlaceIn = new Rectangle(
                    (int)WallPosition.X - 3,
                    (int)WallPosition.Y - 3,
                    6, 6);
                if (!structures.CanPlace(areaToPlaceIn))
                    continue;
                WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 3)), Actions.Chain(new GenAction[]
                   {
                        //new Actions.ClearWall(true),
                        new Actions.SetTile(TileID.IceBlock),
                        new Actions.Smooth(true)
                   }));



            }


        }


        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
            int Y = WorldGen.genRand.Next(0, (int)Main.worldSurface);
            int yBelow = Y + 1;
            Vector2 WallPosition = new Vector2(X, yBelow);
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.IceBlock)
            {

                WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 3)), Actions.Chain(new GenAction[]
                   {
                        new Actions.ClearWall(true),
                        new Actions.PlaceWall(WallID.IceEcho),
                        new Actions.Smooth(true)
                   }));



            }







        }

        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 8.2f) * 6E-04); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
            int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
            int yBelow = Y + 1;
            Vector2 WallPosition = new Vector2(X, yBelow);
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.SnowBlock)
            {

                WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 4)), Actions.Chain(new GenAction[]
                   {
                        new Actions.SetTile(TileID.IceBlock),
                        new Actions.Smooth(true)
                   }));



            }
        }



    }
    private void SurfaceIceHouses(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "The frozen folk making village homes";

        StructureMap circleStructures = new StructureMap();
        for (int k = 0; k < 5; k++)
        {
            int attempts = 0;
            while (attempts++ < 10000000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int smx = WorldGen.genRand.Next(GenVars.snowOriginLeft, GenVars.snowOriginRight);
                //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                int smy = (int)GenVars.worldSurfaceHigh - 700;

                // We go down until we hit a solid tile or go under the world's surface
                Tile tile = Main.tile[smx, smy];
                while (!WorldGen.SolidTile(smx, smy))
                {
                    smy++;
                    tile = Main.tile[smx, smy];
                }

                // If we went under the world's surface, try again
                if (smy > Main.worldSurface + 500)
                {
                    continue;
                }

                Vector2 WallPosition = new Vector2(smx + 8, smy + 11);

                Rectangle areaToPlaceIn = new Rectangle(
                    (int)WallPosition.X - 12,
                    (int)WallPosition.Y - 12,
                    24, 24);
                bool success = circleStructures.CanPlace(areaToPlaceIn);
                if (!success)
                    continue;

                //Place snow underneath of the house structure
                WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(12), Actions.Chain(new GenAction[]
                {
                    new Actions.SetTile(TileID.SnowBlock)
                }));

                circleStructures.AddProtectedStructure(areaToPlaceIn);

                switch (Main.rand.Next(2))
                {
                    case 0:
                        //Start Left
                        for (int da = 0; da < 1; da++)
                        {
                            Point Loc = new Point(smx, smy + 5);
                            string path = "Struct/IceStruct/HouseSurfaceIce1";//

                            Structurizer.ProtectStructure(Loc, path);
                            int[] ChestIndexs = Structurizer.ReadStruct(Loc, path);

                            foreach (int chestIndex in ChestIndexs)
                            {
                                var chest = Main.chest[chestIndex];
                                // etc

                                // itemsToAdd will hold type and stack data for each item we want to add to the chest
                                var itemsToAdd = new List<(int type, int stack)>();

                                // Using a switch statement and a random choice to add sets of items.
                                switch (Main.rand.Next(4))
                                {
                                    case 0:

                                        itemsToAdd.Add((ItemID.ClimbingClaws, Main.rand.Next(1, 1)));
                                        itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 2)));
                                        itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                        itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                        itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                        break;


                                    case 1:
                                        itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 2)));
                                        itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                        itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                        itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                        break;


                                    case 2:
                                        //   itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                                        //     itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                        //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));


                                        itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 2)));
                                        itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                        itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                        itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                        break;


                                    case 3:

                                        itemsToAdd.Add((ItemID.ShoeSpikes, Main.rand.Next(1, 1)));
                                        itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 2)));
                                        itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                        itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                        itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
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
                        break;
                    case 1:
                        for (int da = 0; da < 1; da++)
                        {
                            Point Loc = new Point(smx, smy + 5);
                            string path = "Struct/IceStruct/HouseSurfaceIce2";//

                            Structurizer.ProtectStructure(Loc, path);
                            int[] ChestIndexs = Structurizer.ReadStruct(Loc, path);

                            foreach (int chestIndex in ChestIndexs)
                            {
                                var chest = Main.chest[chestIndex];
                                var itemsToAdd = new List<(int type, int stack)>();
                                // Using a switch statement and a random choice to add sets of items.
                                switch (Main.rand.Next(4))
                                {
                                    case 0:

                                        itemsToAdd.Add((ItemID.ClimbingClaws, Main.rand.Next(1, 1)));
                                        itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 2)));
                                        itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                        itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                        itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                        break;


                                    case 1:
                                        itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 2)));
                                        itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                        itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                        itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                        break;


                                    case 2:
                                        //   itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                                        //     itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                        //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));


                                        itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 2)));
                                        itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                        itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                        itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                        break;


                                    case 3:

                                        itemsToAdd.Add((ItemID.ShoeSpikes, Main.rand.Next(1, 1)));
                                        itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 2)));
                                        itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                        itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                        itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
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
                        break;

                }
                break;
            }
        }


    }
    private void InGroundIceHouses(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Little Icey people making little forts";




        // Select a place in the first 6th of the world, avoiding the oceans
        for (int k = 0; k < (int)((double)((Main.maxTilesX * Main.maxTilesY * 13.2f) * 6E-07) + 9); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
            int Y = WorldGen.genRand.Next(0, (int)Main.worldSurface);
            int yBelow = Y + 1;
            Vector2 WallPosition = new Vector2(X, yBelow);

            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.SnowBlock)
            {

                switch (Main.rand.Next(2))
                {
                    case 0:
                        //Start Left
                        for (int da = 0; da < 1; da++)
                        {
                            Point Loc = new Point(X, yBelow + 5);
                            //StructureLoader.ReadStruct(Loc, "Struct/Underground/Manor", tileBlend);
                            string path = "Struct/IceStruct/HouseSurfaceIce1";//
                            int[] ChestIndexs = Structurizer.ReadStruct(Loc, path);
                            Structurizer.ProtectStructure(Loc, path);
                            foreach (int chestIndex in ChestIndexs)
                            {
                                var chest = Main.chest[chestIndex];
                                // etc

                                // itemsToAdd will hold type and stack data for each item we want to add to the chest
                                var itemsToAdd = new List<(int type, int stack)>();

                                // Here is an example of using WeightedRandom to choose randomly with different weights for different items.

                                /*
                                int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                    Tuple.Create(ModContent.ItemType<RainforestGrassBlock>(), 0.5)


                                // Choose no item with a high weight of 7.
                                );
                                if (specialItem != ItemID.None)
                                {
                                    itemsToAdd.Add((specialItem, 1));
                                }
                                */
                                // Using a switch statement and a random choice to add sets of items.
                                switch (Main.rand.Next(4))
                                {
                                    case 0:

                                        itemsToAdd.Add((ItemID.ClimbingClaws, Main.rand.Next(1, 1)));
                                        itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 2)));
                                        itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                        itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                        itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                        break;


                                    case 1:
                                        itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 2)));
                                        itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                        itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                        itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                        break;


                                    case 2:
                                        //   itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                                        //     itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                        //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));


                                        itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 2)));
                                        itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                        itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                        itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                        break;


                                    case 3:

                                        itemsToAdd.Add((ItemID.ShoeSpikes, Main.rand.Next(1, 1)));
                                        itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 2)));
                                        itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                        itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                        itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
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












                            // GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));



                        }
                        break;
                    case 1:
                        for (int da = 0; da < 1; da++)
                        {
                            Point Loc = new Point(X, yBelow + 3);
                            //StructureLoader.ReadStruct(Loc, "Struct/Underground/Manor", tileBlend);
                            string path = "Struct/IceStruct/HouseSurfaceIce2";//
                            int[] ChestIndexs = Structurizer.ReadStruct(Loc, path);
                            Structurizer.ProtectStructure(Loc, path);
                            foreach (int chestIndex in ChestIndexs)
                            {
                                var chest = Main.chest[chestIndex];
                                // etc

                                // itemsToAdd will hold type and stack data for each item we want to add to the chest
                                var itemsToAdd = new List<(int type, int stack)>();

                                // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                                /*
                                int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                    Tuple.Create(ModContent.ItemType<RainforestGrassBlock>(), 0.5)


                                // Choose no item with a high weight of 7.
                                );
                                if (specialItem != ItemID.None)
                                {
                                    itemsToAdd.Add((specialItem, 1));
                                }

                                */
                                // Using a switch statement and a random choice to add sets of items.
                                switch (Main.rand.Next(4))
                                {
                                    case 0:

                                        itemsToAdd.Add((ItemID.ClimbingClaws, Main.rand.Next(1, 1)));
                                        itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 2)));
                                        itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                        itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                        itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                        break;


                                    case 1:
                                        itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 2)));
                                        itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                        itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                        itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                        break;


                                    case 2:
                                        //   itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                                        //     itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                        //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));


                                        itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 2)));
                                        itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                        itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                        itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                        break;


                                    case 3:

                                        itemsToAdd.Add((ItemID.ShoeSpikes, Main.rand.Next(1, 1)));
                                        itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 2)));
                                        itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                        itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                        itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
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












                            // GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));



                        }
                        break;

                }



            }







        }

    }
    #endregion
    #region Abyss
    public Point AbyssCenter;
    private void WorldGenAbysm(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Shifting Shadows deep in the Ice";
        //Calculate center of the abyss
        AbyssCenter = new Point();
        AbyssCenter.X = GenVars.snowOriginLeft + GenVars.snowOriginRight;
        AbyssCenter.X /= 2;
        AbyssCenter.Y = (int)(GenVars.rockLayerHigh + Main.maxTilesY * 0.15);
        AbyssCenter.Y -= 20;
        //Place the center like a circle

        ushort abyssTile = (ushort)ModContent.TileType<AbyssalDirt>();
        for (int i = 0; i < 100; i++)
        {
            WorldGen.TileRunner(AbyssCenter.X, AbyssCenter.Y, 30, 150, abyssTile, false);
        }

        int width = GenVars.snowOriginRight - GenVars.snowOriginLeft;
        int radius = width / 2;
        int heightRadius = radius / 2;
        for (int i = 0; i < 350; i++)
        {
            Point abyssClump = AbyssCenter;
            abyssClump.X += WorldGen.genRand.Next(-radius, radius);
            abyssClump.Y += WorldGen.genRand.Next(-heightRadius, heightRadius);
            WorldGen.TileRunner(abyssClump.X, abyssClump.Y, 30, 150, abyssTile, false);
        }

        //https://stackoverflow.com/questions/13894715/draw-equidistant-points-on-a-spiral
        double coils = 8;
        // value of theta corresponding to end of last coil
        double thetaMax = coils * 2 * Math.PI;

        // How far to step away from center for each side.
        double spiralRadius = 250;
        double awayStep = spiralRadius / thetaMax;

        // distance between points to plot
        double chord = 10;

        float rotation = 1;
        double centerX = AbyssCenter.X;
        double centerY = AbyssCenter.Y;

        // For every side, step around and away from center.
        // start at the angle corresponding to a distance of chord
        // away from centre.
        for (double theta = chord / awayStep; theta <= thetaMax;)
        {
            // How far away from center
            double away = awayStep * theta;

            // How far around the center.
            double around = theta + rotation;

            // Convert 'around' and 'away' to X and Y.
            double x = centerX + Math.Cos(around) * away;
            double y = centerY + Math.Sin(around) * away;

            Point currentPoint = new Point((int)x, (int)y);
            int fluff = 100;

            // to a first approximation, the points are on a circle
            // so the angle between them is chord/radius
            theta += chord / away;
            if (currentPoint.X < fluff || currentPoint.X > Main.maxTilesX - fluff)
                continue;
            if (currentPoint.Y < fluff || currentPoint.Y > Main.maxTilesY - fluff)
                continue;


            /*
            WorldUtils.Gen(currentPoint,
                new Shapes.Circle(innerCircleRadius, innerCircleRadius),
                new Actions.SetTile(abyssTile));*/
            WorldGen.TileRunner(currentPoint.X, currentPoint.Y, 10, 150, abyssTile, false);
            WorldGen.TileRunner(currentPoint.X, currentPoint.Y, 10, 150, abyssTile, false);
        }
        var genRand = WorldGen.genRand;
        for (double theta = chord / awayStep; theta <= thetaMax;)
        {
            // How far away from center
            double away = awayStep * theta;

            // How far around the center.
            double around = theta + rotation;

            // Convert 'around' and 'away' to X and Y.
            double x = centerX + Math.Cos(around) * away;
            double y = centerY + Math.Sin(around) * away;

            Point currentPoint = new Point((int)x, (int)y);
            int fluff = 100;

            // to a first approximation, the points are on a circle
            // so the angle between them is chord/radius
            theta += chord / away;
            if (currentPoint.X < fluff || currentPoint.X > Main.maxTilesX - fluff)
                continue;
            if (currentPoint.Y < fluff || currentPoint.Y > Main.maxTilesY - fluff)
                continue;

            WorldGen.TileRunner(currentPoint.X, currentPoint.Y,
              genRand.NextFloat(5, 10),
              genRand.Next(60, 80), -1);
        }

        ushort abyssalIce = (ushort)ModContent.TileType<AbyssalIce>();
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = 0; y < Main.maxTilesY; y++)
            {

                Vector2 tilePosition = new Vector2(x, y);
                float distance = Vector2.Distance(AbyssCenter.ToVector2(), tilePosition);
                if (distance < spiralRadius)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile && (tile.TileType == TileID.IceBlock))
                    {
                        WorldGen.PlaceTile(x, y, abyssalIce, forced: true);
                    }
                }
            }
        }
    }

    private void NewCaveFormationAbysm(GenerationProgress progress, GameConfiguration configuration)
    {



    }
    private void WorldGenAurelusTemple(GenerationProgress progress, GameConfiguration configuration)
    {
        StructureMap structures = GenVars.structures;
        Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Aurelus/AurelusTemple2");
        progress.Message = "Singularities Singing!";

        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 1000000)
        {
            Point Loc = AbyssCenter;
            Loc.X -= rectangle.Width / 2;
            Loc.Y += rectangle.Height / 2;
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

                        itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                        itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                        itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                        break;
                    case 2:
                        itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                        itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                        itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                        itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner8>(), Main.rand.Next(1, 1)));
                        itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                        break;
                    case 3:
                        //     itemsToAdd.Add((ModContent.ItemType<TON618Crossbow>(), Main.rand.Next(1, 1)));
                        // itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
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

                        itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                        itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                        itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                        break;

                    case 5:
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
            placed = true;
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

                            itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            break;
                        case 2:
                            itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                            itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                            itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(100, 1500)));
                            itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                            itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                            break;
                        case 3:
                            //   itemsToAdd.Add((ModContent.ItemType<TON618Crossbow>(), Main.rand.Next(1, 1)));
                            // itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
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

                            itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                            itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                            itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                            break;

                        case 5:
                      
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
    #endregion


    #region Veil Biome

    Point pointL;
    Point pointLil;

    private void WorldGenVeilSpot(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Residents of the veil believing in a god";


        int[] tileBlend = new int[]
        {
            TileID.RubyGemspark
        };

        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 10000000)
        {
            // Select a place in the first 6th of the world, avoiding the oceans
            int smx = WorldGen.genRand.Next(300, (Main.maxTilesX - 1000)); // from 50 since there's a unaccessible area at the world's borders
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
            if (!(tile.TileType == TileID.IceBlock
                || tile.TileType == TileID.SnowBlock))
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




                Point Loc = new Point(smx, smy + 343);

                for (int daa = 0; daa < 1; daa++)
                {
                    Point Loc7 = new Point(smx, smy);
                    WorldGen.TileRunner(Loc7.X + 275, Loc7.Y + 100, 600, 2, ModContent.TileType<Tiles.CatagrassBlock>(), false, 0f, 0f, true, true);



                }

                pointLil = new Point(smx + 80, smy + 330);


                //This code just places


                //	WorldUtils.Gen(Loc6, new Shapes.Circle(40), new Actions.SetTile(TileID.Dirt));
                //	Point resultPoint;
                //	bool searchSuccessful = WorldUtils.Find(Loc, Searches.Chain(new Searches.Right(200), new GenCondition[]
                //	{
                //new Conditions.IsSolid().AreaAnd(10, 10),
                //new Conditions.IsTile(TileID.Sand).AreaAnd(10, 10),
                //	}), out resultPoint);
                //		if (searchSuccessful)
                //		{
                //			WorldGen.TileRunner(resultPoint.X, resultPoint.Y, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(150, 150), TileID.Dirt);
                //		}




                //WorldGen.TileRunner(Loc2.X - 10, Loc2.Y - 60, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(120, 120), TileID.Grass);
                //WorldGen.TileRunner(Loc3.X - 20, Loc2.Y, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);
                //WorldGen.TileRunner(Loc3.X - 20, Loc3.Y + 20, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);
                placed = true;
            }


        }

    }


    private void WorldGenVU(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Residents of the veil crafting chasms";


        int[] tileBlend = new int[]
        {
            TileID.RubyGemspark
        };

        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 10000000)
        {
            // Select a place in the first 6th of the world, avoiding the oceans
            int smx = WorldGen.genRand.Next(((Main.maxTilesX) / 2) + 300, (Main.maxTilesX - 1000)); // from 50 since there's a unaccessible area at the world's borders
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
                || tile.TileType == TileID.Grass
                || tile.TileType == TileID.Mud
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

                ShapeData shapeData = new ShapeData();

                StructureLoader.ReadStruct(pointLil, "Struct/Underground/Catacombz", tileBlend);


                Point Loc22 = new Point(pointLil.X + 40, pointLil.Y - 335);

                StructureLoader.ReadStruct(Loc22, "Struct/Morrow/Morrowtop");


                //			WorldUtils.Gen(Loc22, new Shapes.Rectangle(240, -40), new Actions.ClearTile(true));


                Point Loc4 = new Point(smx + 233, smy + 45);
                //	WorldUtils.Gen(Loc2, new Shapes.Mound(60, 90), new Actions.SetTile(TileID.Dirt));
                //	WorldUtils.Gen(Loc4, new Shapes.Rectangle(220, 105), new Actions.SetTile(TileID.Dirt));

                Point Loc5 = new Point(smx + 10, smy + 45);
                //	WorldUtils.Gen(Loc5, new Shapes.Rectangle(220, 50), new Actions.SetTile(TileID.Dirt));



                Point Loc3 = new Point(smx + 455, smy + 30);
                //	WorldUtils.Gen(Loc3, new Shapes.Mound(40, 50), new Actions.SetTile(TileID.Dirt));
                Point Loc6 = new Point(smx + 455, smy + 40);
                //	WorldUtils.Gen(Loc6, new Shapes.Circle(40), new Actions.SetTile(TileID.Dirt));
                //	Point resultPoint;
                //	bool searchSuccessful = WorldUtils.Find(Loc, Searches.Chain(new Searches.Right(200), new GenCondition[]
                //	{
                //new Conditions.IsSolid().AreaAnd(10, 10),
                //new Conditions.IsTile(TileID.Sand).AreaAnd(10, 10),
                //	}), out resultPoint);
                //		if (searchSuccessful)
                //		{
                //			WorldGen.TileRunner(resultPoint.X, resultPoint.Y, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(150, 150), TileID.Dirt);
                //		}



                GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 233, 346));
                //WorldGen.TileRunner(Loc2.X - 10, Loc2.Y - 60, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(120, 120), TileID.Grass);
                //WorldGen.TileRunner(Loc3.X - 20, Loc2.Y, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);
                //WorldGen.TileRunner(Loc3.X - 20, Loc3.Y + 20, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);
                placed = true;
            }


        }

    }

    #endregion


    #region Royal Capital


    public void WorldGenRoyalCapital(GenerationProgress progress, GameConfiguration configuration)
    {
        StructureMap structures = GenVars.structures;
        Rectangle rectangle = Structurizer.ReadRectangle("Struct/Alcad/RoyalCapital3");
        progress.Message = "Fighting the Virulent with magic";





        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 10000000)
        {
            Point Loc = RoyalCapitalLocation;
            rectangle.Location = Loc;
            AlcadLocation = Loc;
            Structurizer.ProtectStructure(Loc, "Structures/RoyalCapital");
            var tileBlend = new int[]
            {
                TileID.RubyGemspark
            };

            int[] ChestIndexs = Structurizer.ReadStruct(Loc, "Structures/RoyalCapital", tileBlend);

            foreach (int chestIndex in ChestIndexs)
            {
                var chest = Main.chest[chestIndex];
                // etc

                // itemsToAdd will hold type and stack data for each item we want to add to the chest
                var itemsToAdd = new List<(int type, int stack)>();

                // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                    //  Tuple.Create(ModContent.ItemType<LostScrap>(), 0.1),
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

                        ;
                        itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                        itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                        itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                        break;
                    case 1:
                
                        itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                        itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                        itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));


                        itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                        itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
                        break;
                    case 2:
                        itemsToAdd.Add((ModContent.ItemType<BlackRose>(), Main.rand.Next(1, 1)));
                        itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                        itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));

                        itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                        itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                        break;
                    case 3:
                        //   itemsToAdd.Add((ModContent.ItemType<FloweredInsource>(), Main.rand.Next(1, 1)));
                        // itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                        itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                        itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                        itemsToAdd.Add((ModContent.ItemType<AlcadizScrap>(), Main.rand.Next(5, 20)));

                        itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                        break;
                    case 4:
                        itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                        itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                        itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                        itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));

                        itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
                        itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                        break;

                    case 5:
                        itemsToAdd.Add((ItemID.FuneralHat, Main.rand.Next(1, 1)));
                        itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                        itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));
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

    #endregion

    #region Illuria
    public void WorldGenIlluria(GenerationProgress progress, GameConfiguration configuration)
    {
        StructureMap structures = GenVars.structures;
        Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Overworld/Illuria");
        progress.Message = "Niivi protecting the cities above.";

        Point Loc = new Point(GenVars.snowOriginRight - 150, (int)Main.worldSurface - 350);
        rectangle.Location = Loc;
        Structurizer.ProtectStructure(Loc, "Struct/Overworld/Illuria");
        int[] ChestIndexs = Structurizer.ReadStruct(Loc, "Struct/Overworld/Illuria");
        foreach (int chestIndex in ChestIndexs)
        {
            var chest = Main.chest[chestIndex];
            var itemsToAdd = new List<(int type, int stack)>();
            // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
            int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                Tuple.Create(ModContent.ItemType<AlcadizScrap>(), 0.5),
                //  Tuple.Create(ModContent.ItemType<LostScrap>(), 0.1),
                Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)
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
              
                    itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                    itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                    itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));

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
                    //    itemsToAdd.Add((ModContent.ItemType<FloweredInsource>(), Main.rand.Next(1, 1)));
                    //   itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
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


    #endregion

    #region Ores
    private void WorldGenDragonpieceOre(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Dragons Scorch the Earth...";
        int tileType = ModContent.TileType<DragonpieceOre>();
        int count = (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05);
        for (int k = 0; k < count; k++)
        {
            int x = WorldGen.genRand.Next(0, Main.maxTilesX);
            int y = WorldGen.genRand.Next(HeatedDepthsStart, HeatedDepthsEnd);

            Tile tile = Main.tile[x, y];
            if (!tile.HasTile)
                continue;

            VeilGen.QuickOrePatch(x, y, tileType);
            progress.Set(k / (float)count);
        }
    }

    private void WorldGenFlameOre(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Scorching Gild and Arnchar burning into the world";
        int tileType = ModContent.TileType<VerianoreTile>();
        for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
        {
            int x = WorldGen.genRand.Next(0, Main.maxTilesX);
            int y = WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY);

            Tile tile = Main.tile[x, y];
            if (!tile.HasTile)
                continue;

            VeilGen.QuickOrePatch(x, y, tileType);
        }
    }

    private void WorldGenFrileOre(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Freezing the world with Frile";
        int tileType = ModContent.TileType<FrileOreTile>();
        double num = (Main.maxTilesX * Main.maxTilesY) * 6E-05;
        num *= 2;
        for (int k = 0; k < (int)(num); k++)
        {
            int x = WorldGen.genRand.Next(GenVars.snowOriginLeft - 600, GenVars.snowOriginRight + 600);
            int y = WorldGen.genRand.Next((int)GenVars.rockLayerHigh - 500, Main.maxTilesY - 400);

            //Only spawn on ice/snow
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile)
                continue;
            if (tile.TileType != TileID.IceBlock && tile.TileType != TileID.SnowBlock)
                continue;

            VeilGen.QuickOrePatch(x, y, tileType);
        }
    }

    private void WorldGenGlisteningOre(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "World Glistens with shines of the Glistening Moon";
        int tileType = ModContent.TileType<GlisteningOreTile>();
        for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
        {
            int x = WorldGen.genRand.Next(0, Main.maxTilesX);
            int y = WorldGen.genRand.Next((int)GenVars.rockLayerHigh, DarkspaceStart);
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile)
                continue;

            VeilGen.QuickOrePatch(x, y, tileType);
        }
    }

    #endregion

    private void WorldGenUnderworldSpice(GenerationProgress progress, GameConfiguration configuration)
    {
        // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
        progress.Message = "Sylia using magic in the Underworld";





        for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-06 + 10); k++)
        {

            int xa = WorldGen.genRand.Next(0, Main.maxTilesX);
            int ya = WorldGen.genRand.Next(Main.maxTilesY - 400, Main.maxTilesY - 50);
            Point Loc = new Point(xa, ya);

            // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
            Tile tile = Main.tile[Loc.X, Loc.Y];

            if (!(tile.TileType == TileID.Ash ||
                tile.TileType == TileID.Stone ||
                tile.TileType == ModContent.TileType<CindersparkDirt>()))
            {
                continue;
            }

            if (tile.HasTile)
            {
                int Sounda = Main.rand.Next(1, 6);
                if (Sounda == 1)
                {


                    for (int da = 0; da < 1; da++)
                    {


                        int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Underworld/Underworld1");
                        foreach (int chestIndex in ChestIndexs)
                        {
                            var chest = Main.chest[chestIndex];
                            // etc

                            // itemsToAdd will hold type and stack data for each item we want to add to the chest
                            var itemsToAdd = new List<(int type, int stack)>();

                            // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                            int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                Tuple.Create(ModContent.ItemType<AlcaricMush>(), 0.5),
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
                                    // itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                                    itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                    itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                    break;
                                case 4:
                                    itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                                    itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));

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
                                    // itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
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










                if (Sounda == 2)
                {


                    for (int da = 0; da < 1; da++)
                    {


                        int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Underworld/Underworld2");
                        foreach (int chestIndex in ChestIndexs)
                        {
                            var chest = Main.chest[chestIndex];
                            // etc

                            // itemsToAdd will hold type and stack data for each item we want to add to the chest
                            var itemsToAdd = new List<(int type, int stack)>();

                            // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                            int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                Tuple.Create(ModContent.ItemType<Gambit>(), 0.5),
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
                                    //  itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                                    itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                    itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                    break;
                                case 4:
                                    itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                                    itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));

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
                                    itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
                                    break;


                                case 7:
                                    itemsToAdd.Add((ItemID.WaterWalkingBoots, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ItemID.Fireblossom, Main.rand.Next(2, 15)));
                                    itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                                    itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                    //  itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
                                    itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
                                    break;

                                case 8:
                                    itemsToAdd.Add((ItemID.ObsidianSkull, Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                    itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));

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






                    if (Sounda == 3 || Sounda == 4)
                    {


                        for (int da = 0; da < 1; da++)
                        {


                            int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Underworld/Underworld4");
                            foreach (int chestIndex in ChestIndexs)
                            {
                                var chest = Main.chest[chestIndex];
                                // etc

                                // itemsToAdd will hold type and stack data for each item we want to add to the chest
                                var itemsToAdd = new List<(int type, int stack)>();

                                // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                                int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                    Tuple.Create(ModContent.ItemType<AlcaricMush>(), 0.5),
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
                                        //  itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                                        itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                        itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                        itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                        break;
                                    case 4:
                                        itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
                                        itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));

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
                                        //    itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
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




                    if (Sounda == 5)
                    {
                        StructureLoader.ReadStruct(Loc, "Struct/Underworld/Underworld3");


                    }



                }



            }
        }
    }

    private void WorldGenMechShop(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Finding a place for the shop";



        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 1000000)
        {


            int abysmx = WorldGen.genRand.Next(700, Main.maxTilesX - 700); // from 50 since there's a unaccessible area at the world's borders

            //This code makes it avoid teh center
            int distanceBetween = Math.Abs(Main.spawnTileX - abysmx);
            for (int i = 0; i < 1000; i++)
            {
                abysmx = WorldGen.genRand.Next(700, Main.maxTilesX - 700);
                distanceBetween = Math.Abs(Main.spawnTileX - abysmx);
                if (distanceBetween > 900)
                    break;
            }

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
            if (!(tile.TileType == TileID.Stone))
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
                Point Loc = new Point(abysmx, abysmy + 100);
                string path = "Struct/Underground/MechanicShop";

                int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path);
                StructureLoader.ProtectStructure(Loc, path);
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
                    switch (Main.rand.Next(1))
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


    #region Ice Biome
    private void WorldGenCathedral(GenerationProgress progress, GameConfiguration configuration)
    {

        // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
        progress.Message = "Verlia Ark";

        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 100000)
        {
            // Select a place in the first 6th of the world, avoiding the oceans
            int towerX = WorldGen.genRand.Next(0, Main.maxTilesX - 200); // from 50 since there's a unaccessible area at the world's borders
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
            if (!(tile.TileType == TileID.IceBlock
                || tile.TileType == TileID.SnowBlock))
            {
                continue;
            }


            for (int da = 0; da < 1; da++)
            {
                Point Loc = new Point(towerX, towerY - 50);

                // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                StructureMap structures = GenVars.structures;
                StructureLoader.ProtectStructure(Loc, "Struct/Ice/VerliasCathedral");
                int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Ice/VerliasCathedral");
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
        while (!placed && attempts++ < 100000)
        {
            // Select a place in the first 6th of the world, avoiding the oceans
            int towerX = WorldGen.genRand.Next(0, Main.maxTilesX - 200); // from 50 since there's a unaccessible area at the world's borders
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
            if (!(tile.TileType == TileID.IceBlock
                || tile.TileType == TileID.SnowBlock))
            {
                continue;
            }


            for (int da = 0; da < 1; da++)
            {
                Point Loc = new Point(towerX, towerY + 14);





                // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                StructureMap structures = GenVars.structures;
                if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Ice/VeldrisHouse"))
                    continue;
                int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Ice/VeldrisHouse");
                Chest c = Main.chest[ChestIndexs[0]];

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
                            //  itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
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

    }



    #endregion









    public override void NetSend(BinaryWriter writer)
    {
        base.NetSend(writer);
        writer.Write(MarshLocation.X);
        writer.Write(MarshLocation.Y);
        writer.Write(CoralwaysLocation.X);
        writer.Write(CoralwaysLocation.Y);
    }
    public override void NetReceive(BinaryReader reader)
    {
        base.NetReceive(reader);
        Point marshLocation = new Point();
        marshLocation.X = reader.ReadInt32();
        marshLocation.Y = reader.ReadInt32();
        MarshLocation = marshLocation;

        Point coralwaysLocation = new Point();
        coralwaysLocation.X = reader.ReadInt32();
        coralwaysLocation.Y = reader.ReadInt32();
        CoralwaysLocation = coralwaysLocation;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag["MarshLocation"] = MarshLocation;
        tag["FableHillLocation"] = FableHillStartLocation;
        tag["CoralwaysLocation"] = CoralwaysLocation;
        tag["CindersparkStart"] = CindersparkStart;
        tag["CindersparkEnd"] = CindersparkEnd;
        tag["DarkspaceStart"] = DarkspaceStart;
        tag["DarkspaceEnd"] = DarkspaceEnd;
        tag["HeatedDepthsStart"] = HeatedDepthsStart;
        tag["HeatedDepthsEnd"] = HeatedDepthsEnd;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        MarshLocation = tag.Get<Point>("MarshLocation");
        FableHillStartLocation = tag.Get<Point>("FableHillLocation");
        CoralwaysLocation = tag.Get<Point>("CoralwaysLocation");
        CindersparkStart = tag.Get<int>("CindersparkStart");
        CindersparkEnd = tag.Get<int>("CindersparkEnd");
        DarkspaceStart = tag.Get<int>("DarkspaceStart");
        DarkspaceEnd = tag.Get<int>("DarkspaceEnd");
        HeatedDepthsStart = tag.Get<int>("HeatedDepthsStart");
        HeatedDepthsEnd = tag.Get<int>("HeatedDepthsEnd");
    }
}