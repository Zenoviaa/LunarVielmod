
using Stellamod.Content.Areas.Junkyard.TilesJY;
using Stellamod.Content.Areas.SpringHills.TilesSH;
using Stellamod.Content.Areas.WorldsEnd.TilesWE;
using Stellamod.Tiles;
using Stellamod.Tiles.Abyss;
using Stellamod.Tiles.Abyss.Aurelus;
using Stellamod.Tiles.Acid;
using Stellamod.Tiles.Catacombs;
using Stellamod.Tiles.Ishtar;
using Stellamod.Tiles.RoyalCapital;
using Stellamod.Tiles.Veil;
using Stellamod.TilesNew.MothlightTiles;
using Stellamod.TilesNew.RainforestTiles;
using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod
{
    public class BiomeTileCounts : ModSystem
    {
        public int AbyssCount;
        public static bool InAbyss => ModContent.GetInstance<BiomeTileCounts>().AbyssCount > 80;


        public int AcidCount;
        public static bool InAcid => ModContent.GetInstance<BiomeTileCounts>().AcidCount > 80;

        public int AurelusCount;
        public static bool InAurelus => ModContent.GetInstance<BiomeTileCounts>().AurelusCount > 70;

        public int GovheilCount;
        public static bool InGovheil => ModContent.GetInstance<BiomeTileCounts>().GovheilCount > 30;

        public int StarbloomCount;
        public static bool InStarbloom => ModContent.GetInstance<BiomeTileCounts>().StarbloomCount > 20;

        public int NaxtrinCount;
        public static bool InNaxtrin => ModContent.GetInstance<BiomeTileCounts>().NaxtrinCount > 10;

        public int RoyalCapitalCount;
        public static bool InRoyalCapital => ModContent.GetInstance<BiomeTileCounts>().RoyalCapitalCount > 10;

        public int VeriCount;
        public static bool InVeri => ModContent.GetInstance<BiomeTileCounts>().VeriCount > 20;

        public int FableCount;
        public static bool InFable => ModContent.GetInstance<BiomeTileCounts>().FableCount > 20;

        public int SeaCount;
        public static bool InSeaTemple => ModContent.GetInstance<BiomeTileCounts>().SeaCount > 20;

        public int FireCount;
        public static bool InCatafire => ModContent.GetInstance<BiomeTileCounts>().FireCount > 20;

        public int TrapCount;
        public static bool InCatatrap => ModContent.GetInstance<BiomeTileCounts>().TrapCount > 20;

        public int WaterCount;
        public static bool InCatawater => ModContent.GetInstance<BiomeTileCounts>().WaterCount > 20;

        public int XixCount;
        public static bool InXixVillage => ModContent.GetInstance<BiomeTileCounts>().XixCount > 10;


        public int CinderCount;
        public static bool InCinder => ModContent.GetInstance<BiomeTileCounts>().CinderCount > 10;

        public int ManorCount;
        public static bool InManor => ModContent.GetInstance<BiomeTileCounts>().ManorCount > 10;

        public int MechCount;
        public static bool InMech => ModContent.GetInstance<BiomeTileCounts>().MechCount > 5;

        public int LabCount;
        public static bool InLab => ModContent.GetInstance<BiomeTileCounts>().LabCount > 5;

        public int IlluriaCount;
        public static bool InIlluria => ModContent.GetInstance<BiomeTileCounts>().IlluriaCount > 5;

        public int VeilCount;
        public static bool InVeil => ModContent.GetInstance<BiomeTileCounts>().VeilCount > 5;

        public int IshtarCount;
        public static bool InIshtar => ModContent.GetInstance<BiomeTileCounts>().IshtarCount > 5;

        public int ColosseumCount;
        public static bool InColosseum => ModContent.GetInstance<BiomeTileCounts>().ColosseumCount > 5;



        public int BloodCathedralCount;
        public static bool InBloodCathedral => ModContent.GetInstance<BiomeTileCounts>().BloodCathedralCount > 250;

        public int AshotiTempleCount;
        public static bool InAshotiTemple => ModContent.GetInstance<BiomeTileCounts>().AshotiTempleCount > 100;

        public int MineshaftTileCount;
        public static bool InMineshaft => ModContent.GetInstance<BiomeTileCounts>().MineshaftTileCount > 5;
        public int MothlightCount;
        public static bool InMothlight => ModContent.GetInstance<BiomeTileCounts>().MothlightCount > 5;

        public int DarkspaceCount;
        public static bool InDarkspace => ModContent.GetInstance<BiomeTileCounts>().DarkspaceCount > 10;

        public int SpringGrassCount;
        public static bool InSpringHills => ModContent.GetInstance<BiomeTileCounts>().SpringGrassCount > 80;
        public int MistyDungeonCount;
        public static bool InMistyDungeon => ModContent.GetInstance<BiomeTileCounts>().MistyDungeonCount > 80;

        public int DesertTownCount;
        public static bool InDesertTown => ModContent.GetInstance<BiomeTileCounts>().DesertTownCount > 15;


        public int MarshCount;
        public static bool InMarsh => ModContent.GetInstance<BiomeTileCounts>().MarshCount > 50;

        public int CathedralCount;
        public int MorrowCount;

        public int WorldsEndCount;
        public static bool InWorldsEnd => ModContent.GetInstance<BiomeTileCounts>().WorldsEndCount >= 50;

        public int MoonspiralTowerCount;
        public static bool InMoonspiralTower => ModContent.GetInstance<BiomeTileCounts>().MoonspiralTowerCount >= 50;
        public int ForestCount;
        public static bool InForest => ModContent.GetInstance<BiomeTileCounts>().ForestCount >= 25;

        public int JunkyardCount;
        public static bool InJunkyard => ModContent.GetInstance<BiomeTileCounts>().JunkyardCount >= 25;
        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            JunkyardCount = tileCounts[ModContent.TileType<JunkyTile>()];
            ForestCount = tileCounts[TileID.Grass];
            MoonspiralTowerCount = tileCounts[ModContent.TileType<CathediteTile>()];
            WorldsEndCount = tileCounts[ModContent.TileType<WhiteGrass>()];
            MarshCount = tileCounts[ModContent.TileType<RainforestGrass>()];
            MorrowCount = tileCounts[ModContent.TileType<OvermorrowdirtTile>()];
         //   CathedralCount = tileCounts[ModContent.TileType<CathediteTile>()];
            MistyDungeonCount = tileCounts[TileID.BlueDungeonBrick] + tileCounts[TileID.GreenDungeonBrick] + tileCounts[TileID.PinkDungeonBrick] + tileCounts[ModContent.TileType<MothlightBrick>()];
            SpringGrassCount = tileCounts[ModContent.TileType<SpringGrass>()];
            AcidCount = tileCounts[ModContent.TileType<AcidialDirt>()];
            AbyssCount = tileCounts[ModContent.TileType<AbyssalDirt>()];
            AurelusCount = tileCounts[ModContent.TileType<AurelusTempleBlock>()];
            GovheilCount = tileCounts[ModContent.TileType<GovheilCastleTile>()];
            StarbloomCount = tileCounts[ModContent.TileType<StarbloomTempleBlock>()];
            NaxtrinCount = tileCounts[ModContent.TileType<NoxianBlock>()];
            RoyalCapitalCount = tileCounts[ModContent.TileType<AlcazBlock>()];
            VeriCount = tileCounts[ModContent.TileType<VeriplantDirt>()];
            FableCount = tileCounts[ModContent.TileType<GovheilTile>()];
            SeaCount = tileCounts[ModContent.TileType<SeavathanBrick>()];
            TrapCount = tileCounts[ModContent.TileType<CatacombStoneTrap>()];
            FireCount = tileCounts[ModContent.TileType<CatacombStoneFire>()];
            WaterCount = tileCounts[ModContent.TileType<CatacombStoneWater>()];
            XixCount = tileCounts[ModContent.TileType<HuntiacTile>()];
            CinderCount = tileCounts[ModContent.TileType<CindersparkDirt>()];
            ManorCount = tileCounts[ModContent.TileType<ManorBlock>()];
            MechCount = tileCounts[ModContent.TileType<StarbloomTempleBlock>()];
            LabCount = tileCounts[ModContent.TileType<LostScrapT>()];
            IlluriaCount = tileCounts[ModContent.TileType<IlluriaGrass>()];
            VeilCount = tileCounts[ModContent.TileType<CatagrassBlock>()];
            IshtarCount = tileCounts[ModContent.TileType<IshtarMoss>()];
            BloodCathedralCount = tileCounts[ModContent.TileType<RobedSandstoneBlock>()];
            AshotiTempleCount = tileCounts[TileID.LihzahrdBrick];
            MineshaftTileCount = tileCounts[ModContent.TileType<RobedCatastoneBlock>()];
            ColosseumCount = tileCounts[ModContent.TileType<ChiseledSandstoneT>()];
   //         MothlightCount = tileCounts[ModContent.TileType<MothlightBrick>()];
            DarkspaceCount = tileCounts[TileID.Granite];
            DesertTownCount = tileCounts[TileID.SmoothSandstone];
        }
    }
}