using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Tiles.Acid
{

    public class AcidWallNatural : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
			AddMapEntry(new Color(60, 80, 60));
		}
	}
}