using Terraria;
using Terraria.WorldBuilding;


namespace Stellamod.WorldG;
public partial class StellaWorld
{
    public class PaintWall : GenAction
    {
        private byte _type;
        private bool _neighbors;

        public PaintWall(byte type, bool neighbors = true)
        {
            _type = type;
            _neighbors = neighbors;
        }

        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            ref Tilemap tm = ref GenBase._tiles;
            Tile tile = tm[x, y];
            tile.WallColor = _type;
            WorldGen.SquareWallFrame(x, y);
            if (_neighbors)
            {
                WorldGen.SquareWallFrame(x + 1, y);
                WorldGen.SquareWallFrame(x - 1, y);
                WorldGen.SquareWallFrame(x, y - 1);
                WorldGen.SquareWallFrame(x, y + 1);
            }

            return UnitApply(origin, x, y, args);
        }
    }
}