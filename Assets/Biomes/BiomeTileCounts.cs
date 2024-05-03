
using Stellamod.Tiles;
using Stellamod.Tiles.Abyss;
using Stellamod.Tiles.Abyss.Aurelus;
using Stellamod.Tiles.Acid;
using Stellamod.Tiles.Catacombs;
using Stellamod.Tiles.Veil;
using Stellamod.Tiles.RoyalCapital;
using System;
using Terraria.ModLoader;
using Stellamod.Tiles.Ishtar;

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
        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
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
        }
    }
}