using Microsoft.Xna.Framework;
using Stellamod.Core.Bases;
using Terraria.ModLoader;

namespace Stellamod.Items.Discs
{
    public class AcidicNightmaresDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/Acidic_Nightmares";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<AcidicNightmaresDiscTile>();
            Penetrate = 2;
            TrailColor = Color.DarkGreen;
        }
    }

    public class AcidicNightmaresDiscTile : BaseRecordTile
    {

    }

    public class AurelusTempleDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/AurelusTemple";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<AurelusTempleDiscTile>();
            Penetrate = 2;
            TrailColor = Color.LightSkyBlue;
        }
    }

    public class AurelusTempleDiscTile : BaseRecordTile
    {

    }

    public class AcidicTerrorsDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/Acidic_Terors";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<AcidicTerrorsDiscTile>();
            Penetrate = 2;
            TrailColor = Color.ForestGreen;
        }
    }

    public class AcidicTerrorsDiscTile : BaseRecordTile
    {

    }

    public class ADemiseDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/ADemise";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<ADemiseDiscTile>();
            Penetrate = 2;
            TrailColor = Color.LightGray;
        }
    }

    public class ADemiseDiscTile : BaseRecordTile
    {

    }


    public class AlcadizHurricaneDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/AlcadizHurricane";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<AlcadizHurricaneDiscTile>();
            Penetrate = 2;
            TrailColor = Color.LightSalmon;
        }
    }

    public class AlcadizHurricaneDiscTile : BaseRecordTile
    {

    }

    public class AlcaricFoxDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/AlcaricFox";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<AlcaricFoxDiscTile>();
            Penetrate = 2;
            TrailColor = Color.White;
        }
    }

    public class AlcaricFoxDiscTile : BaseRecordTile
    {

    }

    public class AshotiDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/Ashoti";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<AshotiDiscTile>();
            Penetrate = 2;
            TrailColor = Color.Orange;
        }
    }

    public class AshotiDiscTile : BaseRecordTile
    {

    }

    public class BloodCathedralDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/BloodCathedral";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<BloodCathedralDiscTile>();
            Penetrate = 2;
            TrailColor = Color.Red;
        }
    }

    public class BloodCathedralDiscTile : BaseRecordTile
    {

    }

    public class Boss6Disc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/Boss6";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<Boss6DiscTile>();
            Penetrate = 2;
            TrailColor = Color.Purple;
        }
    }

    public class Boss6DiscTile : BaseRecordTile
    {

    }

    public class CatacombsDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/Catacombs";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<CatacombsDiscTile>();
            Penetrate = 2;
            TrailColor = Color.Brown;
        }
    }

    public class CatacombsDiscTile : BaseRecordTile
    {

    }

    public class CindersparkDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/Cinderspark";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<CindersparkDiscTile>();
            Penetrate = 2;
            TrailColor = Color.OrangeRed;
        }
    }

    public class CindersparkDiscTile : BaseRecordTile
    {

    }

    public class CountingStarsDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/CountingStars";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<CountingStarsDiscTile>();
            Penetrate = 2;
            TrailColor = Color.LightPink;
        }
    }

    public class CountingStarsDiscTile : BaseRecordTile
    {

    }

    public class DaedusDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/Daedus";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<DaedusDiscTile>();
            Penetrate = 2;
            TrailColor = Color.Goldenrod;
        }
    }

    public class DaedusDiscTile : BaseRecordTile
    {

    }

    public class DreadHeartDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/DreadHeart";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<DreadHeartDiscTile>();
            Penetrate = 2;
            TrailColor = Color.DarkRed;
        }
    }

    public class DreadHeartDiscTile : BaseRecordTile
    {

    }

    public class EndingDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/Ending";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<EndingDiscTile>();
            Penetrate = 2;
            TrailColor = Color.LightBlue;
        }
    }

    public class EndingDiscTile : BaseRecordTile
    {

    }

    public class EreshkigalDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/Ereshkigal";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<EreshkigalDiscTile>();
            Penetrate = 2;
            TrailColor = Color.Goldenrod;
        }
    }

    public class EreshkigalDiscTile : BaseRecordTile
    {

    }

    public class GintzicaneDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/Gintzicane";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<GintzicaneDiscTile>();
            Penetrate = 2;
            TrailColor = Color.LightGray;
        }
    }

    public class GintzicaneDiscTile : BaseRecordTile
    {

    }


    public class GothiviaDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/Gothivia";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<GothiviaDiscTile>();
            Penetrate = 2;
            TrailColor = Color.Teal;
        }
    }

    public class GothiviaDiscTile : BaseRecordTile
    {

    }

    public class GovheilCastleDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/GovheilCastle";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<GovheilCastleDiscTile>();
            Penetrate = 2;
            TrailColor = Color.MistyRose;
        }
    }

    public class GovheilCastleDiscTile : BaseRecordTile
    {

    }

    public class HidingInTheShadowsDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/Hidding_In_The_Shadows";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<HidingInTheShadowsDiscTile>();
            Penetrate = 2;
            TrailColor = Color.LightCyan;
        }
    }

    public class HidingInTheShadowsDiscTile : BaseRecordTile
    {

    }

    public class IrradiaDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/Irradia";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<IrradiaDiscTile>();
            Penetrate = 2;
            TrailColor = Color.LightYellow;
        }
    }

    public class IrradiaDiscTile : BaseRecordTile
    {

    }

    public class IshtarDisc : BaseDiscItem
    {
        public override string MusicPath => "Assets/Music/Ishtar";
        public override void SetDiscDefaults()
        {
            base.SetDiscDefaults();
            TileToPlace = ModContent.TileType<IshtarDiscTile>();
            Penetrate = 2;
            TrailColor = Color.White;
        }
    }

    public class IshtarDiscTile : BaseRecordTile
    {

    }
}
