using Terraria;
using Terraria.ModLoader;


namespace Stellamod
{
	public class MyPlayer : ModPlayer
	{
		public const int CAMO_DELAY = 100;

		internal static bool swingingCheck;
		internal static Item swingingItem;

		public int Shake = 0;
	}
}
