using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Terraria.ModLoader;

namespace Stellamod.Items.Discs
{
    internal class AurelusTempleDisc : BaseDiscItem
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

    internal class AurelusTempleDiscTile : BaseRecordTile
    {

    }
}
