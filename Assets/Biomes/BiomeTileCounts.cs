
using Stellamod.Content.Areas.Fable.TilesFB;
using Stellamod.Content.Areas.Ishtar.TilesIS;
using Stellamod.Content.Areas.Junkyard.TilesJY;
using Stellamod.Content.Areas.SpringHills.TilesSH;
using Stellamod.Content.Areas.Terror.TilesTR;
using Stellamod.Content.Areas.Tundra.Abyss.TilesAB;
using Stellamod.Content.Areas.Tundra.Abyss.TilesAB.Aurelus;
using Stellamod.Content.Areas.Underground.TilesUG;
using Stellamod.Content.Areas.WorldsEnd.TilesWE;
using Stellamod.Tiles;
using Stellamod.Tiles.Ishtar;
using Stellamod.Tiles.RoyalCapital;
using Stellamod.Tiles.Veil;
using Stellamod.TilesNew.MothlightTiles;
using Stellamod.TilesNew.RainforestTiles;
using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod;

public class BiomeTileCounts : ModSystem
{
    public static int[] TileCounts = new int[64];
    public static ref int AbyssCount => ref TileCounts[0];
    public static bool InAbyss => AbyssCount > 80;

    public static ref int AurelusCount => ref TileCounts[1];
    
    public static bool InAurelus => AurelusCount > 70;

    public static ref int GovheilCount => ref TileCounts[2];
    public static bool InGovheil => GovheilCount > 30;

    public static ref int RoyalCapitalCount => ref TileCounts[3];
    public static bool InRoyalCapital => RoyalCapitalCount > 10;

    public static ref int FableCount => ref TileCounts[4];
    public static bool InFable => FableCount > 20;

    public static ref int XixCount => ref TileCounts[5];
    public static bool InXixVillage => XixCount > 10;

    public static ref int CinderCount => ref TileCounts[6];
    public static bool InCinder => CinderCount > 10;

    public static ref int ManorCount => ref TileCounts[7];
    public static bool InManor => ManorCount > 10;

    public static ref int MechCount => ref TileCounts[8];
    public static bool InMech => MechCount > 5;

    public static ref int IlluriaCount => ref TileCounts[9];
    public static bool InIlluria => IlluriaCount > 5;

    public static ref int IshtarCount => ref TileCounts[10];
    public static bool InIshtar => IshtarCount > 5;

    public static ref int ColosseumCount => ref TileCounts[11];
    public static bool InColosseum => ColosseumCount > 5;

    public static ref int BloodCathedralCount => ref TileCounts[12];
    public static bool InBloodCathedral => BloodCathedralCount > 250;

    public static ref int AshotiTempleCount => ref TileCounts[13];
    public static bool InAshotiTemple => AshotiTempleCount > 100;

    public static ref int MineshaftTileCount => ref TileCounts[14];
    public static bool InMineshaft => MineshaftTileCount > 5;

    public static ref int MothlightCount => ref TileCounts[15];
    public static bool InMothlight => MothlightCount > 5;

    public static ref int DarkspaceCount => ref TileCounts[16];
    public static bool InDarkspace => DarkspaceCount > 10;

    public static ref int SpringGrassCount => ref TileCounts[17];
    public static bool InSpringHills => SpringGrassCount > 80;

    public static ref int MistyDungeonCount => ref TileCounts[18];
    public static bool InMistyDungeon => MistyDungeonCount > 80;

    public static ref int DesertTownCount => ref TileCounts[19];
    public static bool InDesertTown => DesertTownCount > 15;

    public static ref int MarshCount => ref TileCounts[20];
    public static bool InMarsh => MarshCount > 50;

    public static ref int WorldsEndCount => ref TileCounts[21];
    public static bool InWorldsEnd => WorldsEndCount >= 50;

    public static ref int MoonspiralTowerCount => ref TileCounts[22];
    public static bool InMoonspiralTower => MoonspiralTowerCount >= 50;

    public static ref int SacredUnknownsCount => ref TileCounts[23];
    public static bool InSacredUnknowns => SacredUnknownsCount >= 10;

    public static ref int AegislavCount => ref TileCounts[24];
    public static bool InAegislav => AegislavCount >= 50;

    public static ref int CrimsonBridewellCount => ref TileCounts[25];
    public static bool InCrimsonBridewell => CrimsonBridewellCount >= 50;

    public static ref int ForestCount => ref TileCounts[26];
    public static bool InForest => ForestCount >= 25 || InSpringHills;

    public static ref int JunkyardCount => ref TileCounts[27];
    public static bool InJunkyard => JunkyardCount >= 25;

    public static ref int EveroseCount => ref TileCounts[28];
    public static bool InEveroseVillage => EveroseCount >= 80;

    public static ref int HeatedDepthsCount => ref TileCounts[29];
    public static bool InHeatedDepths => HeatedDepthsCount >= 50;
    public static DomainExpansionManager Domain => ModContent.GetInstance<DomainExpansionManager>();
    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
    {

        //THis should prevent other biomes triggering during a domain
        if (Domain.noRender)
        {
            for(var i = 0; i < TileCounts.Length; i++)
            {
                TileCounts[i] = 0;
            }
            return;
        }

        CrimsonBridewellCount = tileCounts[ModContent.TileType<BridewellTile>()];

        AegislavCount = tileCounts[ModContent.TileType<VeilBrickTile>()] + tileCounts[ModContent.TileType<AegislavSandTile>()] + tileCounts[TileID.CrimsonGrass] + tileCounts[TileID.Crimstone];
        SacredUnknownsCount = tileCounts[ModContent.TileType<MimicTileBlock>()];
        JunkyardCount = tileCounts[ModContent.TileType<JunkyTile>()];
        ForestCount = tileCounts[TileID.Grass];
        MoonspiralTowerCount = tileCounts[ModContent.TileType<CathediteTile>()];
        WorldsEndCount = tileCounts[ModContent.TileType<WhiteGrass>()];
        MarshCount = tileCounts[ModContent.TileType<RainforestGrass>()];

        MistyDungeonCount = tileCounts[TileID.BlueDungeonBrick] + tileCounts[TileID.GreenDungeonBrick] + tileCounts[TileID.PinkDungeonBrick] + tileCounts[ModContent.TileType<MothlightBrick>()];
        SpringGrassCount = tileCounts[ModContent.TileType<SpringGrass>()];
        AbyssCount = tileCounts[ModContent.TileType<AbyssalDirt>()];
        AurelusCount = tileCounts[ModContent.TileType<AurelusTempleBlock>()];
        GovheilCount = tileCounts[ModContent.TileType<GovheilCastleTile>()];
        RoyalCapitalCount = tileCounts[ModContent.TileType<AlcazBlock>()];
        FableCount = tileCounts[ModContent.TileType<WitchCarvedStone>()];
  
        XixCount = tileCounts[ModContent.TileType<HuntiacTile>()];
        EveroseCount = tileCounts[ModContent.TileType<VeriplantGrass>()];
        CinderCount = tileCounts[ModContent.TileType<CindersparkDirt>()];
        ManorCount = tileCounts[ModContent.TileType<ManorBlock>()];
        MechCount = tileCounts[ModContent.TileType<StarbloomTempleBlock>()];
        IlluriaCount = tileCounts[ModContent.TileType<IlluriaGrass>()];

        IshtarCount = tileCounts[ModContent.TileType<IshtarMoss>()] + tileCounts[ModContent.TileType<IshtarTempleBlock>()];
        BloodCathedralCount = tileCounts[ModContent.TileType<RobedSandstoneBlock>()];
        AshotiTempleCount = tileCounts[TileID.LihzahrdBrick];
        MineshaftTileCount = tileCounts[ModContent.TileType<RobedCatastoneBlock>()];
        ColosseumCount = tileCounts[ModContent.TileType<ChiseledSandstoneT>()];
        DarkspaceCount = tileCounts[TileID.Granite];
        DesertTownCount = tileCounts[TileID.SmoothSandstone];


       
        HeatedDepthsCount = tileCounts[ModContent.TileType<CharredStone>()];
        if (InCinder || InDarkspace)
        {
            HeatedDepthsCount = 0;
        }
    }
}