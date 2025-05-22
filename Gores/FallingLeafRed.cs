using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Gores
{
	public class FallingLeafWhite : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{		
			gore.numFrames = 8;
			gore.frame = (byte)Main.rand.Next(8);
			gore.frameCounter = (byte)Main.rand.Next(8);
			gore.timeLeft = 805;
			UpdateType = 910;
		}
	}
}