using Terraria;
using Terraria.ModLoader;

namespace Stellamod.UI.Systems
{
    public class ShakeModSystem : ModSystem
	{

		private static float _shake;
		public static float Shake
		{
			get
			{
				return _shake;
			}
			set
			{
                LunarVeilClientConfig config = ModContent.GetInstance<LunarVeilClientConfig>();
                if (!config.ShakeToggle)
                    return;
                _shake = value;
			}
		}

		public override void ModifyScreenPosition()
		{

			Main.screenPosition += Utils.RandomVector2(Main.rand, Main.rand.NextFloat(-_shake, _shake), Main.rand.NextFloat(-_shake, _shake));

			if (_shake > 0)
			{
                _shake--;			
			}
		}
	}
}




