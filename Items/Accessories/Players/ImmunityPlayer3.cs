using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Players
{
    public class ImmunityPlayer3 : ModPlayer
	{
		public bool HasStealiImmunityAcccc;

		// Always reset the accessory field to its default value here.
		public override void ResetEffects()
		{
			HasStealiImmunityAcccc = false;
		}

		// Vanilla applies immunity time before this method and after PreHurt and Hurt
		// Therefore, we should apply our immunity time increment here
	
		public override void PostUpdate()
		{
			if (HasStealiImmunityAcccc)
            {
				Player.immune = true;
				
				
				
				Player.immuneTime = 6;
			}
			
		}
	}
}