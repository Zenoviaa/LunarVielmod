using Terraria;
using Terraria.ModLoader;

namespace Stellamod.UI.Systems
{
	public class ShakeModSystem : ModSystem
	{

		public static float Shake;

		public override void ModifyScreenPosition()
		{
			Main.screenPosition += Utils.RandomVector2(Main.rand, Main.rand.NextFloat(-Shake, Shake), Main.rand.NextFloat(-Shake, Shake));

			if (Shake > 0)
			{
				Shake--;			
			}
		}
	}
}




