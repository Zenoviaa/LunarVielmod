
using Microsoft.Xna.Framework;
using Stellamod.Items.Consumables;
using Stellamod.Tiles;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.TilesAB.Aurelus
{
    //TODO: Smart Cursor Outlines and tModLoader support
    public class AurelusDoorClosed : LockedDoor
    {
        public override int KeyType => ModContent.ItemType<VoidKey>();
        public override string FailString => "You're not ready yet! Key needed!";
        public override Color FailColor => Color.LightSkyBlue;
    }
}
