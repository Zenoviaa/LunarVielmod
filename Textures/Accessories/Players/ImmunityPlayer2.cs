using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Players
{
    public class ImmunityPlayer2 : ModPlayer
	{
		public bool HasStealiImmunityAccc;

		// Always reset the accessory field to its default value here.
		public override void ResetEffects()
		{
			HasStealiImmunityAccc = false;
		}

		// Vanilla applies immunity time before this method and after PreHurt and Hurt
		// Therefore, we should apply our immunity time increment here
	
		public override void PostUpdate()
		{
			if (HasStealiImmunityAccc)
            {
				Player.immune = true;
				
				
				
				Player.immuneTime = 6;
			}
			
		}
	}
}